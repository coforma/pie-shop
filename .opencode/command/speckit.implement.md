# Spec-Kit: Implement Command

Use this command to execute implementation tasks. This is typically handled by the modern-architect-engineer agent.

## Usage

When the user says "implement [feature]" after tasks are defined:

1. **Review Task List**: Understand what needs to be built.

2. **Execute Tasks in Order**:
   - Follow task list from `specs/###-feature-name/tasks.md`
   - Mark tasks complete as you go: `- [x] 1. Task description`
   - Run quality checks after each significant task

3. **Test-Driven Approach**:
   - Write tests first (or alongside code)
   - Ensure tests pass before moving to next task
   - Run full test suite periodically

4. **Quality Gates**:
   - Linting (language-specific)
   - Type checking (if applicable)
   - Unit tests (aim for good coverage, not 100%)
   - Integration tests (happy path + key errors)
   - Security checks (input validation, auth)
   - Accessibility checks (if UI)

5. **Documentation**:
   - Update README if API changes
   - Add inline comments for complex logic
   - Update OpenAPI specs if endpoints change

6. **Commit Strategy**:
   - Commit after each task (or logical group)
   - Descriptive commit messages
   - Reference task number in commits

## Example

User: "implement 001"

Response:
- Start with task 1 (database schema)
- Write migration, apply it, verify
- Mark task complete
- Move to task 2 (data access layer)
- Continue through all tasks
- Run full quality checks at end
- Commit with meaningful messages

## Key Principles

- **TDD approach**: Test-driven or test-alongside development
- **Incremental progress**: Small, verifiable steps
- **Quality continuous**: Not just at the end
- **Acceptance criteria**: Verify each FR is met
- **Constitutional alignment**: Follow project principles
