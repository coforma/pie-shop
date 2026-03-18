const API_BASE = '/api';

// Order form submission
const orderForm = document.getElementById('orderForm');
if (orderForm) {
    orderForm.addEventListener('submit', async function(e) {
        e.preventDefault();

        const errorEl = document.getElementById('error-message');
        const successEl = document.getElementById('success-message');
        errorEl.style.display = 'none';
        successEl.style.display = 'none';

        const pieType = document.getElementById('pieType').value;
        const name = document.getElementById('customerName').value;
        const email = document.getElementById('customerEmail').value;
        const phone = document.getElementById('customerPhone').value;
        const street = document.getElementById('street').value;
        const city = document.getElementById('city').value;
        const state = document.getElementById('state').value;
        const zip = document.getElementById('zip').value;

        if (!pieType || !name || !email || !street || !city || !state || !zip) {
            errorEl.textContent = 'Please fill in all required fields.';
            errorEl.style.display = 'block';
            return;
        }

        const payload = {
            pieType: pieType,
            customer: { name, email, phone: phone || undefined },
            deliveryAddress: { street, city, state, zip }
        };

        try {
            const response = await fetch(`${API_BASE}/orders`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                const err = await response.json();
                errorEl.textContent = err.detail?.message || 'Failed to place order.';
                errorEl.style.display = 'block';
                return;
            }

            const data = await response.json();
            orderForm.parentElement.style.display = 'none';

            const confirmPanel = document.getElementById('orderConfirmation');
            document.getElementById('confirmedOrderId').textContent = data.orderId;
            document.getElementById('estimatedDelivery').textContent =
                data.estimatedDelivery ? new Date(data.estimatedDelivery).toLocaleString() : 'TBD';
            confirmPanel.style.display = 'block';

        } catch (err) {
            errorEl.textContent = 'Network error. Please try again.';
            errorEl.style.display = 'block';
        }
    });
}

// Admin dashboard functions
async function loadOrders() {
    const status = document.getElementById('filterStatus')?.value || '';
    const pieType = document.getElementById('filterPieType')?.value || '';

    let url = `${API_BASE}/orders`;
    const params = new URLSearchParams();
    if (status) params.set('status', status);
    if (pieType) params.set('pie_type', pieType);
    if (params.toString()) url += '?' + params.toString();

    try {
        const response = await fetch(url);
        const orders = await response.json();
        renderOrdersTable(orders);
        await loadStats();
    } catch (err) {
        console.error('Failed to load orders:', err);
    }
}

function renderOrdersTable(orders) {
    const tbody = document.getElementById('ordersTableBody');
    if (!tbody) return;

    if (orders.length === 0) {
        tbody.innerHTML = '<tr><td colspan="7" style="text-align:center;padding:40px;color:#999">No orders found</td></tr>';
        return;
    }

    tbody.innerHTML = orders.map(order => `
        <tr>
            <td>${order.orderId}</td>
            <td>${order.customerName}</td>
            <td>${order.pieType}</td>
            <td>
                <span class="status-dot status-${order.status}"></span>
                ${order.status}
            </td>
            <td>${new Date(order.createdAt).toLocaleDateString()}</td>
            <td>${order.estimatedDelivery ? new Date(order.estimatedDelivery).toLocaleString() : '--'}</td>
            <td>
                <button class="action-btn" onclick="viewOrder('${order.orderId}')">View</button>
                <button class="action-btn" onclick="advanceOrder('${order.orderId}')">Advance</button>
            </td>
        </tr>
    `).join('');
}

async function viewOrder(orderId) {
    try {
        const response = await fetch(`${API_BASE}/orders/${orderId}`);
        const order = await response.json();

        const detailPanel = document.getElementById('orderDetail');
        const contentEl = document.getElementById('orderDetailContent');
        const historyEl = document.getElementById('orderHistoryContent');

        contentEl.innerHTML = `
            <p><strong>Order ID:</strong> ${order.orderId}</p>
            <p><strong>Pie Type:</strong> ${order.pieType}</p>
            <p><strong>Customer:</strong> ${order.customer?.name} (${order.customer?.email})</p>
            <p><strong>Status:</strong> ${order.status}</p>
            <p><strong>Address:</strong> ${order.deliveryAddress?.street}, ${order.deliveryAddress?.city}, ${order.deliveryAddress?.state}</p>
        `;

        historyEl.innerHTML = (order.history || []).map(h => `
            <div class="history-entry">
                <span class="status-dot status-${h.state}"></span>
                <strong>${h.state}</strong> — ${new Date(h.timestamp).toLocaleString()}
                ${h.notes ? `<br><small>${h.notes}</small>` : ''}
                ${h.error_message ? `<br><small style="color:red">${h.error_message}</small>` : ''}
            </div>
        `).join('') || '<p>No history available</p>';

        detailPanel.style.display = 'block';

    } catch (err) {
        console.error('Failed to load order:', err);
    }
}

async function advanceOrder(orderId) {
    try {
        const response = await fetch(`${API_BASE}/orders/${orderId}/advance`, { method: 'POST' });
        if (response.ok) {
            await loadOrders();
        }
    } catch (err) {
        console.error('Failed to advance order:', err);
    }
}

async function loadStats() {
    try {
        const response = await fetch(`${API_BASE}/admin/stats`);
        const data = await response.json();

        const statuses = data.ordersByStatus || {};
        const activeStatuses = ['ORDERED', 'PICKING', 'PREPPING', 'BAKING', 'DELIVERING'];
        const active = activeStatuses.reduce((sum, s) => sum + (statuses[s] || 0), 0);

        const totalEl = document.getElementById('statTotal');
        const activeEl = document.getElementById('statActive');
        const completedEl = document.getElementById('statCompleted');
        const errorsEl = document.getElementById('statErrors');

        if (totalEl) totalEl.textContent = data.total || 0;
        if (activeEl) activeEl.textContent = active;
        if (completedEl) completedEl.textContent = statuses['COMPLETED'] || 0;
        if (errorsEl) errorsEl.textContent = statuses['ERROR'] || 0;

    } catch (err) {
        console.error('Failed to load stats:', err);
    }
}
