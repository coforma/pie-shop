const express = require('express');
const OrderService = require('../../services/order-service');
const logger = require('../../utils/logger');

const router = express.Router();
const orderService = new OrderService();

router.post('/orders', async (req, res) => {
  try {
    const order = await orderService.createOrder(req.body);
    
    res.status(201).json({
      orderId: order.id,
      status: order.currentState,
      estimatedDelivery: order.estimatedDelivery,
      createdAt: order.createdAt
    });
  } catch (error) {
    logger.error(`Error creating order: ${error.message}`);
    res.status(400).json({
      error: 'CREATE_ORDER_FAILED',
      message: error.message
    });
  }
});

router.get('/orders/:id', async (req, res) => {
  try {
    const order = await orderService.getOrder(req.params.id);
    
    res.json({
      orderId: order.id,
      pieType: order.pieType,
      customer: {
        name: order.customerName,
        email: order.customerEmail,
        phone: order.customerPhone
      },
      deliveryAddress: {
        street: order.deliveryStreet,
        city: order.deliveryCity,
        state: order.deliveryState,
        zip: order.deliveryZip
      },
      status: order.currentState,
      createdAt: order.createdAt,
      updatedAt: order.updatedAt,
      estimatedDelivery: order.estimatedDelivery,
      history: order.history.map(h => ({
        state: h.toState,
        timestamp: h.timestamp,
        notes: h.notes,
        errorMessage: h.errorMessage
      }))
    });
  } catch (error) {
    logger.error(`Error getting order: ${error.message}`);
    res.status(404).json({
      error: 'ORDER_NOT_FOUND',
      message: error.message
    });
  }
});

router.get('/orders', async (req, res) => {
  try {
    const orders = await orderService.listOrders();
    
    res.json({
      orders: orders.map(order => ({
        orderId: order.id,
        pieType: order.pieType,
        customerName: order.customerName,
        status: order.currentState,
        createdAt: order.createdAt,
        estimatedDelivery: order.estimatedDelivery
      }))
    });
  } catch (error) {
    logger.error(`Error listing orders: ${error.message}`);
    res.status(500).json({
      error: 'LIST_ORDERS_FAILED',
      message: error.message
    });
  }
});

module.exports = router;
