# Spec-Kit: Clarify Command

Use this command to ask clarifying questions about ambiguous specifications before moving to implementation.

## Usage

When the user says "clarify" or you encounter `[NEEDS CLARIFICATION]` markers:

1. **Review Specifications**: Scan all feature specs for ambiguities or incomplete requirements.

2. **Ask Targeted Questions**: For each ambiguity:
   - Ask 3-5 specific questions (not overwhelming)
   - Explain WHY each answer is needed
   - Provide examples of good answers
   - Show the impact on implementation

3. **Document Answers**: When user responds:
   - Remove `[NEEDS CLARIFICATION]` markers
   - Add concrete requirements (FR-XXXa, FR-XXXb)
   - Update spec version (v1.0 → v1.1)
   - Add "Clarifications Applied" section at bottom

4. **Verify Completeness**: Ensure zero ambiguities remain before planning phase.

## Example

Spec contains: "System should handle errors gracefully [NEEDS CLARIFICATION]"

Response:
```
I need clarification on error handling:

1. **Retry Strategy** (FR-008):
   Q: How many retries for external service failures?
   Why: Determines resilience vs latency trade-off
   Example: "3 retries with exponential backoff" or "no retries, fail fast"

2. **User Communication** (FR-009):
   Q: What should users see when an error occurs?
   Why: Affects UX and support burden
   Example: "Generic message" or "Specific error with recovery steps"

3. **Error Recovery** (FR-010):
   Q: Can failed orders be manually retried?
   Why: Determines if we need admin intervention features
   Example: "Yes, admin can retry" or "No, customer must reorder"
```

After user answers, update spec with new FR-008a, FR-008b, FR-008c requirements.

## Key Principles

- **Stop on ambiguity**: Don't proceed with unclear requirements
- **Specific questions**: Not "what did you mean?" but "option A or B?"
- **Document everything**: Every answer becomes a requirement
- **Version specs**: Track what changed and when
