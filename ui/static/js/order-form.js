document.getElementById('orderForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const formData = {
        pieType: document.getElementById('pieType').value,
        customer: {
            name: document.getElementById('name').value,
            email: document.getElementById('email').value,
            phone: document.getElementById('phone').value
        },
        deliveryAddress: {
            street: document.getElementById('street').value,
            city: document.getElementById('city').value,
            state: document.getElementById('state').value,
            zip: document.getElementById('zip').value
        }
    };
    
    const resultDiv = document.getElementById('result');
    
    try {
        const response = await fetch('/api/orders', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        });
        
        const data = await response.json();
        
        if (response.ok) {
            resultDiv.className = 'result success';
            resultDiv.innerHTML = `
                <h3>Order Placed Successfully!</h3>
                <p>Order ID: ${data.orderId}</p>
                <p>Status: ${data.status}</p>
                <p>Estimated Delivery: ${new Date(data.estimatedDelivery).toLocaleString()}</p>
            `;
            document.getElementById('orderForm').reset();
        } else {
            resultDiv.className = 'result error';
            resultDiv.innerHTML = `
                <h3>Order Failed</h3>
                <p>${data.message}</p>
            `;
        }
    } catch (error) {
        resultDiv.className = 'result error';
        resultDiv.innerHTML = `
            <h3>Error</h3>
            <p>Failed to place order. Please try again.</p>
        `;
    }
});
