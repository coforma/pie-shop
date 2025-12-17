# Feature 001: Pie Shop Order Orchestration System

**Version**: 1.0  
**Last Updated**: 2025-12-17  
**Status**: Specification Complete  
**Priority**: P1 (Critical)  
**Dependencies**: None

## Problem Statement

Technical interviews need realistic but approachable systems that can assess multiple competencies (backend architecture, service integration, code quality, accessibility, security awareness) within a 1-hour timeframe. Generic CRUD applications are too simple, while production codebases are too complex to review quickly.

This system provides a "just right" complexity level: realistic distributed system patterns (external service orchestration, state management, error handling) in an approachable domain (a bakery that uses robots). The deliberately incomplete implementation creates natural discussion points about production readiness.

## Solution Overview

A backend orchestration system that manages the complete lifecycle of pie orders:
1. **Order Receipt**: Customer places order via REST API
2. **Ingredient Sourcing**: Integrate with robot fruit picker service to harvest fresh ingredients
3. **Preparation Scheduling**: Coordinate ingredient prep (washing, peeling, dough making)
4. **Baking Orchestration**: Schedule baking with robot baker service
5. **Delivery Coordination**: Arrange drone delivery to customer location
6. **Status Tracking**: Provide real-time order status through admin dashboard

The system uses a simple state machine for orchestration, with contracts defined for external robot services. Implementations can be mocked or real based on interview needs.

## User Stories

### US-001: Customer Order Placement
**As a** customer  
**I want to** place an order for a pie through a simple web form  
**So that** I can get a freshly baked pie delivered to my location  

**Acceptance Criteria**:
- Customer can select from available pie types (apple, cherry, pumpkin, etc.)
- Customer provides delivery address and contact information
- System validates order and returns order ID immediately
- Customer receives confirmation with estimated delivery time
- Form has intentional accessibility issues for candidates to identify

### US-002: Order Status Tracking
**As a** customer  
**I want to** check the status of my order  
**So that** I know when to expect delivery  

**Acceptance Criteria**:
- Customer can view order status using order ID
- Status display shows current stage (ordered, picking, prepping, baking, delivering, delivered)
- Display includes estimated time remaining
- Error states are clearly communicated (e.g., ingredient shortage, baking failure)

### US-003: Order Workflow Orchestration
**As the** system  
**I need to** coordinate multiple robot services in sequence  
**So that** orders progress from ingredients to delivered pies  

**Acceptance Criteria**:
- System transitions through states: ORDERED → PICKING → PREPPING → BAKING → DELIVERING → COMPLETED
- Each transition triggers appropriate robot service API call
- Failed service calls transition order to ERROR state
- System retries transient failures (with TODOs for improvement)
- State transitions are logged for observability

### US-004: Admin Dashboard Monitoring
**As an** administrator  
**I want to** view all orders and their current status  
**So that** I can monitor operations and troubleshoot issues  

**Acceptance Criteria**:
- Dashboard displays list of all orders with current status
- Admin can filter by status, date, pie type
- Admin can view detailed order history (all state transitions)
- Dashboard refreshes automatically (or has manual refresh)
- Dashboard has intentional accessibility issues for discussion

### US-005: Robot Service Integration
**As the** system  
**I need to** communicate with external robot services  
**So that** physical operations (picking, baking, delivery) can be performed  

**Acceptance Criteria**:
- System sends requests to fruit picker service with ingredient requirements
- System schedules baking with baker service including temperature and duration
- System arranges delivery with drone service including address and time window
- All services return job IDs for tracking
- System handles service timeouts and errors (basic implementation, room for improvement)

## Functional Requirements

### FR-001: Order Management API
The system shall provide a REST API with the following endpoints:
- `POST /api/orders` - Create new order
- `GET /api/orders/{id}` - Retrieve order details and status
- `GET /api/orders` - List all orders (with pagination TODO)
- `PATCH /api/orders/{id}` - Update order (admin only, auth TODO)

**Request/Response Formats**: See API contracts section

### FR-002: State Machine Orchestration
The system shall implement a state machine with these states and transitions:
- **ORDERED**: Initial state when order is created
  - Transition to PICKING when ingredient sourcing begins
- **PICKING**: Fruit picker service is harvesting ingredients
  - Transition to PREPPING when ingredients arrive
  - Transition to ERROR on picker service failure
- **PREPPING**: Ingredients are being prepared (washing, peeling, dough making)
  - Transition to BAKING when prep is complete
  - Transition to ERROR on prep failure
- **BAKING**: Pie is in the oven
  - Transition to DELIVERING when baking completes
  - Transition to ERROR on baking failure
- **DELIVERING**: Drone is en route to customer
  - Transition to COMPLETED on successful delivery
  - Transition to ERROR on delivery failure
- **ERROR**: Unrecoverable failure occurred
  - Manual intervention required (admin feature TODO)
- **COMPLETED**: Order successfully delivered

### FR-003: Fruit Picker Service Integration
The system shall integrate with a robot fruit picker service:
- **Contract**: OpenAPI specification defining picker service endpoints
- **Mock Implementation**: Provided by default, simulates picking with realistic delays (30-60 seconds)
- **Request Data**: Pie type, quantity of fruit needed
- **Response Data**: Job ID, estimated completion time
- **Error Handling**: Timeout after 120 seconds, retry once, then ERROR state

**[SECURITY CHECKPOINT]**: Consider authentication, rate limiting, input validation for this integration

### FR-004: Baker Service Integration
The system shall integrate with a robot baker service:
- **Contract**: OpenAPI specification defining baker service endpoints
- **Mock Implementation**: Provided by default, simulates baking with realistic delays (15-20 minutes)
- **Request Data**: Pie type, baking temperature, baking duration
- **Response Data**: Job ID, oven assignment, completion time
- **Error Handling**: Timeout after 30 minutes, no retry (burnt pie), ERROR state

**[OBSERVABILITY TODO]**: Add distributed tracing to track requests across services

### FR-005: Drone Delivery Service Integration
The system shall integrate with a drone delivery service:
- **Contract**: OpenAPI specification defining delivery service endpoints
- **Mock Implementation**: Provided by default, simulates delivery with realistic delays (10-30 minutes based on distance)
- **Request Data**: Delivery address, package size, delivery window
- **Response Data**: Job ID, drone assignment, ETA
- **Error Handling**: Weather-related failures, address validation errors

**[RESILIENCE TODO]**: Implement circuit breaker pattern for delivery service

### FR-006: Pie Catalog
The system shall maintain a simple catalog of available pies:
- **Pie Types**: Apple, Cherry, Pumpkin, Pecan, Blueberry
- **Attributes per Pie**: Name, baking time (minutes), baking temperature (°F), base ingredients list
- **Storage**: Document store (e.g., MongoDB) for flexible schema
- **Example**: 
  ```json
  {
    "type": "apple",
    "name": "Classic Apple Pie",
    "bakingTime": 45,
    "bakingTemp": 375,
    "ingredients": ["apples", "sugar", "cinnamon", "butter", "flour", "salt"]
  }
  ```

### FR-007: Data Persistence
The system shall use polyglot persistence:
- **Relational Database** (PostgreSQL): Orders, order state history
  - Orders table: id, customer_info, pie_type, delivery_address, current_state, created_at, updated_at
  - State_history table: id, order_id, from_state, to_state, timestamp, notes
- **Document Store** (MongoDB): Pie recipes and catalog
- **Time-Series DB** (optional, InfluxDB): Operational metrics (order duration, service latency)

**[DISCUSSION POINT]**: Why separate databases? What are the trade-offs?

### FR-008: Configuration Management
The system shall support configuration for:
- Robot service endpoints and credentials
- Timeout values for each service
- Retry policies
- Database connection strings

**Implementation Status**:
- ✅ Some configs in environment variables
- ❌ Some configs hard-coded with `// TODO: Move to config` comments
- ❌ Secrets management not implemented (passwords in plain text)

**[SECURITY TODO]**: Implement proper secrets management (Vault, AWS Secrets Manager, etc.)

### FR-009: Error Handling and Logging
The system shall implement basic error handling:
- **Logging**: Structured logs at each state transition (timestamp, order_id, from_state, to_state)
- **Error Capture**: Errors logged with stack traces
- **Retry Logic**: Simple retry with exponential backoff (partially implemented)
- **Dead Letter Queue**: Not implemented (TODO)

**[OBSERVABILITY TODO]**: Add trace IDs for distributed tracing, implement proper alerting

### FR-010: Customer Order Form UI
The system shall provide a web form for customers:
- Fields: Pie type (dropdown), Name, Email, Phone, Delivery address
- Submit button creates order via API
- Success: Shows order ID and estimated delivery time
- Failure: Displays error message

**Intentional Accessibility Issues** (for candidate discussion):
- ❌ Missing form labels (no `<label>` elements)
- ❌ Poor color contrast on submit button (fails WCAG AA)
- ❌ No keyboard navigation support for dropdown
- ❌ Error messages not announced to screen readers
- ❌ No skip navigation link

### FR-011: Admin Dashboard UI
The system shall provide an admin dashboard:
- **Order List View**: Table showing order_id, customer, pie_type, status, created_at
- **Order Detail View**: Full order info with state transition history
- **Filter Controls**: Filter by status, date range, pie type
- **Refresh**: Manual refresh button (auto-refresh TODO)

**Intentional Accessibility Issues** (for candidate discussion):
- ❌ Table missing proper headers and scope attributes
- ❌ Status indicators use color only (no text/icons)
- ❌ Improper heading hierarchy (jumps from h1 to h4)
- ❌ ARIA attributes misused (aria-label on non-interactive elements)
- ❌ Focus indicators removed via CSS

### FR-012: API Documentation
The system shall provide API documentation:
- Swagger/OpenAPI UI available at `/api/docs`
- Interactive testing of endpoints
- Request/response examples for each endpoint
- Error response documentation

## Non-Functional Requirements

### NFR-001: Performance
- Order creation responds within 200ms (database write only)
- Status queries respond within 100ms
- Admin dashboard loads within 2 seconds with up to 1000 orders
- **[TODO]**: Add pagination for large order lists

### NFR-002: Scalability
- System designed to handle 100 concurrent orders (not load tested)
- Stateless API design allows horizontal scaling
- **[DISCUSSION POINT]**: What bottlenecks exist? How would you scale to 10,000 concurrent orders?

### NFR-003: Reliability
- Service failures don't crash the orchestrator
- Orders in ERROR state can be manually recovered (admin feature TODO)
- Database transactions ensure order state consistency
- **[RESILIENCE TODO]**: Implement saga pattern for distributed transactions

### NFR-004: Observability
- All state transitions logged with structured format
- API request/response logging (excluding sensitive data)
- Basic metrics collected (order count by status)
- **[TODO]**: Distributed tracing, performance monitoring, alerting

### NFR-005: Security
- **[TODO]**: API authentication (endpoints currently open)
- **[TODO]**: Authorization (admin vs customer permissions)
- **[PARTIAL]**: Input validation on some endpoints
- **[TODO]**: Rate limiting to prevent abuse
- **[TODO]**: SQL injection prevention (ORM provides some protection)
- **[TODO]**: CORS configuration
- **[HARD-CODED]**: API keys for robot services in config files

### NFR-006: Accessibility
- Target: WCAG 2.1 Level AA compliance
- **Current Status**: Intentional violations for interview assessment
- **[TODO]**: Fix all accessibility issues identified in FR-010 and FR-011

### NFR-007: Maintainability
- Code organized by layer (API, business logic, data access, integrations)
- **[MIXED]**: Some modules well-documented, others sparse
- **[MIXED]**: Some tests comprehensive, others missing
- **[TODO]**: README out of date with current API endpoints

### NFR-008: Deployability
- Docker Compose runs entire system locally
- Single command startup: `docker-compose up`
- **[TODO]**: Health check endpoints for containers
- **[TODO]**: Kubernetes manifests for production deployment
- **[TODO]**: CI/CD pipeline configuration

## API Contracts

### Order Creation
```
POST /api/orders
Content-Type: application/json

Request:
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

Response (201 Created):
{
  "orderId": "ord_abc123",
  "status": "ORDERED",
  "estimatedDelivery": "2025-12-17T18:30:00Z",
  "createdAt": "2025-12-17T16:00:00Z"
}

Error Response (400 Bad Request):
{
  "error": "INVALID_PIE_TYPE",
  "message": "Pie type 'chocolate' is not available",
  "availableTypes": ["apple", "cherry", "pumpkin", "pecan", "blueberry"]
}
```

### Order Status
```
GET /api/orders/{orderId}

Response (200 OK):
{
  "orderId": "ord_abc123",
  "pieType": "apple",
  "customer": { ... },
  "deliveryAddress": { ... },
  "status": "BAKING",
  "createdAt": "2025-12-17T16:00:00Z",
  "updatedAt": "2025-12-17T16:45:00Z",
  "estimatedDelivery": "2025-12-17T18:30:00Z",
  "history": [
    {"state": "ORDERED", "timestamp": "2025-12-17T16:00:00Z"},
    {"state": "PICKING", "timestamp": "2025-12-17T16:05:00Z"},
    {"state": "PREPPING", "timestamp": "2025-12-17T16:35:00Z"},
    {"state": "BAKING", "timestamp": "2025-12-17T16:45:00Z"}
  ]
}

Error Response (404 Not Found):
{
  "error": "ORDER_NOT_FOUND",
  "message": "Order 'ord_abc123' does not exist"
}
```

### Robot Service Contracts
See `.specify/contracts/` directory for full OpenAPI specifications:
- `fruit-picker-service.yaml` - Ingredient sourcing API
- `baker-service.yaml` - Baking orchestration API
- `delivery-service.yaml` - Drone delivery API

## External Service Contracts (Summary)

### Fruit Picker Service
```
POST /api/v1/pick-fruit
Request: { "fruitType": "apple", "quantity": 6, "quality": "premium" }
Response: { "jobId": "pick_xyz789", "estimatedCompletion": "2025-12-17T16:35:00Z" }

GET /api/v1/jobs/{jobId}
Response: { "jobId": "pick_xyz789", "status": "COMPLETED", "fruits": [...] }
```

### Baker Service
```
POST /api/v1/bake
Request: { "pieType": "apple", "temperature": 375, "duration": 45 }
Response: { "jobId": "bake_abc456", "ovenId": "oven-3", "estimatedCompletion": "2025-12-17T17:30:00Z" }

GET /api/v1/jobs/{jobId}
Response: { "jobId": "bake_abc456", "status": "IN_PROGRESS", "progress": 65 }
```

### Delivery Service
```
POST /api/v1/deliveries
Request: { "package": {...}, "destination": {...}, "window": "2025-12-17T18:00:00Z/2025-12-17T19:00:00Z" }
Response: { "deliveryId": "del_def789", "droneId": "drone-12", "eta": "2025-12-17T18:30:00Z" }

GET /api/v1/deliveries/{deliveryId}
Response: { "deliveryId": "del_def789", "status": "IN_TRANSIT", "location": {...} }
```

## Data Models

### Order Entity (Relational)
```
orders
- id: UUID (primary key)
- pie_type: VARCHAR(50)
- customer_name: VARCHAR(255)
- customer_email: VARCHAR(255)
- customer_phone: VARCHAR(20)
- delivery_street: VARCHAR(255)
- delivery_city: VARCHAR(100)
- delivery_state: VARCHAR(2)
- delivery_zip: VARCHAR(10)
- current_state: ENUM (ORDERED, PICKING, PREPPING, BAKING, DELIVERING, COMPLETED, ERROR)
- picker_job_id: VARCHAR(255) [nullable]
- baker_job_id: VARCHAR(255) [nullable]
- delivery_id: VARCHAR(255) [nullable]
- estimated_delivery: TIMESTAMP
- created_at: TIMESTAMP
- updated_at: TIMESTAMP
```

### State History Entity (Relational)
```
state_history
- id: SERIAL (primary key)
- order_id: UUID (foreign key -> orders.id)
- from_state: ENUM [nullable]
- to_state: ENUM
- timestamp: TIMESTAMP
- notes: TEXT [nullable]
- error_message: TEXT [nullable]
```

### Pie Recipe Document (MongoDB)
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
    {"item": "cinnamon", "quantity": 1, "unit": "tsp"},
    {"item": "butter", "quantity": 2, "unit": "tbsp"},
    {"item": "flour", "quantity": 2.5, "unit": "cup"},
    {"item": "salt", "quantity": 1, "unit": "tsp"}
  ],
  "prepSteps": ["wash_fruit", "peel_fruit", "slice_fruit", "make_dough", "assemble"],
  "difficulty": "medium"
}
```

## Testing Strategy

### Unit Tests
- ✅ **State machine transitions**: Well-tested with clear assertions
- ✅ **Order validation**: Tests for valid and invalid inputs
- ⚠️ **Service client classes**: Some tests, but missing error case coverage
- ❌ **Configuration loading**: Not tested (TODO)

### Integration Tests
- ✅ **Happy path**: Order creation → status check → completion
- ⚠️ **Error scenarios**: Service timeouts tested, but retry logic not tested
- ❌ **Database transactions**: Not explicitly tested (relies on ORM)
- ❌ **Concurrent orders**: Not tested (TODO)

### Contract Tests
- ✅ **Mock service responses**: Validate against OpenAPI schemas
- ❌ **Contract evolution**: No versioning strategy tested

### Test Quality Issues (for discussion)
- Some tests are brittle (depend on exact timing)
- Mock setup is verbose and repetitive
- Test names don't always describe behavior clearly
- Missing edge case coverage (empty strings, null values, boundary conditions)

## Deployment Architecture

### Local Development (Docker Compose)
```
Services:
- api: The main orchestration service (port 8080)
- postgres: Order database (port 5432)
- mongodb: Recipe catalog (port 27017)
- fruit-picker-mock: Mock fruit picker service (port 8081)
- baker-mock: Mock baker service (port 8082)
- delivery-mock: Mock delivery service (port 8083)
```

**[TODO]**: Health checks for each service  
**[TODO]**: Proper networking and service discovery  
**[TODO]**: Volume management for data persistence

### Production Deployment (Not Implemented)
**[DISCUSSION POINTS]**:
- How would you deploy this to AWS/Azure/GCP?
- What orchestration platform? (Kubernetes, ECS, etc.)
- How would you handle secrets and configuration?
- What monitoring and alerting would you add?
- How would you handle database migrations?
- What's your disaster recovery strategy?

## Implementation Phases

### Phase 1: Core Orchestration (Week 1)
- [ ] Database schema and migrations
- [ ] REST API skeleton with routing
- [ ] State machine implementation
- [ ] Basic order creation and status endpoints
- [ ] In-memory mock services

### Phase 2: Service Integration (Week 1)
- [ ] OpenAPI contract definitions for robot services
- [ ] Service client implementations
- [ ] Mock service implementations with realistic delays
- [ ] Error handling and retry logic (basic)

### Phase 3: UI Development (Week 2)
- [ ] Customer order form (with intentional accessibility issues)
- [ ] Admin dashboard (with intentional accessibility issues)
- [ ] API documentation UI (Swagger)

### Phase 4: Testing & Polish (Week 2)
- [ ] Unit tests (mixed quality)
- [ ] Integration tests (happy path + some errors)
- [ ] Docker Compose setup
- [ ] Documentation (intentionally incomplete README)

## Success Criteria

### Functional Completeness
- ✅ Orders can be created and tracked through all states
- ✅ State machine handles all transitions correctly
- ✅ Mock services respond with realistic delays and occasional failures
- ✅ UI allows basic order creation and monitoring

### Interview Readiness
- ✅ System runs locally with `docker-compose up`
- ✅ At least 20 identifiable discussion points (TODOs, accessibility issues, security gaps, test improvements)
- ✅ Code is readable and understandable within 15 minutes
- ✅ Multiple valid improvement paths for candidates to suggest

### Assessment Coverage
- ✅ Backend architecture: State management, service integration patterns
- ✅ API design: REST conventions, error handling, documentation
- ✅ Code quality: Structure, naming, documentation, testability
- ✅ Security awareness: Authentication, authorization, input validation, secrets management
- ✅ Accessibility: WCAG violations in UI
- ✅ Testing: Test structure, coverage, quality
- ✅ Observability: Logging, metrics, tracing opportunities
- ✅ Production readiness: Deployment, scaling, reliability concerns

## Open Questions & Discussion Points

These are intentionally left for candidate discussion:

1. **Idempotency**: What happens if an order creation request is sent twice?
2. **Cancellation**: How would you implement order cancellation mid-process?
3. **Service Versioning**: How do you handle robot service API changes?
4. **Data Consistency**: What if database write succeeds but service call fails?
5. **Monitoring**: What metrics would you track? What alerts would you set?
6. **Scaling**: What becomes the bottleneck at 1000 orders/hour? 10,000?
7. **Multi-tenancy**: How would you support multiple pie shops?
8. **Pricing**: Where would you add pricing and payment processing?
9. **Inventory**: How would you track ingredient availability?
10. **SLA Management**: How would you ensure orders complete within promised time?

---

## Document History

**v1.0** (2025-12-17): Initial specification based on interview requirements gathering
