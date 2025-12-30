const request = require('supertest');
const app = require('../../src/index');

describe('Order API Integration Tests', () => {
  test('should create an order successfully', async () => {
    const orderData = {
      pieType: 'apple',
      customer: {
        name: 'Test User',
        email: 'test@example.com',
        phone: '+1-555-0123'
      },
      deliveryAddress: {
        street: '123 Main St',
        city: 'Springfield',
        state: 'IL',
        zip: '62701'
      }
    };

    const response = await request(app)
      .post('/api/orders')
      .send(orderData)
      .expect(201);

    expect(response.body).toHaveProperty('orderId');
    expect(response.body).toHaveProperty('status', 'ORDERED');
    expect(response.body).toHaveProperty('estimatedDelivery');
    expect(response.body).toHaveProperty('createdAt');
  });

  test('should reject order with invalid pie type', async () => {
    const orderData = {
      pieType: 'chocolate',
      customer: {
        name: 'Test User',
        email: 'test@example.com'
      },
      deliveryAddress: {
        street: '123 Main St',
        city: 'Springfield',
        state: 'IL',
        zip: '62701'
      }
    };

    const response = await request(app)
      .post('/api/orders')
      .send(orderData)
      .expect(400);

    expect(response.body).toHaveProperty('error');
  });

  test('should retrieve order by ID', async () => {
    const orderData = {
      pieType: 'cherry',
      customer: {
        name: 'Jane Doe',
        email: 'jane@example.com'
      },
      deliveryAddress: {
        street: '456 Oak Ave',
        city: 'Portland',
        state: 'OR',
        zip: '97201'
      }
    };

    const createResponse = await request(app)
      .post('/api/orders')
      .send(orderData);

    const orderId = createResponse.body.orderId;

    const getResponse = await request(app)
      .get(`/api/orders/${orderId}`)
      .expect(200);

    expect(getResponse.body).toHaveProperty('orderId', orderId);
    expect(getResponse.body).toHaveProperty('pieType', 'cherry');
    expect(getResponse.body).toHaveProperty('customer');
    expect(getResponse.body.customer.name).toBe('Jane Doe');
  });

  test('should return 404 for non-existent order', async () => {
    const response = await request(app)
      .get('/api/orders/00000000-0000-0000-0000-000000000000')
      .expect(404);

    expect(response.body).toHaveProperty('error', 'ORDER_NOT_FOUND');
  });

  test('should list all orders', async () => {
    const response = await request(app)
      .get('/api/orders')
      .expect(200);

    expect(response.body).toHaveProperty('orders');
    expect(Array.isArray(response.body.orders)).toBe(true);
  });
});
