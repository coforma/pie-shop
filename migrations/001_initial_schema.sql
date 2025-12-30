-- Migration: Initial schema for Pie Shop orders
-- Version: 001
-- Date: 2025-12-17

CREATE TABLE IF NOT EXISTS orders (
    id UUID PRIMARY KEY,
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
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS state_history (
    id SERIAL PRIMARY KEY,
    order_id UUID NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
    from_state VARCHAR(20),
    to_state VARCHAR(20) NOT NULL,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    notes TEXT,
    error_message TEXT
);

CREATE INDEX idx_orders_current_state ON orders(current_state);
CREATE INDEX idx_orders_created_at ON orders(created_at);
CREATE INDEX idx_state_history_order_id ON state_history(order_id);
