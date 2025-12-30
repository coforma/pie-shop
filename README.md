# Pie Shop Order Orchestration System

A distributed system for managing pie orders through robot services - from ingredient sourcing to drone delivery.

## Quick Start

### Prerequisites
- Docker and Docker Compose
- Node.js 18+ (for local development)

### Running Locally

1. Clone the repository
```bash
git clone <repository-url>
cd pie-shop
```

2. Start all services
```bash
docker-compose up
```

3. Wait for services to start, then access:
- Customer Order Form: http://localhost:8080
- Admin Dashboard: http://localhost:8080/admin
- API: http://localhost:8080/api

### Manual Setup (without Docker)

1. Install dependencies
```bash
npm install
```

2. Set up environment variables
```bash
cp .env.example .env
```

3. Start PostgreSQL and MongoDB

4. Run database migrations
```bash
npm run migrate
```

5. Seed recipe data
```bash
node migrations/seed-recipes.js
```

6. Start the application
```bash
npm start
```

## API Endpoints

### Create Order
```
POST /api/orders
Content-Type: application/json

{
  "pieType": "apple",
  "customer": {
    "name": "John Doe",
    "email": "john@example.com",
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
GET /api/orders/{orderId}
```

### List Orders
```
GET /api/orders
```

## System Architecture

The system orchestrates pie orders through multiple stages:

1. **ORDERED** - Customer places order
2. **PICKING** - Robot picks fresh fruit
3. **PREPPING** - Ingredients are prepared
4. **BAKING** - Pie is baked in robot oven
5. **DELIVERING** - Drone delivers to customer
6. **COMPLETED** - Order fulfilled

## External Services

The system integrates with three mock robot services:

- **Fruit Picker Service** (port 8081) - Harvests fresh ingredients
- **Baker Service** (port 8082) - Bakes pies in robot ovens
- **Delivery Service** (port 8083) - Drone delivery coordination

## Available Pie Types

- Apple
- Cherry
- Pumpkin
- Pecan
- Blueberry

## Development

### Running Tests
```bash
npm test
```

### Running in Development Mode
```bash
npm run dev
```

## Project Structure

```
pie-shop/
├── src/
│   ├── api/           # API routes and middleware
│   ├── core/          # Core business logic
│   ├── services/      # External service clients
│   ├── models/        # Database models
│   └── utils/         # Utility functions
├── tests/             # Test files
├── mocks/             # Mock external services
├── ui/                # User interface
├── migrations/        # Database migrations
└── docker/            # Docker configuration
```

## Troubleshooting

### Services not starting
Make sure Docker is running and ports 3000, 5432, 27017, 8081-8083 are available.

### Database connection errors
Check that PostgreSQL and MongoDB containers are running:
```bash
docker-compose ps
```

## License

MIT
