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

## Repository Structure

### Main Branch (Template - No Code)
```
pie-shop/
├── .specify/                           # Specification-Driven Development
│   ├── memory/
│   │   └── constitution.md            # Project principles and design philosophy
│   ├── features/
│   │   └── 001-pie-shop-orchestration.md  # Complete feature specification
│   ├── IMPLEMENTATION_PROMPT.md        # Generate code in any language
│   ├── INTERVIEW_GUIDE_SHARED_DRIVE_README.md  # Guide for interviewers
│   ├── WORKFLOW_PROPOSAL.md           # Complete workflow documentation
│   └── PROJECT_SUMMARY.md             # Overview and design decisions
├── .github/                           # Issue and PR templates
├── .gitignore                         # Prevents interview guides from being committed
├── AGENTS.md                          # LLM configuration (Coforma standards)
└── README.md                          # This file
```
