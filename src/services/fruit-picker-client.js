const axios = require('axios');
const config = require('../core/config');
const logger = require('../utils/logger');

class FruitPickerClient {
  constructor() {
    this.baseUrl = config.services.fruitPicker.url;
    this.apiKey = config.services.fruitPicker.apiKey;
    this.timeout = 10000;
  }

  async pickFruit(fruitType, quantity) {
    try {
      const response = await axios.post(
        `${this.baseUrl}/api/v1/pick-fruit`,
        {
          fruitType: fruitType,
          quantity: quantity,
          quality: 'premium'
        },
        {
          headers: {
            'X-API-Key': this.apiKey
          },
          timeout: this.timeout
        }
      );
      
      logger.info(`Fruit picker job created: ${response.data.jobId}`);
      return response.data;
    } catch (error) {
      if (error.code === 'ECONNABORTED') {
        logger.error('Fruit picker service timeout');
        throw new Error('Fruit picker service timeout');
      }
      logger.error(`Failed to pick fruit: ${error.message}`);
      throw error;
    }
  }

  async getJobStatus(jobId) {
    try {
      const response = await axios.get(
        `${this.baseUrl}/api/v1/jobs/${jobId}`,
        {
          headers: {
            'X-API-Key': this.apiKey
          },
          timeout: 5000
        }
      );
      return response.data;
    } catch (error) {
      logger.error(`Failed to get job status: ${error.message}`);
      throw error;
    }
  }
}

module.exports = FruitPickerClient;
