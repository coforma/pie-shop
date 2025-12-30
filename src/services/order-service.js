const { Order, StateHistory } = require('../models/order');
const Recipe = require('../models/recipe');
const { StateMachine, OrderStates } = require('../core/state-machine');
const FruitPickerClient = require('./fruit-picker-client');
const BakerClient = require('./baker-client');
const DeliveryClient = require('./delivery-client');
const logger = require('../utils/logger');
const { v4: uuidv4 } = require('uuid');

class OrderService {
  constructor() {
    this.stateMachine = new StateMachine();
    this.fruitPickerClient = new FruitPickerClient();
    this.bakerClient = new BakerClient();
    this.deliveryClient = new DeliveryClient();
  }

  async createOrder(orderData) {
    const validPieTypes = ['apple', 'cherry', 'pumpkin', 'pecan', 'blueberry'];
    
    if (!validPieTypes.includes(orderData.pieType)) {
      throw new Error('Invalid pie type');
    }

    const recipe = await Recipe.findById(orderData.pieType);
    if (!recipe) {
      throw new Error('Recipe not found');
    }

    const estimatedDelivery = new Date();
    estimatedDelivery.setHours(estimatedDelivery.getHours() + 2);

    const order = await Order.create({
      id: uuidv4(),
      pieType: orderData.pieType,
      customerName: orderData.customer.name,
      customerEmail: orderData.customer.email,
      customerPhone: orderData.customer.phone,
      deliveryStreet: orderData.deliveryAddress.street,
      deliveryCity: orderData.deliveryAddress.city,
      deliveryState: orderData.deliveryAddress.state,
      deliveryZip: orderData.deliveryAddress.zip,
      currentState: OrderStates.ORDERED,
      estimatedDelivery: estimatedDelivery
    });

    await StateHistory.create({
      orderId: order.id,
      fromState: null,
      toState: OrderStates.ORDERED,
      notes: 'Order created'
    });

    logger.info(`Order created: ${order.id}`);

    this.processOrder(order.id).catch(err => {
      logger.error(`Failed to process order ${order.id}: ${err.message}`);
    });

    return order;
  }

  async getOrder(orderId) {
    const order = await Order.findByPk(orderId, {
      include: [{
        model: StateHistory,
        as: 'history',
        order: [['timestamp', 'ASC']]
      }]
    });

    if (!order) {
      throw new Error('Order not found');
    }

    return order;
  }

  async listOrders() {
    const orders = await Order.findAll({
      order: [['createdAt', 'DESC']],
      limit: 100
    });
    return orders;
  }

  async processOrder(orderId) {
    let order = await Order.findByPk(orderId);
    
    if (order.currentState === OrderStates.ORDERED) {
      await this.transitionState(order, OrderStates.PICKING);
      order = await Order.findByPk(orderId);
      
      const recipe = await Recipe.findById(order.pieType);
      const appleIngredient = recipe.ingredients.find(i => i.item === 'apples' || i.item === order.pieType + 's');
      const quantity = appleIngredient ? appleIngredient.quantity : 6;
      
      try {
        const pickJob = await this.fruitPickerClient.pickFruit(order.pieType, quantity);
        order.pickerJobId = pickJob.jobId;
        await order.save();
        
        await this.sleep(5000);
        
        await this.transitionState(order, OrderStates.PREPPING);
        order = await Order.findByPk(orderId);
      } catch (error) {
        await this.transitionState(order, OrderStates.ERROR, error.message);
        return;
      }
    }

    if (order.currentState === OrderStates.PREPPING) {
      await this.sleep(3000);
      
      await this.transitionState(order, OrderStates.BAKING);
      order = await Order.findByPk(orderId);
      
      const recipe = await Recipe.findById(order.pieType);
      
      try {
        const bakeJob = await this.bakerClient.scheduleBaking(
          order.pieType,
          recipe.bakingTemp,
          recipe.bakingTime
        );
        order.bakerJobId = bakeJob.jobId;
        await order.save();
        
        await this.sleep(8000);
        
        await this.transitionState(order, OrderStates.DELIVERING);
        order = await Order.findByPk(orderId);
      } catch (error) {
        await this.transitionState(order, OrderStates.ERROR, error.message);
        return;
      }
    }

    if (order.currentState === OrderStates.DELIVERING) {
      try {
        const deliveryJob = await this.deliveryClient.scheduleDelivery(
          {
            street: order.deliveryStreet,
            city: order.deliveryCity,
            state: order.deliveryState,
            zip: order.deliveryZip
          },
          {
            type: 'pie',
            size: 'medium',
            temperature: 'warm'
          }
        );
        order.deliveryId = deliveryJob.deliveryId;
        await order.save();
        
        await this.sleep(10000);
        
        await this.transitionState(order, OrderStates.COMPLETED);
      } catch (error) {
        await this.transitionState(order, OrderStates.ERROR, error.message);
        return;
      }
    }
  }

  async transitionState(order, newState, errorMessage = null) {
    const oldState = order.currentState;
    
    if (!this.stateMachine.canTransition(oldState, newState)) {
      throw new Error(`Cannot transition from ${oldState} to ${newState}`);
    }

    order.currentState = newState;
    await order.save();

    await StateHistory.create({
      orderId: order.id,
      fromState: oldState,
      toState: newState,
      errorMessage: errorMessage
    });

    logger.info(`Order ${order.id} transitioned: ${oldState} -> ${newState}`);
  }

  sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
}

module.exports = OrderService;
