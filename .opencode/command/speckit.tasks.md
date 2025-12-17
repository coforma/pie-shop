# Spec-Kit: Tasks Command

Use this command to break implementation plans into ordered, actionable tasks. This is typically handled by the modern-architect-engineer agent.

## Usage

When the user says "tasks [feature]" after plan is complete:

1. **Review Implementation Plan**: Understand architecture and dependencies.

2. **Break Down into Tasks**: Create task list in `specs/###-feature-name/tasks.md`:
   - Each task should be completable and testable
   - Order matters (dependencies first)
   - Mark parallel-safe tasks with [P]
   - Include testing tasks, not just features

3. **Task Format**:
   ```markdown
   - [ ] 1. Setup database schema (orders, state_history tables)
   - [ ] 2. Create data access layer with ORM models
   - [ ] 3. [P] Implement state machine logic
   - [ ] 4. [P] Create service client for fruit picker API
   - [ ] 5. Write unit tests for state transitions
   - [ ] 6. Create REST API endpoints (POST /orders, GET /orders/:id)
   - [ ] 7. Integration test: complete order flow
   - [ ] 8. Add OpenAPI documentation
   ```

4. **Validation Criteria per Task**:
   - What defines "done" for this task?
   - What tests prove it works?
   - What acceptance criteria does it satisfy?

## Example

User: "tasks 001"

Response:
- Review plan.md for feature 001
- Create ordered task list (database → logic → API → tests)
- Mark independent tasks as [P] for parallel execution
- Include quality gates (linting, type checking, tests)

## Key Principles

- **Actionable**: Each task is clear and unambiguous
- **Testable**: Know when it's done
- **Ordered**: Dependencies are respected
- **Right-sized**: Not too big (> 1 day) or too small (< 30 min)
- **Quality included**: Testing is part of tasks, not an afterthought
