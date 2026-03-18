# Pie Shop - Order Orchestration System

A Python/FastAPI backend that orchestrates pie orders through a fleet of robot services.

**Order Flow**: Customer orders pie → Robot picks fruit → Ingredients prepped → Robot bakes pie → Drone delivers

## Tech Stack

- **API**: Python 3.11, FastAPI, uvicorn
- **Database**: PostgreSQL (orders), MongoDB (pie catalog)
- **Service Clients**: httpx (sync)
- **Testing**: pytest, pytest-asyncio
- **Infrastructure**: Docker, Docker Compose

## Quick Start

### Prerequisites
- Docker and Docker Compose
- Python 3.11+ (for local development)

### Running with Docker Compose

```bash
docker-compose up
```

The API will be available at `http://localhost:3000` and the admin dashboard at `http://localhost:3000/admin`.

API docs are at `http://localhost:3000/api/docs`.

### Running Locally (without Docker)

1. Install dependencies:
   ```bash
   pip install -r requirements.txt
   ```

2. Copy and configure environment:
   ```bash
   cp config/development.env .env
   ```

3. Start mock services:
   ```bash
   uvicorn mocks.fruit_picker_mock:app --port 8081 &
   uvicorn mocks.baker_mock:app --port 8082 &
   uvicorn mocks.delivery_mock:app --port 8083 &
   ```

4. Start the API:
   ```bash
   uvicorn src.main:app --reload --port 8080
   ```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/orders` | Create a new order |
| GET | `/api/orders` | List all orders |
| GET | `/api/orders/{id}` | Get order details |
| POST | `/api/orders/{id}/advance` | Advance order to next state |
| GET | `/api/pies` | List available pie types |
| GET | `/api/admin/stats` | Order statistics |

## Running Tests

```bash
pytest tests/unit/ -v
```

## Project Structure

```
pie-shop/
├── src/                            # Application source
│   ├── main.py                     # FastAPI app, route registration, startup
│   ├── database.py                 # SQLAlchemy engine and session
│   ├── api/
│   │   ├── routes/
│   │   │   ├── orders.py           # Order CRUD endpoints
│   │   │   ├── catalog.py          # Pie catalog endpoints
│   │   │   └── admin.py            # Admin stats endpoint
│   │   ├── middleware/
│   │   │   └── auth.py             # Request logging middleware
│   │   └── schemas.py              # Pydantic request/response models
│   ├── core/
│   │   ├── state_machine.py        # Order state machine and transitions
│   │   └── config.py               # Settings (pydantic-settings)
│   ├── models/
│   │   ├── order.py                # SQLAlchemy Order and StateHistory models
│   │   └── recipe.py               # Pie catalog (in-memory)
│   ├── services/
│   │   ├── order_service.py        # Order business logic and orchestration
│   │   ├── fruit_picker_client.py  # Fruit picker robot service client
│   │   ├── baker_client.py         # Baker robot service client
│   │   └── delivery_client.py      # Drone delivery service client
│   └── utils/
│       └── logger.py               # structlog configuration
├── tests/
│   ├── unit/
│   │   ├── test_state_machine.py   # State machine transition tests
│   │   └── test_order_service.py   # Order service unit tests
│   └── integration/
│       └── test_order_flow.py      # End-to-end order flow tests
├── mocks/                          # Local mock robot services (FastAPI)
│   ├── fruit_picker_mock.py        # Runs on port 8081
│   ├── baker_mock.py               # Runs on port 8082
│   └── delivery_mock.py            # Runs on port 8083
├── ui/
│   ├── templates/
│   │   ├── order_form.html         # Customer order form
│   │   └── admin_dashboard.html    # Order management dashboard
│   └── static/
│       ├── css/styles.css
│       └── js/app.js
├── migrations/
│   └── 001_initial_schema.sql      # PostgreSQL schema
├── config/
│   ├── development.env             # Local development config
│   └── production.env.example      # Production config template
├── docker/
│   ├── Dockerfile                  # API container
│   └── Dockerfile.mock             # Mock services container
├── docker-compose.yml
├── requirements.txt
└── pytest.ini
```