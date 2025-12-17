# Spec-Kit: Plan Command

Use this command to create implementation plans from complete specifications. This is typically handled by the modern-architect-engineer agent.

## Usage

When the user says "plan [feature]" after specifications are complete:

1. **Verify Prerequisites**:
   - Constitution exists
   - Feature specification is complete (no `[NEEDS CLARIFICATION]` markers)
   - All clarification questions answered

2. **Create Feature Branch**:
   ```bash
   git checkout -b 001-feature-name
   ```

3. **Generate Plan Structure**:
   ```bash
   .specify/scripts/bash/setup-plan.sh --json
   ```

4. **Write Implementation Plan** in `specs/###-feature-name/plan.md`:
   - **Technical Context**: Versions, dependencies, platform
   - **Constitution Check**: Verify alignment with project principles
   - **Project Structure**: Concrete directory/file paths
   - **Research Findings**: Technology choices with rationale
   - **Data Models**: Complete entity definitions
   - **API Contracts**: OpenAPI, GraphQL schemas, or event definitions
   - **Dependencies**: What must be built first
   - **Risks and Mitigations**: What could go wrong

5. **Create Supporting Documents**:
   - `research.md`: Technology evaluation and decisions
   - `data-model.md`: Entity relationships and constraints
   - `contracts/`: API specifications
   - `quickstart.md`: Key validation scenarios

## Example

User: "plan feature 001"

Response:
- Verify spec complete
- Create branch `001-pie-shop-orchestration`
- Run setup-plan.sh
- Write technical plan with architecture decisions
- Define data models (orders table, state_history table)
- Create OpenAPI contracts for robot services
- Document technology choices (Python/FastAPI, PostgreSQL, MongoDB)

## Key Principles

- **Executable specifications**: Plan must be implementable without clarification
- **Technology choices**: Research latest versions, justify selections
- **Constitutional alignment**: Reference principles for decisions
- **Concrete, not abstract**: Specific file paths, not "somewhere in src/"
