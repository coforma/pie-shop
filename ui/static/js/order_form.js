const API_BASE = '/api';

document.getElementById('order-form').addEventListener('submit', async function(e) {
  e.preventDefault();

  const errorDiv = document.getElementById('form-error');
  errorDiv.style.display = 'none';

  const payload = {
    pieType: document.getElementById('pieType').value,
    customer: {
      name: document.getElementById('customerName').value,
      email: document.getElementById('customerEmail').value,
      phone: document.getElementById('customerPhone').value,
    },
    deliveryAddress: {
      street: document.getElementById('street').value,
      city: document.getElementById('city').value,
      state: document.getElementById('state').value,
      zip: document.getElementById('zip').value,
    },
  };

  try {
    const response = await fetch(API_BASE + '/orders', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });

    const data = await response.json();

    if (!response.ok) {
      errorDiv.textContent = data.message || 'Something went wrong. Please try again.';
      errorDiv.style.display = 'block';
      return;
    }

    document.getElementById('order-form-container').style.display = 'none';
    document.getElementById('order-confirmation').style.display = 'block';
    document.getElementById('order-id').textContent = data.orderId;
    document.getElementById('estimated-delivery').textContent = new Date(data.estimatedDelivery).toLocaleString();

  } catch (err) {
    errorDiv.textContent = 'Unable to connect to the server. Please try again.';
    errorDiv.style.display = 'block';
  }
});
