# Pie Shop System - Project Constitution

**Version**: 1.0  
**Last Updated**: 2025-12-17  
**Purpose**: Interview and technical assessment project template

## Project Purpose

The Pie Shop System is a deliberately designed technical assessment project for evaluating candidate capabilities in:
- Backend orchestration and service integration
- Distributed system design patterns
- Code quality and maintainability
- Accessibility awareness
- Production readiness thinking
- Security considerations

This system manages the end-to-end workflow of pie orders, from ingredient sourcing through robotic preparation and delivery, serving as a realistic but approachable example of microservice orchestration.

## Core Principles

### 1. Specification-First, Language-Agnostic
The specification is the source of truth. Implementations can be generated in any programming language based on interview needs. All architectural decisions, contracts, and behaviors must be defined in the specification before implementation.

**Why**: Enables reuse across different interview contexts (Python, Node.js, Java, Go, etc.) while maintaining consistency in what's being evaluated.

### 2. Deliberately Incomplete for Discussion
The codebase intentionally includes gaps, TODOs, and areas for improvement. These are features, not bugs, designed to spark meaningful technical discussions about production readiness.

**Why**: Distinguishes candidates who can identify what's missing in a system versus those who only see what's present. Reveals production experience and architectural thinking.

### 3. Realistic Complexity, Manageable Scope
The system should represent real-world distributed system challenges (external service integration, state management, error handling) while remaining comprehensible in a 1-hour interview.

**Why**: Creates authentic assessment scenarios without overwhelming candidates or requiring extensive setup time.

### 4. Accessibility by Design (with Educational Gaps)
The system includes both good accessibility practices and intentional violations of WCAG 2.1 AA / Section 508 standards to assess candidate awareness.

**Why**: Aligns with Coforma's government work requirements and public sector focus. Tests whether candidates consider inclusive design.

### 5. Production Concerns Over Academic Perfection
The system prioritizes realistic production considerations (observability, configuration management, error handling, security checkpoints) over theoretical perfection.

**Why**: Evaluates practical engineering judgment, not memorized patterns. Shows how candidates think about operating systems in the real world.

### 6. Polyglot Persistence with Purpose
Different data storage patterns serve different needs: relational for transactional data, document store for flexible schemas, time-series for operational metrics.

**Why**: Creates discussion opportunities about data modeling trade-offs and storage pattern appropriateness.

### 7. Interview-Friendly Setup
The system must be runnable locally with minimal dependencies (Docker Compose) and clear documentation, even if that documentation is intentionally incomplete.

**Why**: Respects candidate and interviewer time. Enables quick demonstrations without infrastructure complexity.

## Technical Decisions

### Architecture Style
**Decision**: Simple state machine orchestration with external service integration  
**Rationale**: Balances realism with comprehensibility. Opens discussions about event-driven architecture and saga patterns without requiring their implementation.  
**Trade-offs**: Simpler than production systems, but intentionally so—creates teaching moments.

### Service Integration Pattern
**Decision**: Contract-first with mock implementations (swappable for real services)  
**Rationale**: Demonstrates API design thinking while keeping setup simple. Mocks use realistic delays and failure scenarios.  
**Trade-offs**: Real services would show more failure modes, but would complicate interview setup.

### API Style
**Decision**: REST API for order management  
**Rationale**: Universal understanding, straightforward to implement and discuss. Leaves room for candidates to suggest GraphQL, gRPC, or async alternatives.  
**Trade-offs**: REST is well-understood but not always optimal—good discussion point.

### State Management
**Decision**: Sequential state machine with explicit transitions  
**Rationale**: Clear, deterministic behavior. Easy to reason about during interviews. Provides contrast point for discussing event-driven alternatives.  
**Trade-offs**: Less flexible than event sourcing, but far simpler to understand quickly.

### Data Storage
**Decision**: Polyglot persistence (relational + document + time-series)  
**Rationale**: Shows understanding of different data access patterns and storage trade-offs.  
**Trade-offs**: More complex than single database, but creates rich discussion about data modeling.

### External Service Contracts
**Decision**: OpenAPI specifications with example implementations  
**Rationale**: Provides concrete contracts while allowing flexibility in how candidates would implement or improve them.  
**Trade-offs**: Contracts might not cover all edge cases—intentionally creates discussion opportunities.

### Deployment Strategy
**Decision**: Docker Compose for local development  
**Rationale**: Simple, cross-platform, widely understood. Leaves production deployment concerns as discussion topics.  
**Trade-offs**: Not production-ready (no orchestration, scaling, health checks)—but that's a feature for interviews.

### Testing Philosophy
**Decision**: Mixed test quality (some excellent, some needing improvement)  
**Rationale**: Gives candidates concrete examples to critique and improve upon.  
**Trade-offs**: Could confuse candidates about expectations, but provides assessment signal.

## Quality Standards

### Code Quality Expectations
- **Readability**: Code should be clear enough for a new developer to understand in 1 hour
- **Modularity**: Clear separation of concerns (API layer, business logic, external integrations)
- **Error Handling**: Present but incomplete—enough to show the pattern, gaps to discuss improvement
- **Documentation**: Intentionally outdated or incomplete in places to assess candidate attention to detail

### Testing Standards
- **Unit Tests**: Core business logic should have some test coverage (not 100%)
- **Integration Tests**: At least one end-to-end flow tested, others marked as TODO
- **Contract Tests**: External service contracts should have validation tests
- **Test Quality**: Mix of well-written and improvable tests

### Accessibility Standards
- **Violations to Include**: Missing ARIA labels, poor color contrast, keyboard navigation issues, improper heading hierarchy
- **Target**: WCAG 2.1 AA violations that are realistic and common
- **Purpose**: Assess whether candidates proactively identify accessibility concerns

### Security Standards
- **Authentication**: Basic implementation with TODOs for production concerns
- **Authorization**: Present but incomplete
- **Input Validation**: Some endpoints validated, others not
- **Secrets Management**: Hard-coded values with TODO comments
- **Purpose**: Checkpoints for security discussion, not actual security

### Observability Standards
- **Logging**: Structured logging at state transitions
- **Metrics**: Basic metrics with TODOs for distributed tracing
- **Monitoring**: Mention of health checks and alerting without full implementation
- **Purpose**: Show awareness of operational concerns

## Anti-Patterns to Avoid

### In Specification Design
❌ **Over-specification**: Don't define implementation details that constrain language choice  
❌ **Academic perfection**: Don't create a "textbook" system—make it realistic  
❌ **Hidden gotchas**: Intentional issues should be realistic, not trick questions  
❌ **Overwhelming complexity**: Keep scope manageable for 1-hour discussions

### In Implementation Generation
❌ **Production-ready code**: This is an interview tool, not a real service  
❌ **Single "correct" solution**: Multiple valid approaches should be discussable  
❌ **Obscure technologies**: Stick to mainstream, well-documented tools  
❌ **Tight coupling**: Make it easy to discuss how to improve modularity

### In Interview Usage
❌ **Gatekeeping**: Issues should reveal thinking, not test obscure knowledge  
❌ **Gotcha questions**: Gaps should be realistic, not artificially tricky  
❌ **Single dimension**: System should assess multiple skills (design, security, accessibility, testing)

## Success Metrics

### For the Template
- ✅ Can be understood by interviewers in < 15 minutes
- ✅ Can be set up and run locally in < 10 minutes
- ✅ Generates implementations in multiple languages from same spec
- ✅ Provides 20+ discussion points for a 1-hour interview
- ✅ Assesses multiple dimensions (architecture, code quality, security, accessibility)

### For Candidate Assessment
- ✅ Identifies production-readiness gaps without prompting
- ✅ Discusses trade-offs rather than claiming single "right" answer
- ✅ Considers multiple user types (accessibility, different technical contexts)
- ✅ Suggests concrete improvements with rationale
- ✅ Asks clarifying questions about requirements and constraints

### For Interview Experience
- ✅ Engaging and approachable (pie shop is relatable)
- ✅ Not obviously an "interview project" 
- ✅ Generates natural conversation rather than interrogation
- ✅ Allows candidates at different levels to demonstrate their knowledge
- ✅ Respectful of candidate time (no extensive take-home setup)

## Constitution Amendments

This constitution may be amended when:
1. Interview feedback reveals gaps or issues in the design
2. New assessment dimensions need to be added (e.g., AI integration, new security patterns)
3. Technology landscape changes make examples outdated
4. Language-specific implementations reveal specification ambiguities

**Amendment Process**:
1. Document the proposed change and rationale
2. Update constitution version and date
3. Review all feature specifications for consistency
4. Update any generated implementations if needed
5. Notify interview team of changes

---

**Constitutional Authority**: This document guides all specification and implementation decisions for the Pie Shop System. When in doubt, refer back to these principles.
