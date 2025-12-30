require('dotenv').config();

module.exports = {
  port: process.env.PORT || 3000,
  nodeEnv: process.env.NODE_ENV || 'development',
  
  database: {
    host: process.env.DB_HOST || 'localhost',
    port: parseInt(process.env.DB_PORT || '5432'),
    name: process.env.DB_NAME || 'piedb',
    user: process.env.DB_USER || 'pieuser',
    password: process.env.DB_PASSWORD || 'password123'
  },

  mongodb: {
    url: process.env.MONGO_URL || 'mongodb://localhost:27017/pierecipes'
  },

  services: {
    fruitPicker: {
      url: process.env.FRUIT_PICKER_URL || 'http://localhost:8081',
      apiKey: 'picker-secret-key-123',
      timeout: 120000
    },
    baker: {
      url: process.env.BAKER_URL || 'http://localhost:8082',
      apiKey: 'baker-secret-key-456',
      timeout: 1800000
    },
    delivery: {
      url: process.env.DELIVERY_URL || 'http://localhost:8083',
      apiKey: 'delivery-secret-key-789',
      timeout: 3600000
    }
  }
};
