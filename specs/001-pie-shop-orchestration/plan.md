# Implementation Plan: Pie Shop Orchestration System
# C# / React Full Stack - Senior Level

**Language**: C# 12 (.NET 8.0)  
**Frontend**: React 18.2 with TypeScript 5.3  
**Target Role**: Senior Full Stack Engineer  
**Generated**: 2025-12-17  
**Complexity**: Production-like with intentional technical debt

## Technology Stack

### Backend
- **Runtime**: .NET 8.0 (LTS)
- **Framework**: ASP.NET Core 8.0 (Web API)
- **ORM**: Entity Framework Core 8.0
- **Database**: PostgreSQL 16 (orders, state history)
- **Document Store**: MongoDB 7.0 (recipe catalog)
- **HTTP Client**: HttpClient with IHttpClientFactory
- **Testing**: xUnit 2.6, Moq 4.20, FluentAssertions 6.12
- **API Documentation**: Swashbuckle.AspNetCore 6.5 (Swagger/OpenAPI)
- **Logging**: Serilog 3.1 with structured logging

### Frontend
- **Framework**: React 18.2.0
- **Language**: TypeScript 5.3
- **Build Tool**: Vite 5.0
- **HTTP Client**: Axios 1.6
- **Routing**: React Router 6.20
- **State Management**: React Context API (intentionally simple)
- **Styling**: CSS Modules (with accessibility issues)
- **Testing**: Vitest 1.0, React Testing Library 14.1

### Infrastructure
- **Container Runtime**: Docker 24.0+
- **Orchestration**: Docker Compose 2.23
- **Mock Services**: ASP.NET Core minimal APIs (.NET 8.0)

## Project Structure

```
pie-shop/
├── src/
│   ├── PieShop.Api/                    # Main API project
│   │   ├── Controllers/
│   │   │   ├── OrdersController.cs     # Order management endpoints (no auth!)
│   │   │   └── HealthController.cs     # Health checks (TODO: implement properly)
│   │   ├── Middleware/
│   │   │   ├── ErrorHandlingMiddleware.cs  # Basic error handling
│   │   │   └── AuthMiddleware.cs       # TODO: Not implemented
│   │   ├── Program.cs                  # App configuration (secrets hard-coded!)
│   │   ├── appsettings.json           # Config with hard-coded secrets
│   │   └── PieShop.Api.csproj
│   │
│   ├── PieShop.Core/                   # Business logic layer
│   │   ├── StateMachine/
│   │   │   ├── OrderStateMachine.cs    # Well-implemented state machine
│   │   │   ├── OrderState.cs           # State enum
│   │   │   └── StateTransition.cs      # Transition model
│   │   ├── Services/
│   │   │   ├── OrderService.cs         # Business logic (some methods too long!)
│   │   │   └── RecipeService.cs        # Recipe catalog queries
│   │   ├── Models/
│   │   │   ├── Order.cs                # Domain model
│   │   │   ├── Customer.cs             # Value object
│   │   │   ├── DeliveryAddress.cs      # Value object
│   │   │   └── Recipe.cs               # Document model
│   │   └── PieShop.Core.csproj
│   │
│   ├── PieShop.Infrastructure/         # External integrations
│   │   ├── Data/
│   │   │   ├── PieShopDbContext.cs     # EF Core context
│   │   │   ├── MongoDbContext.cs       # MongoDB connection
│   │   │   └── Migrations/             # EF migrations
│   │   ├── ExternalServices/
│   │   │   ├── FruitPickerClient.cs    # No circuit breaker!
│   │   │   ├── BakerClient.cs          # No timeout handling!
│   │   │   ├── DeliveryClient.cs       # Basic retry only!
│   │   │   └── IExternalServiceClient.cs  # Interface
│   │   ├── Repositories/
│   │   │   ├── OrderRepository.cs      # Data access
│   │   │   └── RecipeRepository.cs     # MongoDB queries
│   │   └── PieShop.Infrastructure.csproj
│   │
│   └── PieShop.Tests/                  # Test project
│       ├── Unit/
│       │   ├── StateMachineTests.cs    # Well-written tests ✓
│       │   ├── OrderServiceTests.cs    # Some coverage, missing edge cases
│       │   └── RecipeServiceTests.cs   # Basic tests
│       ├── Integration/
│       │   └── OrderFlowTests.cs       # Happy path only
│       └── PieShop.Tests.csproj
│
├── mocks/
│   ├── FruitPickerMock/               # Mock fruit picker service
│   │   ├── Program.cs                 # Minimal API with realistic delays
│   │   └── FruitPickerMock.csproj
│   ├── BakerMock/                     # Mock baker service
│   │   ├── Program.cs                 # Realistic baking delays (15-20 min)
│   │   └── BakerMock.csproj
│   └── DeliveryMock/                  # Mock delivery service
│       ├── Program.cs                 # Distance-based delays
│       └── DeliveryMock.csproj
│
├── ui/
│   ├── public/
│   │   └── index.html
│   ├── src/
│   │   ├── components/
│   │   │   ├── OrderForm.tsx          # With accessibility issues!
│   │   │   ├── AdminDashboard.tsx     # With accessibility issues!
│   │   │   └── OrderStatus.tsx        # Basic component
│   │   ├── services/
│   │   │   └── apiClient.ts           # Axios wrapper
│   │   ├── styles/
│   │   │   ├── OrderForm.module.css   # Poor contrast, no focus indicators
│   │   │   └── AdminDashboard.module.css  # Color-only status
│   │   ├── App.tsx
│   │   ├── main.tsx
│   │   └── vite-env.d.ts
│   ├── package.json
│   ├── tsconfig.json
│   └── vite.config.ts
│
├── docker/
│   ├── Dockerfile.api                 # Works but missing health checks
│   ├── Dockerfile.mocks               # For mock services
│   ├── Dockerfile.ui                  # React production build
│   └── docker-compose.yml             # No resource limits!
│
├── migrations/
│   └── 001_initial_schema.sql         # PostgreSQL schema
│
├── config/
│   ├── development.json               # Hard-coded secrets here!
│   └── production.json.example        # Template (TODO: use Key Vault)
│
├── PieShop.sln                        # Solution file
├── README.md                          # Outdated documentation
├── .gitignore
└── global.json                        # .NET SDK version pinning

```

## Architecture Decisions

### Layered Architecture
**Decision**: Clean architecture with API → Core → Infrastructure layers  
**Rationale**: Clear separation of concerns, testable business logic  
**Intentional Debt**: Some dependencies leak across layers, not perfectly clean

### State Management Pattern
**Decision**: Explicit state machine in Core layer  
**Rationale**: Deterministic, easy to test, clear transitions  
**Good Example**: `OrderStateMachine.cs` is well-implemented for reference  
**Discussion Point**: Could be event-driven or saga pattern instead

### External Service Communication
**Decision**: HttpClient with IHttpClientFactory  
**Rationale**: Built-in .NET best practice for HTTP clients  
**Intentional Gaps**: 
- No circuit breaker (no Polly policies)
- Basic retry without exponential backoff
- No timeout configuration
- Inconsistent error handling

### Configuration Management
**Decision**: Mix of appsettings.json and hard-coded values  
**Intentional Issues**:
- Database passwords in plain text in appsettings.json
- API keys committed to repo
- Some timeouts hard-coded in service clients  
**Discussion**: Azure Key Vault, AWS Secrets Manager, environment variables

### Data Access
**Decision**: Entity Framework Core for PostgreSQL, MongoDB.Driver for recipes  
**Rationale**: 
- EF Core provides LINQ, migrations, change tracking for relational data
- MongoDB for flexible recipe schema  
**Trade-offs**: Polyglot persistence adds complexity but shows understanding

### API Design
**Decision**: RESTful API with Swagger documentation  
**Intentional Issues**:
- No authentication middleware
- Missing input validation on some endpoints
- No rate limiting
- CORS not configured properly  
**Discussion**: JWT, OAuth2, API Gateway patterns

## Database Schema

### PostgreSQL (Relational)

**Orders Table**:
```sql
CREATE TABLE orders (
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

CREATE INDEX idx_orders_state ON orders(current_state);
CREATE INDEX idx_orders_created ON orders(created_at DESC);
```

**State_History Table**:
```sql
CREATE TABLE state_history (
    id SERIAL PRIMARY KEY,
    order_id UUID NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
    from_state VARCHAR(20),
    to_state VARCHAR(20) NOT NULL,
    timestamp TIMESTAMP NOT NULL DEFAULT NOW(),
    notes TEXT,
    error_message TEXT
);

CREATE INDEX idx_history_order ON state_history(order_id, timestamp DESC);
```

### MongoDB (Document Store)

**Recipes Collection**:
```json
{
  "_id": "apple",
  "name": "Classic Apple Pie",
  "description": "Traditional American apple pie with cinnamon",
  "bakingTime": 45,
  "bakingTemp": 375,
  "ingredients": [
    {"item": "apples", "quantity": 6, "unit": "whole"},
    {"item": "sugar", "quantity": 0.75, "unit": "cup"},
    {"item": "cinnamon", "quantity": 1, "unit": "tsp"}
  ],
  "prepSteps": ["wash_fruit", "peel_fruit", "slice_fruit", "make_dough", "assemble"],
  "difficulty": "medium"
}
```

## External Service Mock Implementations

### Fruit Picker Mock (Port 8081)
- **Endpoint**: `POST /api/v1/pick-fruit`
- **Behavior**: Returns job ID, simulates 30-60 second delay
- **Failure Rate**: 10% random failures to test error handling
- **Response**: `{ "jobId": "pick_xyz", "estimatedCompletion": "..." }`

### Baker Mock (Port 8082)
- **Endpoint**: `POST /api/v1/bake`
- **Behavior**: Returns job ID, simulates 15-20 minute delay (accelerated for testing)
- **Failure Rate**: 5% random failures (oven malfunction)
- **Response**: `{ "jobId": "bake_abc", "ovenId": "oven-3", "estimatedCompletion": "..." }`

### Delivery Mock (Port 8083)
- **Endpoint**: `POST /api/v1/deliveries`
- **Behavior**: Calculates ETA based on distance, simulates 10-30 min delay
- **Failure Rate**: 8% failures (weather, address issues)
- **Response**: `{ "deliveryId": "del_def", "droneId": "drone-12", "eta": "..." }`

## Testing Strategy

### Unit Tests (xUnit + Moq + FluentAssertions)
- ✅ **State Machine**: Comprehensive tests, all transitions covered
- ✅ **Order Validation**: Tests for valid and invalid inputs
- ⚠️ **Service Clients**: Some tests, missing timeout and error cases
- ❌ **Configuration Loading**: Not tested

### Integration Tests
- ✅ **Happy Path**: Full order flow from creation to delivery
- ⚠️ **Error Scenarios**: Service timeouts tested, retry logic not tested
- ❌ **Concurrent Orders**: Not tested
- ❌ **Database Transactions**: Not explicitly tested

### Test Quality Issues (Intentional)
- Some tests use magic numbers without explanation
- Mock setup is repetitive (could use builders)
- Some test names are unclear ("Test1", "Test2")
- Missing edge cases: null values, empty strings, boundary conditions

## Docker Compose Configuration

```yaml
services:
  api:
    build: ./docker/Dockerfile.api
    ports: ["8080:8080"]
    environment:
      - ConnectionStrings__Postgres=...  # Hard-coded password!
      - ConnectionStrings__MongoDB=...
      - ExternalServices__FruitPicker=http://fruit-picker:8081
    # TODO: Add health checks
    # TODO: Add resource limits
    # TODO: Add restart policy
  
  postgres:
    image: postgres:16-alpine
    ports: ["5432:5432"]
    environment:
      - POSTGRES_PASSWORD=password123  # Insecure!
  
  mongodb:
    image: mongo:7.0
    ports: ["27017:27017"]
  
  fruit-picker:
    build: ./docker/Dockerfile.mocks
    ports: ["8081:8081"]
  
  baker:
    build: ./docker/Dockerfile.mocks
    ports: ["8082:8082"]
  
  delivery:
    build: ./docker/Dockerfile.mocks
    ports: ["8083:8083"]
  
  ui:
    build: ./docker/Dockerfile.ui
    ports: ["3000:80"]
```

## Key Discussion Points for Interview

### Security (CRITICAL - All Levels)
1. **No Authentication** (`OrdersController.cs`): All endpoints are open
2. **Hard-coded Secrets** (`appsettings.json`): Database passwords committed
3. **No Input Validation** (Some controller actions): SQL injection risk
4. **No Authorization**: Admin endpoints accessible to anyone
5. **CORS Misconfiguration** (`Program.cs`): AllowAny origin

### Resilience Patterns (Senior+ Focus)
1. **No Circuit Breaker** (`FruitPickerClient.cs`): Will hammer failing services
2. **Basic Retry** (`BakerClient.cs`): No exponential backoff
3. **No Timeout Configuration** (`DeliveryClient.cs`): Defaults only
4. **No Bulkhead Pattern**: All services share same thread pool
5. **No Dead Letter Queue**: Failed orders lost

### Code Quality (All Levels)
1. **Long Method** (`OrderService.cs`, ~100 lines): ProcessOrderAsync does too much
2. **Magic Numbers** (Various files): Timeout values hard-coded
3. **Inconsistent Error Handling**: Some throw, some return null
4. **Repetitive Code** (Service clients): Similar patterns not extracted

### Accessibility (Full Stack Focus)
1. **Missing Labels** (`OrderForm.tsx`): Form inputs not associated
2. **Poor Contrast** (`OrderForm.module.css`): Button fails WCAG AA
3. **No Keyboard Nav** (`AdminDashboard.tsx`): Table not keyboard accessible
4. **Color-Only Status** (`AdminDashboard.tsx`): Status indicators
5. **Focus Indicators Removed** (CSS): `:focus { outline: none; }`

### Testing (Mid-Senior+ Focus)
1. **Brittle Tests** (`OrderFlowTests.cs`): Depend on exact timing
2. **Missing Edge Cases**: Null values, empty strings not tested
3. **No Concurrent Testing**: Race conditions not covered
4. **Mock Contracts**: Not validated against actual service specs

### Observability (Senior+ Focus)
1. **No Correlation IDs**: Can't trace requests across services
2. **Inconsistent Log Levels**: Everything at Information
3. **No Metrics Collection**: No Prometheus, App Insights
4. **No Distributed Tracing**: No OpenTelemetry

### Architecture (Senior+ Focus)
1. **Synchronous Orchestration**: Discuss event-driven alternatives
2. **Polyglot Persistence**: Why separate databases? Trade-offs?
3. **State Machine vs Saga**: When would you use saga pattern?
4. **Monolith vs Microservices**: Current approach pros/cons

## Constitutional Alignment

This implementation follows the project constitution:

✅ **Deliberately Incomplete**: TODOs and gaps for production discussion  
✅ **Realistic Complexity**: Real-world patterns without overwhelming scope  
✅ **Accessibility Focus**: Intentional WCAG violations for assessment  
✅ **Production Concerns**: Security, observability, resilience as checkpoints  
✅ **Interview-Friendly**: Runs locally with Docker Compose  
✅ **Multiple Assessment Dimensions**: Backend, frontend, security, testing, accessibility

## Next Steps

1. ✅ Generate all C# backend code with intentional issues
2. ✅ Create React frontend with accessibility problems
3. ✅ Build mock services with realistic delays
4. ✅ Write tests with mixed quality
5. ✅ Create Docker Compose setup with gaps
6. ✅ Generate README with outdated information
7. ✅ Create interview guide with specific file/line references
