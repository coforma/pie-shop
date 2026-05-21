-- Initial schema for Pie Shop Order Orchestration System
-- Applied automatically by PostgreSQL on first startup

CREATE TABLE IF NOT EXISTS pie_shop_orders (
    id VARCHAR(36) PRIMARY KEY,
    pie_type VARCHAR(50) NOT NULL,
    customer_name VARCHAR(255) NOT NULL,
    customer_email VARCHAR(255) NOT NULL,
    customer_phone VARCHAR(20) DEFAULT '',
    delivery_street VARCHAR(255) NOT NULL,
    delivery_city VARCHAR(100) NOT NULL,
    delivery_state VARCHAR(2) NOT NULL,
    delivery_zip VARCHAR(10) NOT NULL,
    current_state VARCHAR(20) NOT NULL DEFAULT 'ORDERED',
    picker_job_id VARCHAR(255) DEFAULT '',
    baker_job_id VARCHAR(255) DEFAULT '',
    delivery_id VARCHAR(255) DEFAULT '',
    estimated_delivery INTEGER,
    created_at INTEGER NOT NULL,
    updated_at INTEGER NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_orders_state ON pie_shop_orders(current_state);
CREATE INDEX IF NOT EXISTS idx_orders_created ON pie_shop_orders(created_at);

CREATE TABLE IF NOT EXISTS pie_shop_state_history (
    id SERIAL PRIMARY KEY,
    order_id VARCHAR(36) NOT NULL REFERENCES pie_shop_orders(id),
    from_state VARCHAR(20) DEFAULT '',
    to_state VARCHAR(20) NOT NULL,
    timestamp INTEGER NOT NULL,
    notes TEXT,
    error_message TEXT
);

CREATE INDEX IF NOT EXISTS idx_history_order ON pie_shop_state_history(order_id);
