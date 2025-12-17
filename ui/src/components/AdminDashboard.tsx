import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { apiClient } from '../services/apiClient';
import styles from './AdminDashboard.module.css';

interface OrderSummary {
  orderId: string;
  customerName: string;
  pieType: string;
  status: string;
  createdAt: string;
}

/**
 * Admin dashboard component for monitoring all orders
 */
function AdminDashboard() {
  const [orders, setOrders] = useState<OrderSummary[]>([]);
  const [filter, setFilter] = useState('all');
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    loadOrders();
    const interval = setInterval(loadOrders, 10000); // Refresh every 10 seconds
    return () => clearInterval(interval);
  }, []);

  const loadOrders = async () => {
    try {
      const data = await apiClient.getAllOrders();
      setOrders(data);
      setLoading(false);
    } catch (err) {
      console.error('Failed to load orders', err);
      setLoading(false);
    }
  };

  const filteredOrders = filter === 'all'
    ? orders
    : orders.filter(o => o.status === filter);

  const getStatusColor = (status: string) => {
    // ACCESSIBILITY ISSUE: Using color alone to convey status
    switch (status) {
      case 'ORDERED': return '#3498db';
      case 'PICKING': return '#9b59b6';
      case 'PREPPING': return '#f39c12';
      case 'BAKING': return '#e67e22';
      case 'DELIVERING': return '#1abc9c';
      case 'COMPLETED': return '#2ecc71';
      case 'ERROR': return '#e74c3c';
      default: return '#95a5a6';
    }
  };

  if (loading) {
    return <div className={styles.container}>Loading...</div>;
  }

  return (
    <div className={styles.container}>
      {/* ACCESSIBILITY ISSUE: Improper heading hierarchy - should be h2, not jumping to h4 later */}
      <h2>Admin Dashboard</h2>
      
      {/* ACCESSIBILITY ISSUE: ARIA-label on non-interactive div is misused */}
      <div className={styles.stats} aria-label="Order statistics">
        <div className={styles.statCard}>
          {/* ACCESSIBILITY ISSUE: Heading hierarchy jumps from h2 to h4 */}
          <h4>Total Orders</h4>
          <div className={styles.statValue}>{orders.length}</div>
        </div>
        <div className={styles.statCard}>
          <h4>In Progress</h4>
          <div className={styles.statValue}>
            {orders.filter(o => !['COMPLETED', 'ERROR'].includes(o.status)).length}
          </div>
        </div>
        <div className={styles.statCard}>
          <h4>Completed</h4>
          <div className={styles.statValue}>
            {orders.filter(o => o.status === 'COMPLETED').length}
          </div>
        </div>
        <div className={styles.statCard}>
          <h4>Errors</h4>
          <div className={styles.statValue}>
            {orders.filter(o => o.status === 'ERROR').length}
          </div>
        </div>
      </div>

      <div className={styles.controls}>
        <div className={styles.filters}>
          {/* Basic filter buttons - could use better semantics */}
          <button
            className={filter === 'all' ? styles.activeFilter : ''}
            onClick={() => setFilter('all')}
          >
            All
          </button>
          <button
            className={filter === 'ORDERED' ? styles.activeFilter : ''}
            onClick={() => setFilter('ORDERED')}
          >
            Ordered
          </button>
          <button
            className={filter === 'PICKING' ? styles.activeFilter : ''}
            onClick={() => setFilter('PICKING')}
          >
            Picking
          </button>
          <button
            className={filter === 'BAKING' ? styles.activeFilter : ''}
            onClick={() => setFilter('BAKING')}
          >
            Baking
          </button>
          <button
            className={filter === 'DELIVERING' ? styles.activeFilter : ''}
            onClick={() => setFilter('DELIVERING')}
          >
            Delivering
          </button>
          <button
            className={filter === 'COMPLETED' ? styles.activeFilter : ''}
            onClick={() => setFilter('COMPLETED')}
          >
            Completed
          </button>
          <button
            className={filter === 'ERROR' ? styles.activeFilter : ''}
            onClick={() => setFilter('ERROR')}
          >
            Errors
          </button>
        </div>
        <button onClick={loadOrders} className={styles.refresh}>
          Refresh
        </button>
      </div>

      {/* ACCESSIBILITY ISSUE: Table missing proper structure */}
      <div className={styles.tableContainer}>
        <table className={styles.table}>
          {/* ACCESSIBILITY ISSUE: thead missing, th missing scope attribute */}
          <tr>
            <td className={styles.header}>Order ID</td>
            <td className={styles.header}>Customer</td>
            <td className={styles.header}>Pie Type</td>
            <td className={styles.header}>Status</td>
            <td className={styles.header}>Created</td>
            <td className={styles.header}>Actions</td>
          </tr>
          {filteredOrders.map(order => (
            <tr key={order.orderId}>
              <td>{order.orderId.substring(0, 8)}...</td>
              <td>{order.customerName}</td>
              <td>{order.pieType}</td>
              <td>
                {/* ACCESSIBILITY ISSUE: Status conveyed by color only */}
                <span
                  className={styles.status}
                  style={{ backgroundColor: getStatusColor(order.status) }}
                >
                  {order.status}
                </span>
              </td>
              <td>{new Date(order.createdAt).toLocaleString()}</td>
              <td>
                <button
                  onClick={() => navigate(`/order/${order.orderId}`)}
                  className={styles.viewButton}
                >
                  View
                </button>
              </td>
            </tr>
          ))}
        </table>
      </div>

      {filteredOrders.length === 0 && (
        <div className={styles.empty}>No orders found</div>
      )}
    </div>
  );
}

export default AdminDashboard;
