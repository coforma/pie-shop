let allOrders = [];

async function loadOrders() {
    try {
        const response = await fetch('/api/orders');
        const data = await response.json();
        allOrders = data.orders;
        renderOrders(allOrders);
    } catch (error) {
        console.error('Failed to load orders:', error);
    }
}

function renderOrders(orders) {
    const tbody = document.getElementById('ordersBody');
    
    if (orders.length === 0) {
        tbody.innerHTML = '<tr><td colspan="6">No orders found</td></tr>';
        return;
    }
    
    tbody.innerHTML = orders.map(order => `
        <tr>
            <td>${order.orderId}</td>
            <td>${order.customerName}</td>
            <td>${order.pieType}</td>
            <td><span class="status-badge status-${order.status}">${order.status}</span></td>
            <td>${new Date(order.createdAt).toLocaleString()}</td>
            <td><button class="view-btn" onclick="viewOrder('${order.orderId}')">View</button></td>
        </tr>
    `).join('');
}

async function viewOrder(orderId) {
    try {
        const response = await fetch(`/api/orders/${orderId}`);
        const order = await response.json();
        
        const detailDiv = document.getElementById('orderDetail');
        detailDiv.className = 'order-detail visible';
        detailDiv.innerHTML = `
            <h3>Order Details: ${order.orderId}</h3>
            <p><strong>Pie Type:</strong> ${order.pieType}</p>
            <p><strong>Customer:</strong> ${order.customer.name}</p>
            <p><strong>Email:</strong> ${order.customer.email}</p>
            <p><strong>Phone:</strong> ${order.customer.phone || 'N/A'}</p>
            <p><strong>Delivery Address:</strong> ${order.deliveryAddress.street}, ${order.deliveryAddress.city}, ${order.deliveryAddress.state} ${order.deliveryAddress.zip}</p>
            <p><strong>Current Status:</strong> ${order.status}</p>
            <p><strong>Estimated Delivery:</strong> ${new Date(order.estimatedDelivery).toLocaleString()}</p>
            
            <h4>Order History</h4>
            ${order.history.map(h => `
                <div class="history-item">
                    <strong>${h.state}</strong> - ${new Date(h.timestamp).toLocaleString()}
                    ${h.errorMessage ? `<br><span style="color: red;">Error: ${h.errorMessage}</span>` : ''}
                </div>
            `).join('')}
        `;
    } catch (error) {
        console.error('Failed to load order details:', error);
    }
}

document.getElementById('statusFilter').addEventListener('change', (e) => {
    const status = e.target.value;
    if (status === '') {
        renderOrders(allOrders);
    } else {
        const filtered = allOrders.filter(order => order.status === status);
        renderOrders(filtered);
    }
});

loadOrders();
