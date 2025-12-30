const axios = require('axios');
const config = require('../core/config');
const logger = require('../utils/logger');

class BakerClient {
  constructor() {
    this.baseUrl = config.services.baker.url;
    this.apiKey = config.services.baker.apiKey;
  }

  async scheduleBaking(pieType, temperature, duration) {
    try {
      const response = await axios.post(
        `${this.baseUrl}/api/v1/bake`,
        {
          pieType: pieType,
          temperature: temperature,
          duration: duration
        },
        {
          headers: {
            'X-API-Key': this.apiKey
          },
          timeout: 10000
        }
      );
      
      logger.info(`Baking job created: ${response.data.jobId}`);
      return response.data;
    } catch (error) {
      logger.error(`Failed to schedule baking: ${error.message}`);
      throw error;
    }
  }

  async getJobStatus(jobId) {
    const response = await axios.get(
      `${this.baseUrl}/api/v1/jobs/${jobId}`,
      {
        headers: {
          'X-API-Key': this.apiKey
        }
      }
    );
    return response.data;
  }
}

module.exports = BakerClient;
