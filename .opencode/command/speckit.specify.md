# Spec-Kit: Specify Command

Use this command to create or update feature specifications following the Specification-Driven Development methodology.

## Usage

When the user says "specify [feature description]" or asks you to create a specification:

1. **Check for Constitution**: Ensure `.specify/memory/constitution.md` exists. If not, create it first.

2. **Create Feature Specification**: Write a detailed specification in `.specify/features/###-feature-name.md` following this structure:

   - **Problem Statement**: What problem does this solve?
   - **Solution Overview**: High-level approach
   - **User Stories**: Who benefits and how (As a... I want... So that...)
   - **Functional Requirements**: Specific capabilities (FR-001, FR-002, etc.)
   - **Non-Functional Requirements**: Performance, security, accessibility, etc.
   - **API Contracts**: Request/response formats
   - **Data Models**: Entity structures
   - **Success Criteria**: How to measure completion

3. **Use Constitutional Principles**: All specifications must align with the project constitution.

4. **Mark Ambiguities**: Use `[NEEDS CLARIFICATION]` for anything unclear.

5. **Number Features**: Use sequential numbers (001, 002, 003).

## Example

User: "specify user authentication"

Response:
- Check constitution exists
- Create `.specify/features/003-user-authentication.md`
- Include problem, solution, user stories, requirements
- Reference constitutional security principles
- Mark any unclear requirements for clarification

## Key Principles

- **Language-agnostic**: Don't specify implementation details
- **Clear acceptance criteria**: Must be testable
- **Complete requirements**: No missing edge cases
- **Constitutional alignment**: Check against project principles
