const express = require('express');
const { v4: uuidv4 } = require('uuid');

const app = express();
app.use(express.json());

const deliveries = new Map();

app.post('/api/v1/deliveries', (req, res) => {
  const { package: pkg, destination, window } = req.body;
  
  const deliveryId = `del_${uuidv4().substring(0, 8)}`;
  const droneId = `drone-${Math.floor(Math.random() * 20) + 1}`;
  
  const shouldFail = Math.random() < 0.08;
  
  if (shouldFail) {
    return res.status(400).json({
      error: 'WEATHER_RESTRICTION',
      message: 'High winds preventing drone flights'
    });
  }
  
  const eta = new Date();
  eta.setMinutes(eta.getMinutes() + 20);
  
  deliveries.set(deliveryId, {
    deliveryId,
    droneId,
    package: pkg,
    destination,
    status: 'IN_TRANSIT',
    eta: eta.toISOString(),
    location: {
      lat: 40.7128,
      lon: -74.0060
    }
  });
  
  setTimeout(() => {
    const delivery = deliveries.get(deliveryId);
    if (delivery) {
      delivery.status = 'DELIVERED';
      delivery.deliveredAt = new Date().toISOString();
    }
  }, 10000);
  
  res.status(201).json({
    deliveryId,
    droneId,
    eta: eta.toISOString()
  });
});

app.get('/api/v1/deliveries/:deliveryId', (req, res) => {
  const delivery = deliveries.get(req.params.deliveryId);
  
  if (!delivery) {
    return res.status(404).json({
      error: 'DELIVERY_NOT_FOUND',
      message: 'Delivery not found'
    });
  }
  
  res.json(delivery);
});

const PORT = process.env.PORT || 8083;
app.listen(PORT, () => {
  console.log(`Delivery mock service running on port ${PORT}`);
});
