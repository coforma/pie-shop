const API_BASE = '/api';

async function loadOrders() {
  const status = document.getElementById('filter-status').value;
  const pieType = document.getElementById('filter-pie-type').value;

  let url = API_BASE + '/orders?';
  if (status) url += 'status=' + status + '&';
  if (pieType) url += 'pie_type=' + pieType;

  try {
    const response = await fetch(url);
    const data = await response.json();
    renderOrders(data.orders || []);
  } catch (err) {
    document.getElementById('orders-body').innerHTML = '<tr><td colspan="6">Failed to load orders.</td></tr>';
  }
}

function renderOrders(orders) {
  const tbody = document.getElementById('orders-body');

  if (orders.length === 0) {
    tbody.innerHTML = '<tr><td colspan="6">No orders found.</td></tr>';
    return;
  }

  tbody.innerHTML = orders.map(order => `
    <tr>
      <td>${order.orderId.substring(0, 8)}...</td>
      <td>${order.customer.name}</td>
      <td>${order.pieType}</td>
      <td>
        <span class="status-dot status-${order.status}"></span>
        ${order.status}
      </td>
      <td>${new Date(order.createdAt).toLocaleDateString()}</td>
      <td><a href="#" onclick="viewOrder('${order.orderId}')">View</a></td>
    </tr>
  `).join('');
}

async function viewOrder(orderId) {
  try {
    const response = await fetch(API_BASE + '/orders/' + orderId);
    const order = await response.json();

    const detail = document.getElementById('order-detail');
    const content = document.getElementById('order-detail-content');

    content.innerHTML = `
      <p><strong>Order ID:</strong> ${order.orderId}</p>
      <p><strong>Pie Type:</strong> ${order.pieType}</p>
      <p><strong>Customer:</strong> ${order.customer.name} (${order.customer.email})</p>
      <p><strong>Status:</strong> ${order.status}</p>
      <p><strong>Delivery Address:</strong> ${order.deliveryAddress.street}, ${order.deliveryAddress.city}, ${order.deliveryAddress.state} ${order.deliveryAddress.zip}</p>
      <h4>History</h4>
      <ul>
        ${(order.history || []).map(h => `<li>${h.state} - ${new Date(h.timestamp).toLocaleString()}${h.notes ? ' (' + h.notes + ')' : ''}</li>`).join('')}
      </ul>
    `;

    detail.style.display = 'block';
  } catch (err) {
    alert('Failed to load order details.');
  }
}

// Load orders on page load
loadOrders();
