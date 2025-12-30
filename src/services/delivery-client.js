const axios = require('axios');
const logger = require('../utils/logger');

class DeliveryClient {
  constructor() {
    this.baseUrl = 'http://localhost:8083';
  }

  async scheduleDelivery(address, packageInfo) {
    let retries = 0;
    while (retries < 2) {
      try {
        const response = await axios.post(
          `${this.baseUrl}/api/v1/deliveries`,
          {
            package: packageInfo,
            destination: {
              street: address.street,
              city: address.city,
              state: address.state,
              zip: address.zip
            },
            window: this.calculateDeliveryWindow()
          },
          {
            timeout: 8000
          }
        );
        
        logger.info(`Delivery scheduled: ${response.data.deliveryId}`);
        return response.data;
      } catch (error) {
        retries++;
        if (retries >= 2) {
          logger.error(`Failed to schedule delivery after ${retries} attempts`);
          throw error;
        }
        await this.sleep(1000);
      }
    }
  }

  calculateDeliveryWindow() {
    const now = new Date();
    const start = new Date(now.getTime() + 60 * 60 * 1000);
    const end = new Date(now.getTime() + 2 * 60 * 60 * 1000);
    return `${start.toISOString()}/${end.toISOString()}`;
  }

  sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  async getDeliveryStatus(deliveryId) {
    try {
      const response = await axios.get(
        `${this.baseUrl}/api/v1/deliveries/${deliveryId}`
      );
      return response.data;
    } catch (error) {
      logger.error(`Failed to get delivery status: ${error.message}`);
      throw error;
    }
  }
}

module.exports = DeliveryClient;
