# Pie Shop - Automated Bakery Order System

A distributed order orchestration system for an automated pie bakery that coordinates robot services for ingredient sourcing, baking, and delivery.

## Overview

The Pie Shop system manages the complete lifecycle of pie orders from placement through delivery. It orchestrates three external robot services:

- **Fruit Picker**: Harvests fresh ingredients
- **Baker**: Handles baking operations  
- **Delivery**: Manages drone delivery

## Quick Start

### Prerequisites
- Docker Desktop
- .NET 8.0 SDK (for local development)
- Node.js 18+ (for UI development)

### Running with Docker Compose

```bash
# Start all services
docker-compose up --build

# Access the application
# - Customer UI: http://localhost:3000
# - Admin Dashboard: http://localhost:3000/admin
# - API: http://localhost:5000
# - API Documentation: http://localhost:5000/swagger
```

### Running Services Individually

**API:**
```bash
cd src/PieShop.Api
dotnet run
```

**React UI:**
```bash
cd ui
npm install
npm run dev
```

**Mock Services:**
```bash
cd mocks/FruitPickerMock && dotnet run  # Port 8081
cd mocks/BakerMock && dotnet run        # Port 8082
cd mocks/DeliveryMock && dotnet run     # Port 8083
```

## Architecture

### Project Structure

```
pie-shop/
├── src/
│   ├── PieShop.Api/           # REST API
│   ├── PieShop.Core/          # Business logic & state machine
│   ├── PieShop.Infrastructure/ # Data access & external services
│   └── PieShop.Tests/         # Unit & integration tests
├── ui/                        # React frontend
├── mocks/                     # Mock external services
└── docker/                    # Docker configuration
```

### Technology Stack

**Backend:**
- .NET 8.0 / ASP.NET Core
- Entity Framework Core
- PostgreSQL 16
- MongoDB 7.0

**Frontend:**
- React 18.2
- TypeScript 5.3
- Vite 5.0

**Infrastructure:**
- Docker & Docker Compose

### State Machine

Orders progress through the following states:

```
ORDERED → PICKING → PREPPING → BAKING → DELIVERING → COMPLETED
                                                   ↓
                                                ERROR
```

## API Endpoints

### Order Management

**Create Order**
```http
POST /api/orders
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

**Get Order Status**
```http
GET /api/orders/{orderId}
```

**List All Orders**
```http
GET /api/orders
```

**Get Pie Catalog**
```http
GET /api/catalog
```

**Health Check**
```http
GET /api/health
```

### Response Format

**Success (201 Created):**
```json
{
  "orderId": "550e8400-e29b-41d4-a716-446655440000",
  "status": "ORDERED",
  "estimatedDelivery": "2025-12-17T18:30:00Z",
  "createdAt": "2025-12-17T16:00:00Z"
}
```

**Error (400 Bad Request):**
```json
{
  "error": "INVALID_PIE_TYPE",
  "message": "Pie type 'chocolate' is not available",
  "availableTypes": ["apple", "cherry", "pumpkin", "pecan", "blueberry"]
}
```

## Available Pies

- Apple Pie (45 min bake time)
- Cherry Pie (40 min bake time)
- Pumpkin Pie (50 min bake time)
- Pecan Pie (55 min bake time)
- Blueberry Pie (42 min bake time)

## Data Storage

**PostgreSQL** - Transactional order data:
- Orders table
- State history table

**MongoDB** - Recipe catalog:
- Pie recipes with ingredients and baking instructions

## Development

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test src/PieShop.Tests/PieShop.Tests.csproj

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Database Migrations

```bash
# Apply migrations
cd src/PieShop.Api
dotnet ef database update

# Create new migration
dotnet ef migrations add MigrationName -p ../PieShop.Infrastructure
```

### Configuration

Configuration is managed through `appsettings.json` files:

- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides

Key configuration sections:
- `ConnectionStrings` - Database connections
- `ExternalServices` - Service endpoints
- `Logging` - Log levels

## Mock Services

The mock services simulate realistic behavior:

- **Fruit Picker**: 30-60 second processing time
- **Baker**: 15-20 minute baking time (accelerated for testing)
- **Delivery**: 10-30 minute delivery time based on distance

Each service occasionally returns errors to simulate real-world failure scenarios.

## Troubleshooting

### Common Issues

**Port Already in Use:**
```bash
# Check what's using the port
lsof -i :5000
# Kill the process or change port in appsettings.json
```

**Database Connection Failed:**
- Ensure PostgreSQL and MongoDB containers are running
- Check connection strings in appsettings.json
- Verify Docker network configuration

**Mock Services Not Responding:**
- Check Docker logs: `docker-compose logs [service-name]`
- Verify service URLs in appsettings.json match Docker Compose ports
- Restart services: `docker-compose restart`

**CORS Errors in Browser:**
- Verify CORS configuration in `Program.cs`
- Check that UI is running on expected port (3000)

## Testing the System

### End-to-End Flow

1. Open customer UI at http://localhost:3000
2. Select a pie type and fill out the order form
3. Submit the order and note the order ID
4. Watch the order progress through states
5. Check admin dashboard at http://localhost:3000/admin
6. View order details and state history

### Using the API Directly

```bash
# Create an order
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "pieType": "apple",
    "customer": {
      "name": "Test User",
      "email": "test@example.com",
      "phone": "+1-555-0100"
    },
    "deliveryAddress": {
      "street": "456 Oak Ave",
      "city": "Portland",
      "state": "OR",
      "zip": "97201"
    }
  }'

# Check order status
curl http://localhost:5000/api/orders/{orderId}
```

## Project Goals

This system demonstrates:
- Microservice orchestration patterns
- State machine implementation
- External service integration
- Error handling and retry logic
- Polyglot persistence
- Full-stack development with .NET and React

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests to ensure everything works
5. Submit a pull request

## License

MIT License - See LICENSE file for details

## Support

For questions or issues, please open a GitHub issue or contact the development team.
