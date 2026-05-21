# Pie Shop Order Orchestration System

A Drupal 10 backend for managing the complete lifecycle of pie orders, from ingredient sourcing through robotic preparation and drone delivery.

## Overview

The system coordinates multiple robot services to fulfill pie orders:

1. Customer places order via REST API or web form
2. Robot fruit picker harvests fresh ingredients
3. Ingredients are prepared (washing, peeling, dough making)
4. Robot baker bakes the pie
5. Drone delivers the order to the customer

## Requirements

- Docker and Docker Compose
- PHP 8.2+ (for local development without Docker)
- Composer 2.x

## Quick Start

```bash
git clone <repository-url>
cd pie-shop
docker-compose up
```

The application will be available at `http://localhost:8080`.

- Customer order form: `http://localhost:8080/order`
- Admin dashboard: `http://localhost:8080/admin/pie-shop`
- API: `http://localhost:8080/api/orders`

## API Reference

### Create Order

```
POST http://localhost:3000/api/orders
Content-Type: application/json

{
  "pieType": "apple",
  "customer": {
    "name": "Jane Doe",
    "email": "jane@example.com",
    "phone": "+1-555-0123"
  },
  "deliveryAddress": {
    "street": "123 Main St",
    "city": "Springfield",
    "state": "IL",
    "zip": "62701"
  }
}
```

### Get Order Status

```
GET http://localhost:3000/api/orders/{orderId}
```

### List Orders

```
GET http://localhost:3000/api/orders
```

## Project Structure

```
pie-shop/
├── src/modules/pie_shop/     # Drupal custom module
│   ├── src/
│   │   ├── Controller/       # API and UI controllers
│   │   ├── Service/          # Business logic and service clients
│   │   └── Plugin/           # Queue workers
│   └── *.yml                 # Drupal configuration files
├── tests/                    # PHPUnit tests
├── mocks/                    # Mock robot services
├── ui/                       # Frontend templates and assets
├── migrations/               # Database schema and seed data
├── config/                   # Environment configuration
└── docker-compose.yml
```

## Available Pie Types

- Apple
- Cherry
- Pumpkin
- Pecan
- Blueberry

## Order States

Orders progress through the following states:

`ORDERED` → `PICKING` → `PREPPING` → `BAKING` → `DELIVERING` → `COMPLETED`

Any state can transition to `ERROR` on failure.

## Running Tests

```bash
composer install
./vendor/bin/phpunit tests/
```

## Mock Services

Three mock services simulate the robot integrations:

| Service | Port | Description |
|---------|------|-------------|
| Fruit Picker | 8081 | Simulates ingredient harvesting (30-60s) |
| Baker | 8082 | Simulates pie baking (scaled down timing) |
| Delivery | 8083 | Simulates drone delivery (10-30 min) |

Each mock has a ~10% random failure rate to test error handling.

## Configuration

Copy `config/development.env` and adjust values for your environment. See `config/production.env.example` for production settings.

## License

GPL-2.0-or-later
