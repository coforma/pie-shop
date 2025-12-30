const { Sequelize, DataTypes } = require('sequelize');
const config = require('../core/config');

const sequelize = new Sequelize(
  config.database.name,
  config.database.user,
  config.database.password,
  {
    host: config.database.host,
    port: config.database.port,
    dialect: 'postgres',
    logging: false
  }
);

const Order = sequelize.define('Order', {
  id: {
    type: DataTypes.UUID,
    primaryKey: true,
    defaultValue: DataTypes.UUIDV4
  },
  pieType: {
    type: DataTypes.STRING(50),
    allowNull: false,
    field: 'pie_type'
  },
  customerName: {
    type: DataTypes.STRING(255),
    allowNull: false,
    field: 'customer_name'
  },
  customerEmail: {
    type: DataTypes.STRING(255),
    allowNull: false,
    field: 'customer_email'
  },
  customerPhone: {
    type: DataTypes.STRING(20),
    field: 'customer_phone'
  },
  deliveryStreet: {
    type: DataTypes.STRING(255),
    allowNull: false,
    field: 'delivery_street'
  },
  deliveryCity: {
    type: DataTypes.STRING(100),
    allowNull: false,
    field: 'delivery_city'
  },
  deliveryState: {
    type: DataTypes.STRING(2),
    allowNull: false,
    field: 'delivery_state'
  },
  deliveryZip: {
    type: DataTypes.STRING(10),
    allowNull: false,
    field: 'delivery_zip'
  },
  currentState: {
    type: DataTypes.STRING(20),
    allowNull: false,
    field: 'current_state'
  },
  pickerJobId: {
    type: DataTypes.STRING(255),
    field: 'picker_job_id'
  },
  bakerJobId: {
    type: DataTypes.STRING(255),
    field: 'baker_job_id'
  },
  deliveryId: {
    type: DataTypes.STRING(255),
    field: 'delivery_id'
  },
  estimatedDelivery: {
    type: DataTypes.DATE,
    field: 'estimated_delivery'
  }
}, {
  tableName: 'orders',
  timestamps: true,
  underscored: true
});

const StateHistory = sequelize.define('StateHistory', {
  id: {
    type: DataTypes.INTEGER,
    primaryKey: true,
    autoIncrement: true
  },
  orderId: {
    type: DataTypes.UUID,
    allowNull: false,
    field: 'order_id'
  },
  fromState: {
    type: DataTypes.STRING(20),
    field: 'from_state'
  },
  toState: {
    type: DataTypes.STRING(20),
    allowNull: false,
    field: 'to_state'
  },
  timestamp: {
    type: DataTypes.DATE,
    defaultValue: DataTypes.NOW
  },
  notes: {
    type: DataTypes.TEXT
  },
  errorMessage: {
    type: DataTypes.TEXT,
    field: 'error_message'
  }
}, {
  tableName: 'state_history',
  timestamps: false
});

Order.hasMany(StateHistory, { foreignKey: 'orderId', as: 'history' });
StateHistory.belongsTo(Order, { foreignKey: 'orderId' });

module.exports = {
  sequelize,
  Order,
  StateHistory
};
