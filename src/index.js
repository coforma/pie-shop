const express = require('express');
const path = require('path');
const config = require('./core/config');
const logger = require('./utils/logger');
const { sequelize } = require('./models/order');
const ordersRouter = require('./api/routes/orders');
const healthRouter = require('./api/routes/health');

const app = express();

app.use(express.json());
app.use(express.urlencoded({ extended: true }));

app.use(express.static(path.join(__dirname, '../ui/static')));

app.get('/', (req, res) => {
  res.sendFile(path.join(__dirname, '../ui/templates/order-form.html'));
});

app.get('/admin', (req, res) => {
  res.sendFile(path.join(__dirname, '../ui/templates/admin-dashboard.html'));
});

app.use('/api', ordersRouter);
app.use('/api', healthRouter);

app.use((err, req, res, next) => {
  logger.error(`Unhandled error: ${err.message}`);
  res.status(500).json({
    error: 'INTERNAL_SERVER_ERROR',
    message: 'An unexpected error occurred'
  });
});

async function startServer() {
  try {
    await sequelize.authenticate();
    logger.info('Database connection established');

    app.listen(config.port, () => {
      logger.info(`Server running on port ${config.port}`);
      logger.info(`Environment: ${config.nodeEnv}`);
    });
  } catch (error) {
    logger.error(`Failed to start server: ${error.message}`);
    process.exit(1);
  }
}

startServer();

module.exports = app;
