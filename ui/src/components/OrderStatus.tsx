import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { apiClient } from '../services/apiClient';

interface OrderDetail {
  orderId: string;
  pieType: string;
  customer: {
    name: string;
    email: string;
    phone?: string;
  };
  deliveryAddress: {
    street: string;
    city: string;
    state: string;
    zip: string;
  };
  status: string;
  createdAt: string;
  updatedAt: string;
  estimatedDelivery?: string;
  history: Array<{
    state: string;
    timestamp: string;
  }>;
}

function OrderStatus() {
  const { orderId } = useParams<{ orderId: string }>();
  const [order, setOrder] = useState<OrderDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    if (orderId) {
      loadOrder(orderId);
      const interval = setInterval(() => loadOrder(orderId), 5000);
      return () => clearInterval(interval);
    }
  }, [orderId]);

  const loadOrder = async (id: string) => {
    try {
      const data = await apiClient.getOrder(id);
      setOrder(data);
      setLoading(false);
    } catch (err) {
      setError('Failed to load order details');
      setLoading(false);
    }
  };

  if (loading) {
    return <div style={{ padding: '2rem' }}>Loading order details...</div>;
  }

  if (error || !order) {
    return (
      <div style={{ padding: '2rem' }}>
        <h2>Error</h2>
        <p>{error || 'Order not found'}</p>
        <Link to="/">Back to Order Form</Link>
      </div>
    );
  }

  return (
    <div style={{ background: 'white', padding: '2rem', borderRadius: '8px' }}>
      <h2>Order Status</h2>
      
      <div style={{ marginBottom: '2rem' }}>
        <p><strong>Order ID:</strong> {order.orderId}</p>
        <p><strong>Status:</strong> <span style={{ 
          color: order.status === 'COMPLETED' ? '#2ecc71' : 
                 order.status === 'ERROR' ? '#e74c3c' : '#3498db',
          fontWeight: 'bold'
        }}>{order.status}</span></p>
        <p><strong>Pie Type:</strong> {order.pieType}</p>
        {order.estimatedDelivery && (
          <p><strong>Estimated Delivery:</strong> {new Date(order.estimatedDelivery).toLocaleString()}</p>
        )}
      </div>

      <h3>Customer Information</h3>
      <p><strong>Name:</strong> {order.customer.name}</p>
      <p><strong>Email:</strong> {order.customer.email}</p>
      {order.customer.phone && <p><strong>Phone:</strong> {order.customer.phone}</p>}

      <h3>Delivery Address</h3>
      <p>
        {order.deliveryAddress.street}<br />
        {order.deliveryAddress.city}, {order.deliveryAddress.state} {order.deliveryAddress.zip}
      </p>

      <h3>Order History</h3>
      <div>
        {order.history.map((entry, index) => (
          <div key={index} style={{ 
            padding: '0.5rem', 
            borderLeft: '3px solid #3498db',
            marginBottom: '0.5rem',
            paddingLeft: '1rem'
          }}>
            <strong>{entry.state}</strong> - {new Date(entry.timestamp).toLocaleString()}
          </div>
        ))}
      </div>

      <div style={{ marginTop: '2rem' }}>
        <Link to="/">Place Another Order</Link>
        {' | '}
        <Link to="/admin">View All Orders</Link>
      </div>
    </div>
  );
}

export default OrderStatus;
