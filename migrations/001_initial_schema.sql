-- Pie Shop Database Schema
-- PostgreSQL 16

-- Orders table
CREATE TABLE IF NOT EXISTS orders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    pie_type VARCHAR(50) NOT NULL,
    customer_name VARCHAR(255) NOT NULL,
    customer_email VARCHAR(255) NOT NULL,
    customer_phone VARCHAR(20),
    delivery_street VARCHAR(255) NOT NULL,
    delivery_city VARCHAR(100) NOT NULL,
    delivery_state VARCHAR(2) NOT NULL,
    delivery_zip VARCHAR(10) NOT NULL,
    current_state VARCHAR(20) NOT NULL,
    picker_job_id VARCHAR(255),
    baker_job_id VARCHAR(255),
    delivery_id VARCHAR(255),
    estimated_delivery TIMESTAMP,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- State history table
CREATE TABLE IF NOT EXISTS state_history (
    id SERIAL PRIMARY KEY,
    order_id UUID NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
    from_state VARCHAR(20),
    to_state VARCHAR(20) NOT NULL,
    timestamp TIMESTAMP NOT NULL DEFAULT NOW(),
    notes TEXT,
    error_message TEXT
);

-- Indexes for performance
CREATE INDEX IF NOT EXISTS idx_orders_state ON orders(current_state);
CREATE INDEX IF NOT EXISTS idx_orders_created ON orders(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_history_order ON state_history(order_id, timestamp DESC);

-- Comments for documentation
COMMENT ON TABLE orders IS 'Stores pie orders with customer and delivery information';
COMMENT ON TABLE state_history IS 'Tracks order state transitions for audit trail';
COMMENT ON COLUMN orders.current_state IS 'Current state: ORDERED, PICKING, PREPPING, BAKING, DELIVERING, COMPLETED, ERROR';
