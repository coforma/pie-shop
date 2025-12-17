import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { apiClient } from '../services/apiClient';
import styles from './OrderForm.module.css';

interface Recipe {
  id: string;
  name: string;
  description: string;
  bakingTime: number;
  difficulty: string;
}

/**
 * Order form component for placing pie orders
 */
function OrderForm() {
  const [recipes, setRecipes] = useState<Recipe[]>([]);
  const [selectedPie, setSelectedPie] = useState('');
  const [customerName, setCustomerName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [street, setStreet] = useState('');
  const [city, setCity] = useState('');
  const [state, setState] = useState('');
  const [zip, setZip] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    loadRecipes();
  }, []);

  const loadRecipes = async () => {
    try {
      const data = await apiClient.getCatalog();
      setRecipes(data);
    } catch (err) {
      setError('Failed to load pie catalog');
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const order = await apiClient.createOrder({
        pieType: selectedPie,
        customer: {
          name: customerName,
          email: email,
          phone: phone || undefined
        },
        deliveryAddress: {
          street,
          city,
          state,
          zip
        }
      });

      navigate(`/order/${order.orderId}`);
    } catch (err) {
      // ACCESSIBILITY ISSUE: Error not announced to screen readers
      setError('Failed to create order. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.container}>
      <h2>Order Your Pie</h2>
      
      {/* ACCESSIBILITY ISSUE: Error message not in aria-live region */}
      {error && <div className={styles.error}>{error}</div>}

      <form onSubmit={handleSubmit} className={styles.form}>
        <div className={styles.section}>
          <h3>Select Pie Type</h3>
          
          {/* ACCESSIBILITY ISSUE: No label element, no keyboard navigation pattern */}
          <div className={styles.pieGrid}>
            {recipes.map(recipe => (
              <div
                key={recipe.id}
                className={`${styles.pieCard} ${selectedPie === recipe.id ? styles.selected : ''}`}
                onClick={() => setSelectedPie(recipe.id)}
              >
                <div className={styles.pieName}>{recipe.name}</div>
                <div className={styles.pieDesc}>{recipe.description}</div>
                <div className={styles.pieInfo}>
                  {recipe.bakingTime} min • {recipe.difficulty}
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className={styles.section}>
          <h3>Customer Information</h3>
          
          {/* ACCESSIBILITY ISSUE: Input fields missing label elements */}
          <div className={styles.field}>
            <span>Name</span>
            <input
              type="text"
              value={customerName}
              onChange={(e) => setCustomerName(e.target.value)}
              required
              className={styles.input}
            />
          </div>

          <div className={styles.field}>
            <span>Email</span>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className={styles.input}
            />
          </div>

          <div className={styles.field}>
            <span>Phone (optional)</span>
            <input
              type="tel"
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
              className={styles.input}
            />
          </div>
        </div>

        <div className={styles.section}>
          <h3>Delivery Address</h3>
          
          {/* More missing labels */}
          <div className={styles.field}>
            <span>Street Address</span>
            <input
              type="text"
              value={street}
              onChange={(e) => setStreet(e.target.value)}
              required
              className={styles.input}
            />
          </div>

          <div className={styles.row}>
            <div className={styles.field}>
              <span>City</span>
              <input
                type="text"
                value={city}
                onChange={(e) => setCity(e.target.value)}
                required
                className={styles.input}
              />
            </div>

            <div className={styles.field}>
              <span>State</span>
              <input
                type="text"
                value={state}
                onChange={(e) => setState(e.target.value)}
                required
                maxLength={2}
                className={styles.input}
              />
            </div>

            <div className={styles.field}>
              <span>ZIP</span>
              <input
                type="text"
                value={zip}
                onChange={(e) => setZip(e.target.value)}
                required
                className={styles.input}
              />
            </div>
          </div>
        </div>

        {/* ACCESSIBILITY ISSUE: Poor color contrast (see CSS) */}
        <button
          type="submit"
          disabled={loading || !selectedPie}
          className={styles.submit}
        >
          {loading ? 'Placing Order...' : 'Place Order'}
        </button>
      </form>
    </div>
  );
}

export default OrderForm;
