# Pie Shop System - Specification Complete ✅

## What We Built

A comprehensive, language-agnostic specification for an interview assessment project featuring a fictional Pie Shop orchestration system. This system manages the complete lifecycle of pie orders through robot services (fruit picking → prep → baking → drone delivery).

## Key Design Decisions

### 1. **Deliberately Incomplete for Learning**
The system intentionally includes gaps, TODOs, and areas for improvement to create natural discussion points:
- Security checkpoints without implementation (auth, secrets management)
- Accessibility violations in UI (WCAG 2.1 AA issues)
- Mixed test quality (some excellent, some needing improvement)
- Incomplete error handling and observability
- Outdated documentation

### 2. **Language-Agnostic Specification**
The specification defines WHAT needs to be built, not HOW:
- Complete feature spec with requirements and acceptance criteria
- API contracts (OpenAPI format)
- Data models (conceptual, not language-specific)
- Architecture patterns described, not implemented
- Can generate implementations in Python, Node.js, Java, Go, etc.

### 3. **Multi-Dimensional Assessment**
The system evaluates candidates across multiple competencies:
- **Backend Architecture**: State machines, service integration, distributed systems
- **Code Quality**: Structure, testing, documentation, maintainability
- **Security**: Auth, input validation, secrets management
- **Accessibility**: WCAG compliance, Section 508 standards
- **Operations**: Observability, deployment, scaling, reliability
- **Product Thinking**: Requirements clarification, edge cases, user needs

### 4. **Interview-Friendly Scope**
- Understandable in 15 minutes
- Runnable locally with Docker Compose
- 50+ discussion points for 1-hour interviews
- Not obviously an "interview project" (realistic domain)

## What Was Created

### 1. Project Constitution (`.specify/memory/constitution.md`)
Defines the guiding principles for the entire project:
- **Core Principles**: Specification-first, deliberately incomplete, realistic complexity
- **Technical Decisions**: State machine orchestration, polyglot persistence, REST API
- **Quality Standards**: Code quality, testing, accessibility, security expectations
- **Anti-Patterns**: What to avoid in specifications and implementations
- **Success Metrics**: How to measure template and candidate assessment success

### 2. Feature Specification (`.specify/features/001-pie-shop-orchestration.md`)
Complete specification including:
- **Problem Statement**: Why this project exists (interview assessment needs)
- **User Stories**: Customer orders, admin monitoring, system orchestration
- **Functional Requirements** (FR-001 to FR-012):
  - Order Management API (REST endpoints)
  - State Machine Orchestration (ORDERED → PICKING → PREPPING → BAKING → DELIVERING → COMPLETED)
  - External Service Integration (fruit picker, baker, drone delivery)
  - Pie Catalog (simple recipes)
  - Data Persistence (PostgreSQL + MongoDB + InfluxDB)
  - Configuration Management (partial, with TODOs)
  - Error Handling and Logging (basic, with gaps)
  - Customer Order Form UI (with accessibility issues)
  - Admin Dashboard UI (with accessibility issues)
  - API Documentation (Swagger/OpenAPI)
- **Non-Functional Requirements**: Performance, scalability, reliability, observability, security, accessibility
- **API Contracts**: Request/response formats for all endpoints
- **External Service Contracts**: OpenAPI specs for robot services
- **Data Models**: Order entity, state history, pie recipes
- **Testing Strategy**: Unit, integration, contract tests (mixed quality)
- **Deployment**: Docker Compose for local, production TODOs

### 3. Interview Guide (`.specify/INTERVIEW_GUIDE.md`)
Comprehensive interviewer resource with:
- **Interview Structure**: Recommended 60-minute flow
- **50+ Discussion Points** organized by category:
  1. Architecture & Design Patterns (state machine, service integration, data persistence)
  2. Code Quality & Maintainability (organization, testing, documentation)
  3. Security (auth, input validation, secrets management)
  4. Accessibility (WCAG violations, ARIA, keyboard navigation)
  5. Observability & Operations (logging, monitoring, error handling, deployment)
  6. Scaling & Performance (bottlenecks, alternative architectures)
  7. Domain Knowledge & Product Thinking (edge cases, data consistency)
- **Expected Critiques**: What candidates should identify at each level
- **Strong Candidate Responses**: What good answers look like
- **Red Flags**: Warning signs to watch for
- **Interviewer Prompts**: Questions to probe deeper
- **Scoring Rubric**: Junior, Mid, Senior, Staff/Principal expectations
- **Sample Interview Flow**: Step-by-step guide
- **Role Customization**: How to adapt for Backend, Full Stack, DevOps, Security, Accessibility roles

### 4. OpenCode Command Files (`.opencode/command/`)
Spec-kit workflow commands for OpenCode agent:
- `speckit.specify.md`: Create feature specifications
- `speckit.clarify.md`: Resolve ambiguities before implementation
- `speckit.plan.md`: Generate implementation plans from specs
- `speckit.tasks.md`: Break plans into actionable tasks
- `speckit.implement.md`: Execute tasks with TDD approach

## Intentional Gaps for Discussion

### Security Issues
- ❌ No authentication (all endpoints open)
- ❌ No authorization (no RBAC)
- ❌ Hard-coded API keys and secrets
- ❌ Incomplete input validation
- ❌ No rate limiting
- ❌ Missing CORS configuration

### Accessibility Issues (UI)
- ❌ Missing form labels
- ❌ Poor color contrast (fails WCAG AA)
- ❌ No keyboard navigation
- ❌ Error messages not announced to screen readers
- ❌ Table missing proper headers and scope
- ❌ Status indicators use color only
- ❌ Improper heading hierarchy
- ❌ ARIA attributes misused
- ❌ Focus indicators removed

### Observability Gaps
- ❌ No distributed tracing (no trace IDs)
- ❌ No circuit breakers for failing services
- ❌ Insufficient retry logic (no exponential backoff)
- ❌ No dead letter queue
- ❌ Missing health check endpoints
- ❌ No alerting strategy

### Code Quality Issues
- ❌ Configuration inconsistency (some env vars, some hard-coded)
- ❌ Mixed test quality (some excellent, some missing edge cases)
- ❌ Verbose mock setup in tests
- ❌ Outdated README
- ❌ Inconsistent inline comments
- ❌ No architecture diagrams

### Production Readiness
- ❌ No Kubernetes manifests
- ❌ No CI/CD pipeline
- ❌ Missing container resource limits
- ❌ No graceful shutdown
- ❌ No zero-downtime deployment strategy
- ❌ Missing liveness/readiness probes

## Next Steps: Generating Implementations

When you're ready to use this for an interview:

### Step 1: Use the Implementation Prompt
Copy the prompt from `.specify/IMPLEMENTATION_PROMPT.md` and paste it to the **modern-architect-engineer** agent.

The agent will ask you three questions:
1. **What programming language and framework?** (Python/FastAPI, Node.js/Express, Java/Spring Boot, Go/Gin, etc.)
2. **What role is this interview for?** (Backend, Full Stack, DevOps, Security, Accessibility Specialist)
3. **What is the candidate's experience level?** (Junior, Mid, Senior, Staff+)

### Step 2: Agent Generates Everything
The modern-architect-engineer agent will create:
1. ✅ Complete working implementation with intentional technical debt
2. ✅ Docker Compose setup (working but missing production concerns)
3. ✅ Mock external services with realistic delays and failures
4. ✅ Tests with mixed quality (some excellent, some gaps)
5. ✅ UI with intentional accessibility violations
6. ✅ README (functional but outdated)
7. ✅ **Language-specific interview guide** with exact file paths and line numbers
8. ✅ Quick reference card for interviewers

### Step 3: Interview Guide Structure
The generated guide will have 4 layers optimized for live code review:

**Layer 1: Orientation (5 min)** - Quick overview of structure  
**Layer 2: Guided Tour (15 min)** - 5-7 critical checkpoints with file/line references  
**Layer 3: Candidate Exploration (30 min)** - Candidate-driven with backup prompts  
**Layer 4: Wrap-up (10 min)** - Cover missed areas, prioritization discussion

Each discussion point includes:
- Exact file path and line numbers
- What the code does
- What's wrong or worth discussing
- Good vs weak candidate responses
- Follow-up probes
- Markers: [CRITICAL], [ROLE: Backend], [LEVEL: Senior+], [TOPIC: Security]

### Step 4: Conduct Interview
1. **Prep (15 min before)**: Review the generated interview guide
2. **Share setup**: Give candidate screenshare control or repo access
3. **Orient (5 min)**: "This is a pie shop system, here's the structure..."
4. **Tour (15 min)**: "Let's start by looking at [file], can you navigate there?"
5. **Explore (30 min)**: "What catches your attention? What would you improve?"
6. **Wrap (10 min)**: "If you had a week, what would you prioritize?"

Use the guide's backup prompts if candidate misses critical areas.

## Files Created

```
.specify/
├── memory/
│   └── constitution.md                  # Project principles and standards
├── features/
│   └── 001-pie-shop-orchestration.md    # Complete feature specification
├── IMPLEMENTATION_PROMPT.md             # Prompt for generating code + interview guide
├── INTERVIEW_GUIDE.md                   # General interviewer resource (50+ discussion points)
└── PROJECT_SUMMARY.md                   # This file

.opencode/command/
├── speckit.specify.md                   # Command to create specs
├── speckit.clarify.md                   # Command to resolve ambiguities
├── speckit.plan.md                      # Command to create implementation plans
├── speckit.tasks.md                     # Command to define tasks
└── speckit.implement.md                 # Command to execute implementation
```

## Key Statistics

- **Specification Length**: 1,800+ lines
- **Discussion Points**: 50+ identified issues and improvements
- **Requirements**: 12 functional, 8 non-functional
- **User Stories**: 5 covering customer, admin, and system perspectives
- **API Endpoints**: 4 REST endpoints defined
- **External Services**: 3 robot service contracts (picker, baker, delivery)
- **Data Models**: 3 entities (orders, state_history, pie_recipes)
- **States**: 7 (ORDERED, PICKING, PREPPING, BAKING, DELIVERING, COMPLETED, ERROR)
- **Accessibility Issues**: 9 intentional WCAG violations
- **Security Gaps**: 6 checkpoints for discussion
- **Test Scenarios**: 10+ areas with mixed coverage

## Interview Assessment Coverage

This system can evaluate candidates on:

✅ **Architecture**: State machines, service integration, distributed systems, event-driven patterns, saga pattern  
✅ **API Design**: REST conventions, error handling, versioning, documentation  
✅ **Data Modeling**: Relational vs document vs time-series, polyglot persistence  
✅ **Testing**: Unit, integration, contract tests, test quality, coverage strategy  
✅ **Security**: Authentication, authorization, input validation, secrets management  
✅ **Accessibility**: WCAG 2.1 AA, Section 508, ARIA, keyboard navigation  
✅ **Observability**: Logging, metrics, tracing, monitoring, alerting  
✅ **Resilience**: Retries, circuit breakers, timeouts, error handling  
✅ **Operations**: Deployment, scaling, configuration, health checks  
✅ **Code Quality**: Structure, naming, documentation, maintainability  
✅ **Product Thinking**: Requirements clarification, edge cases, user needs  

## Why This Works for Interviews

1. **Approachable Domain**: Everyone understands ordering a pie
2. **Technical Depth**: Distributed systems, state management, service integration
3. **Multiple Discussion Angles**: Architecture, code quality, security, accessibility, operations
4. **Not Obviously Fake**: Realistic complexity and considerations
5. **Quick Setup**: Runs locally with Docker Compose
6. **Flexible Assessment**: Adapt for different roles and experience levels
7. **Language Agnostic**: Generate implementations in any language
8. **Respectful of Time**: 1-hour conversation, no extensive take-home

---

**Status**: ✅ Specification phase complete  
**Branch**: `001-pie-shop-specification`  
**Next Phase**: Generate language-specific implementation when needed  
**Ready For**: Code review interviews, architecture discussions, technical assessments
