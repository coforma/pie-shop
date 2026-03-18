-- Pie Shop Initial Schema
-- Migration: 001_initial_schema.sql

CREATE TYPE order_state AS ENUM (
    'ORDERED',
    'PICKING',
    'PREPPING',
    'BAKING',
    'DELIVERING',
    'COMPLETED',
    'ERROR'
);

CREATE TABLE IF NOT EXISTS orders (
    id               VARCHAR(36) PRIMARY KEY,
    pie_type         VARCHAR(50) NOT NULL,
    customer_name    VARCHAR(255) NOT NULL,
    customer_email   VARCHAR(255) NOT NULL,
    customer_phone   VARCHAR(20),
    delivery_street  VARCHAR(255) NOT NULL,
    delivery_city    VARCHAR(100) NOT NULL,
    delivery_state   VARCHAR(2) NOT NULL,
    delivery_zip     VARCHAR(10) NOT NULL,
    current_state    VARCHAR(20) NOT NULL DEFAULT 'ORDERED',
    picker_job_id    VARCHAR(255),
    baker_job_id     VARCHAR(255),
    delivery_id      VARCHAR(255),
    estimated_delivery TIMESTAMP,
    error_message    TEXT,
    created_at       TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at       TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS state_history (
    id            SERIAL PRIMARY KEY,
    order_id      VARCHAR(36) NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
    from_state    VARCHAR(20),
    to_state      VARCHAR(20) NOT NULL,
    timestamp     TIMESTAMP NOT NULL DEFAULT NOW(),
    notes         TEXT,
    error_message TEXT
);

CREATE INDEX idx_orders_state ON orders (current_state);
CREATE INDEX idx_orders_created ON orders (created_at DESC);
CREATE INDEX idx_state_history_order ON state_history (order_id);
