# Implementation Generation Prompt

Use this prompt with the **modern-architect-engineer** agent to generate a language-specific implementation of the Pie Shop system.

---

## Prompt Template

```
I need you to generate a complete implementation of the Pie Shop Order Orchestration System based on the specification in `.specify/features/001-pie-shop-orchestration.md` and following the principles in `.specify/memory/constitution.md`.

**IMPORTANT: Before you begin, ask me these questions:**

1. **What programming language and framework should I use?**
   - Examples: Python/FastAPI, Node.js/Express, Java/Spring Boot, Go/Gin, etc.

2. **What role is this interview for?**
   - Backend Engineer
   - Full Stack Engineer  
   - DevOps/SRE Engineer
   - Security Engineer
   - Accessibility Specialist
   - Other: [specify]

3. **What is the candidate's experience level?**
   - Junior (0-2 years)
   - Mid-level (2-5 years)
   - Senior (5-8 years)
   - Staff/Principal (8+ years)

**After I answer, generate the following:**

## 1. Implementation Plan

Create a detailed implementation plan in `specs/001-pie-shop-orchestration/plan.md` including:
- Technology stack with specific versions (check latest stable versions)
- Project structure (exact directory layout and file paths)
- Database schema (SQL migrations for PostgreSQL, MongoDB collections)
- External service mock implementations
- Docker Compose configuration
- Testing strategy

## 2. Complete Working Implementation

Generate production-like code with intentional technical debt:

### Core Requirements:
- ✅ **Fully functional**: All endpoints work, state machine executes, mocks respond
- ✅ **Production-like structure**: Proper layering (API, business logic, data access)
- ✅ **Working but imperfect**: Code works but has technical debt for discussion

### Intentional Technical Debt to Include:

> **CRITICAL: This section describes what CODE ISSUES to create, NOT what COMMENTS to write.**
> 
> The items below are things the code should DO (or fail to do). They are NOT comments to add.
> For example, "No authentication on API endpoints" means the code should lack authentication - 
> do NOT add a comment saying "No authentication here!" The absence of the feature IS the issue.
> 
> See "Important Guidelines" section #9 for comment guidelines.

#### Security Issues (Critical for Discussion)
- ❌ No authentication on API endpoints (add TODO comment)
- ❌ No authorization/RBAC (add TODO comment)
- ❌ Hard-coded API keys and secrets in config files
- ❌ Some endpoints missing input validation
- ❌ No rate limiting
- ❌ SQL injection risk if not using ORM properly (show example)
- Add comments like: `# TODO: Add authentication middleware`

#### Observability Gaps
- ❌ No distributed tracing (no correlation IDs)
- ❌ Inconsistent log levels (everything at INFO)
- ❌ No circuit breaker for external services
- ❌ Basic retry logic without exponential backoff
- ❌ No dead letter queue for failed orders
- ❌ Missing health check endpoints
- Add comments like: `# TODO: Implement circuit breaker pattern here`

#### Code Quality Issues
- ❌ Some functions are too long (>50 lines) with multiple responsibilities
- ❌ Configuration scattered (some env vars, some hard-coded)
- ❌ Inconsistent error handling (some return errors, some throw exceptions)
- ❌ Magic numbers without constants
- ❌ Verbose, repetitive code in some places
- No comments explaining why, just mechanical TODOs

#### Testing Gaps
- ✅ State machine transitions: well-tested
- ✅ Happy path integration test: complete
- ⚠️ Error cases: only some covered
- ❌ Edge cases: missing (null values, empty strings, boundary conditions)
- ❌ Concurrent operations: not tested
- ❌ Mock contract tests: not validated against real service specs
- Some tests are brittle (depend on exact timing)

#### Accessibility Issues (UI)
For the customer order form HTML:
- ❌ Missing `<label>` elements for form fields
- ❌ Submit button with poor color contrast (e.g., #999 on #AAA)
- ❌ Dropdown with no keyboard navigation support
- ❌ Error messages not announced to screen readers
- ❌ No skip navigation link

For the admin dashboard HTML:
- ❌ Table missing `<th>` and scope attributes
- ❌ Status indicators using color only (no text/icons)
- ❌ Improper heading hierarchy (h1 → h4, skipping h2/h3)
- ❌ ARIA attributes misused (e.g., aria-label on non-interactive elements)
- ❌ Focus indicators removed via CSS (`:focus { outline: none; }`)

#### Deployment/Operations Gaps
- ✅ Docker Compose works locally
- ❌ No health/readiness/liveness endpoints
- ❌ No graceful shutdown handling
- ❌ Missing container resource limits
- ❌ No Kubernetes manifests
- ❌ No CI/CD pipeline configuration
- Add comments like: `# TODO: Add health check endpoint for k8s probes`

#### Documentation Issues
- ⚠️ README is functional but outdated (lists wrong port numbers, missing new endpoints)
- ❌ No architecture diagrams
- ❌ Some inline comments are obvious ("increment counter")
- ❌ Some complex logic has no comments at all
- ❌ No troubleshooting guide

## 3. File Structure

Generate a realistic project structure. Example for Python:

```
pie-shop/
├── src/
│   ├── api/
│   │   ├── routes/
│   │   │   ├── orders.py          # Order management endpoints
│   │   │   └── health.py          # Health checks
│   │   └── middleware/
│   │       └── auth.py             # Authentication
│   ├── core/
│   │   ├── state_machine.py       # State machine logic
│   │   └── config.py               # Configuration
│   ├── services/
│   │   ├── order_service.py       # Business logic
│   │   ├── fruit_picker_client.py # External service client
│   │   ├── baker_client.py        # External service client
│   │   └── delivery_client.py     # External service client
│   ├── models/
│   │   ├── order.py                # SQLAlchemy models
│   │   └── recipe.py               # MongoDB documents
│   └── utils/
│       └── logger.py               # Logging setup
├── tests/
│   ├── unit/
│   │   ├── test_state_machine.py  # State machine tests
│   │   └── test_order_service.py  # Order service tests
│   └── integration/
│       └── test_order_flow.py      # Integration tests
├── mocks/
│   ├── fruit_picker_mock.py       # Mock service
│   ├── baker_mock.py
│   └── delivery_mock.py
├── ui/
│   ├── templates/
│   │   ├── order_form.html        # Customer order form
│   │   └── admin_dashboard.html   # Admin view
│   └── static/
│       ├── css/
│       │   └── styles.css          # Styles
│       └── js/
│           └── app.js               # Frontend logic
├── docker/
│   ├── Dockerfile                  # Container build
│   └── docker-compose.yml          # Local setup
├── migrations/
│   └── 001_initial_schema.sql      # Database setup
├── config/
│   ├── development.env             # Development config
│   └── production.env.example      # Production template
├── README.md                       # Documentation
├── requirements.txt / package.json # Dependencies
└── .gitignore
```

## 4. Key Files with Intentional Issues

Ensure these specific scenarios exist in the code:

### State Machine (Well-Implemented Reference)
- Clean, testable code
- Good separation of concerns
- Well-tested
- **Purpose**: Show what good code looks like for comparison

### Service Clients (Multiple Issues)
- No circuit breaker (just try/catch)
- Simple retry without exponential backoff
- No timeout configuration
- Error handling inconsistent
- **Purpose**: Discuss resilience patterns

### Order Service (Code Quality Issues)
- One method is 80+ lines (does too much)
- Some hard-coded values (timeouts, URLs)
- Mix of sync and async calls
- **Purpose**: Discuss refactoring and SOLID principles

### API Routes (Security Issues)
- No authentication decorator
- No input validation on some endpoints
- SQL injection risk if raw queries used
- **Purpose**: Discuss API security

### Configuration (Anti-pattern)
- Database password in plain text
- API keys committed to repo
- Mix of env vars and hard-coded values
- **Purpose**: Discuss secrets management

### Tests (Mixed Quality)
- State machine: excellent (clear, isolated, fast)
- Service clients: brittle (depend on timing)
- Integration: only happy path
- **Purpose**: Discuss testing strategy

### UI (Accessibility Issues)
- All the violations listed above
- **Purpose**: Assess accessibility awareness

## 5. Mock Services

Generate three mock services that:
- Respond with realistic delays (30-60s for picking, 15-20min for baking, 10-30min for delivery)
- Occasionally fail (10% failure rate) to test error handling
- Return proper job IDs and status
- Follow the OpenAPI contracts defined in spec

## 6. Documentation

Generate:
- **README.md**: Functional but with issues:
  - Quick start guide works
  - API documentation incomplete (lists 3 endpoints, but 5 exist)
  - Port numbers wrong in examples (says 8080, actually 3000)
  - No troubleshooting section
  - No architecture diagram
- **API docs**: OpenAPI/Swagger spec (accurate)
- **Inline comments**: Mix of good, obvious, and missing

## 7. Docker Setup

Generate Docker Compose that:
- ✅ Runs all services (API, PostgreSQL, MongoDB, mock services)
- ✅ Works with `docker-compose up`
- ❌ Missing health checks
- ❌ No resource limits (memory, CPU)
- ❌ No restart policies
- ❌ Logs go to stdout (no log aggregation)

## 8. Generate Interview Guide

After generating the implementation, create a detailed interview guide:

**File**: `INTERVIEW_GUIDE_[LANGUAGE].md` (e.g., `INTERVIEW_GUIDE_PYTHON.md`)

The guide should include:

### Structure (4 Layers):

#### Layer 1: Orientation (5 minutes)
- File structure overview
- How to run locally (even though we won't)
- Key files list with one-line descriptions
- Architecture diagram (ASCII or reference to missing diagram)

#### Layer 2: Guided Tour (15 minutes)
- 5-7 critical discussion points
- Each with: file path, line number range, what to look for
- Organized as checkpoints: [✓] shows progression
- Interviewer guides candidate: "Let's start by looking at..."

#### Layer 3: Candidate Exploration (30 minutes)
- Candidate drives: "What catches your attention?"
- Backup prompts if they miss critical areas
- Format: "If candidate hasn't mentioned X by 35min mark, prompt: 'Have you looked at [file]?'"
- Organized by topic with file references

#### Layer 4: Wrap-up (10 minutes)
- Questions for candidate about missed areas
- "If you had a week to improve this, what would you prioritize?"
- Final assessment areas to probe

### Content Requirements:

For each discussion point, include:
- **File Path**: Exact path (e.g., `src/services/order_service.py`)
- **Line Numbers**: Range or specific lines (e.g., `lines 45-67` or `line 89`)
- **What's Here**: Brief description of the code
- **Issue/Discussion**: What's wrong or worth discussing
- **Good Response Indicators**: What strong candidates will say
- **Red Flags**: What weak candidates might miss or say
- **Follow-up Probes**: Questions to ask based on their response
- **Markers**: [CRITICAL], [ROLE: Backend], [LEVEL: Senior+], [TOPIC: Security]

### Role-Based Emphasis:

Based on the role I specified, mark priorities:
- **Backend Engineer**: Architecture, API design, testing, database
- **Full Stack**: Balance of backend + UI + accessibility
- **DevOps/SRE**: Observability, deployment, scaling, reliability
- **Security Engineer**: Auth, validation, secrets, threat modeling
- **Accessibility Specialist**: WCAG compliance, ARIA, keyboard nav

### Example Format:

```markdown
## Layer 2: Guided Tour (15 minutes)

### Checkpoint 1: State Machine Overview [CRITICAL]
**File**: `src/core/state_machine.py`  
**Lines**: 1-89 (entire file)  
**Time**: 3 minutes

**Interviewer Prompt**: "Let's start by looking at the state machine. Can you open `src/core/state_machine.py` and walk me through what you see?"

**What's Here**: Clean state machine implementation with clear transitions

**Discussion Points**:
- This is actually well-implemented - what do they notice?
- How would they test this?
- What patterns do they recognize?

**Good Response**: 
- Notices clean structure, single responsibility
- Mentions it's testable/well-separated
- Might suggest event-driven alternative but acknowledges trade-offs

**Red Flags**:
- Can't explain what the code does
- Suggests complete rewrite without reason
- Doesn't recognize this as reference implementation

**Follow-up**: "What would need to change if we wanted to support order cancellation?"

---

### Checkpoint 2: Service Client - No Circuit Breaker [CRITICAL] [TOPIC: Resilience]
**File**: `src/services/fruit_picker_client.py`  
**Lines**: 45-67  
**Time**: 4 minutes

**Interviewer Prompt**: "Now let's look at how we call external services. Can you open `fruit_picker_client.py` and go to the `pick_fruit` method around line 45?"

**What's Here**: 
```python
def pick_fruit(self, fruit_type: str, quantity: int):
    try:
        response = requests.post(
            f"{self.base_url}/pick-fruit",
            json={"fruitType": fruit_type, "quantity": quantity},
            timeout=10
        )
        response.raise_for_status()
        return response.json()
    except requests.exceptions.Timeout:
        # TODO: Implement retry logic
        raise
    except requests.exceptions.RequestException as e:
        logger.error(f"Failed to pick fruit: {e}")
        raise
```

**Issues**:
- No circuit breaker (will hammer failing service)
- No retry logic (just TODO)
- Hard-coded timeout
- No exponential backoff
- Doesn't distinguish transient vs permanent failures

**Good Response**:
- Identifies missing circuit breaker pattern
- Mentions retry with exponential backoff
- Discusses bulkhead pattern
- Asks about service SLAs
- [Senior+] Suggests queue-based integration

**Red Flags**:
- Doesn't see any issues
- "Just retry forever"
- Doesn't know what circuit breaker is

**Follow-up**: "What happens if the fruit picker service is down for 10 minutes and we have 100 orders?"

---

[Continue for all checkpoints...]
```

## Layer 3: Candidate Exploration (30 minutes)

Organized by topic with backup prompts:

```markdown
### Topic: Security [CRITICAL for all roles]

**Expected Discovery**: Candidate should notice lack of authentication

**If they mention it**: 
- Probe: "Where would you add authentication?"
- Probe: "What about authorization - customer vs admin?"
- Show: `src/api/routes/orders.py`, lines 23-45 (no auth decorator)

**If they don't mention by 20min mark**:
- Prompt: "Have you looked at the API routes? Anything concerning?"
- If still missed: "Let's look at `src/api/routes/orders.py` line 23. Who can access this endpoint?"

**Files to Reference**:
- `src/api/routes/orders.py`: No auth decorators (lines 23, 45, 67)
- `src/api/middleware/auth.py`: Empty TODO file
- `config/development.env`: Hard-coded secrets (lines 5-8)

**Discussion Depth by Level**:
- [Junior]: Should notice missing auth
- [Mid]: Should discuss JWT vs sessions, API keys
- [Senior]: Should discuss secrets management, rate limiting, RBAC
- [Staff+]: Should discuss threat modeling, compliance (HIPAA, etc.)

---

[Continue for all topics...]
```

## Important Guidelines:

1. **Constitutional Alignment**: Reference `.specify/memory/constitution.md` principles throughout
2. **Realistic Debt**: Code should work but have realistic technical debt, not contrived issues
3. **Language Idioms**: Use idiomatic code for the chosen language
4. **Latest Versions**: Check and use latest stable versions of all dependencies
5. **Runnable (but we won't)**: Code should theoretically run, but optimized for reading
6. **Interviewer Prep**: Guide should help interviewer prep in 15 minutes
7. **Natural TODOs**: Comments should look like real developer TODOs, not interview plants
8. **Balance**: Some code good (state machine), some bad (service clients), most in-between
9. **NO INTERVIEW HINTS IN CANDIDATE-FACING FILES**:
   - README.md should be professional and straightforward (no "intentional issues" mentions)
   - Make it look like a real production codebase a developer would create
   - Issues should be discovered through code review, not telegraphed in comments

   **Comment Guidelines - What TO write:**
   - `// TODO: Add authentication middleware` - vague future work
   - `// TODO: Implement circuit breaker pattern` - names a pattern without explaining why
   - `// TODO: Add pagination` - simple feature note
   - `// TODO: Move to configuration` - neutral improvement
   - `// TODO: Add error handling` - generic without specifics
   - `// Basic retry logic` - neutral description
   - No comment at all (many real codebases have missing code without comments)

   **Comment Guidelines - What NOT to write (these are interview hints):**
   - `// SECURITY ISSUE: ...` or `// ACCESSIBILITY ISSUE: ...` - labels the problem category
   - `// ISSUES: No X, no Y, missing Z` - lists multiple problems in one comment
   - `// No authentication check - anyone can see all orders!` - explains the security impact
   - `// Should be exponential!` or `// should be in configuration!` - tells them what's wrong
   - `// Doesn't categorize errors (400 vs 500 vs 503)` - explains what's missing
   - `// This will fail with many orders` - predicts the failure mode
   - `// Hard-coded URL - should be in configuration!` - both identifies AND solves
   - `// TODO: Missing tests for: [list]` - explicitly lists what's untested
   - `// More missing labels` - acknowledges repeated issues
   - `/// ISSUES: ...` in XML doc comments - especially obvious in doc headers
   - `// No validation here - should check for null/empty values!` - explains what's missing
   - `// Hard-coded!` or `// Magic number` with exclamation - draws attention to issues
   - `// No error handling at all here!` - explicitly calls out missing code
   - `/// This is an example of incomplete tests` - meta-commentary about code quality
   - `// Basic error handling - doesn't distinguish between X and Y` - explains the flaw
   - `// Just rethrow - no recovery attempt` - critiques the implementation

   **Transformation Examples:**
   | Bad (Interview Hint) | Good (Natural TODO) |
   |----------------------|---------------------|
   | `// ACCESSIBILITY ISSUE: Input fields missing label elements` | No comment, or `// TODO: Improve form accessibility` |
   | `// ISSUES: No error categorization, no bulkhead pattern` | No comment (let code speak for itself) |
   | `// Hard-coded base URL - should be in configuration!` | `// TODO: Move to config` |
   | `// Simple linear backoff - should be exponential!` | No comment, or `// Basic retry logic` |
   | `// TODO: This will fail with many orders!` | `// TODO: Add pagination` |
   | `// No authentication check - anyone can see all orders!` | No comment (the missing auth IS the issue) |
   | `// Doesn't distinguish network errors from service errors` | No comment |
   | `// TODO: Missing tests for: [bullet list]` | `// TODO: Add more test coverage` |
   | `// No validation here - should check for null/empty values!` | No comment (let code speak for itself) |
   | `/// This is an example of incomplete tests` | No comment (test coverage speaks for itself) |
   | `// Just rethrow - no recovery attempt` | No comment |

   **The Golden Rule**: A comment should never tell a candidate what's wrong or how to fix it. If removing the comment would make the issue harder to find, the comment is an interview hint.
10. **Interview Guide is Separate**: All discussion points, issue locations, and line numbers go in INTERVIEW_GUIDE_[LANGUAGE].md (which is gitignored)

## Deliverables:

1. ✅ Complete implementation in chosen language
2. ✅ Docker Compose setup
3. ✅ README (intentionally imperfect)
4. ✅ Mock services
5. ✅ Tests (mixed quality)
6. ✅ UI with accessibility issues
7. ✅ Language-specific interview guide with file paths and line numbers
8. ✅ Quick reference card (1-page cheat sheet for interviewers)

Begin by asking me the three questions above, then proceed with implementation and guide generation.
```

---

## Usage Instructions

1. **Copy the prompt above** (everything in the code block)
2. **Start a conversation with the modern-architect-engineer agent**
3. **Paste the prompt**
4. **Answer the three questions** about language, role, and experience level
5. **Agent will generate**: Complete implementation + Interview guide with specific file/line references
6. **Review and customize** as needed for your specific interview

The generated interview guide will be tailored to your language choice and have exact file paths and line numbers for the layered interview approach.
