import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import OrderForm from './components/OrderForm';
import AdminDashboard from './components/AdminDashboard';
import OrderStatus from './components/OrderStatus';
import './App.css';

function App() {
  return (
    <BrowserRouter>
      <div className="app">
        <nav className="nav">
          <h1>Pie Shop - Robot Bakery</h1>
          <ul>
            <li><Link to="/">Order</Link></li>
            <li><Link to="/admin">Admin Dashboard</Link></li>
          </ul>
        </nav>
        
        <main className="main">
          <Routes>
            <Route path="/" element={<OrderForm />} />
            <Route path="/admin" element={<AdminDashboard />} />
            <Route path="/order/:orderId" element={<OrderStatus />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}

export default App;
