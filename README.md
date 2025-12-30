# Pie Shop - Interview Assessment System

A language-agnostic specification for conducting technical interviews through code review of a realistic distributed system.

## What Is This?

The Pie Shop is a deliberately designed interview project featuring a fictional bakery that orchestrates pie orders through robot services:

**Order Flow**: Customer orders pie → Robot picks fruit → Ingredients prepped → Robot bakes pie → Drone delivers

The system is **intentionally incomplete** with realistic technical debt to create natural discussion points about:
- Architecture patterns (state machines, service integration, distributed systems)
- Code quality and testing
- Security (authentication, secrets management, input validation)
- Accessibility (WCAG 2.1 AA compliance)
- Operations (observability, deployment, scaling)

**Key Philosophy**: The code works but has realistic problems for candidates to identify and discuss during live code review.

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

### Interview Branches (Generated Implementations)
**Branch naming**: `interview/[language]-[role]-[level]`

Examples:
- `interview/python-backend-senior` 
- `interview/nodejs-fullstack-mid`
- `interview/csharp-backend-senior`

```
pie-shop/  (on interview/python-backend-senior)
├── (all from main, plus:)
├── src/                               # Language-specific implementation
├── tests/                             # Mixed quality tests
├── mocks/                             # Mock robot services
├── ui/                                # UI with accessibility issues
├── docker/                            # Docker setup
├── docker-compose.yml                 # Works locally, has operational gaps
└── README.md                          # Updated with setup instructions
```

**Note**: Interview guides (`INTERVIEW_GUIDE_PYTHON.md`, etc.) are **never committed**. They're stored in company shared drive only.

## Quick Start - Generate an Interview

Use the built-in slash commands with your preferred AI coding tool:

| Tool | Command |
|------|---------|
| **OpenCode** | `/generate-interview python-fastapi backend senior` |
| **Claude Code** | `/project:generate-interview python-fastapi backend senior` |
| **Cursor** | `/generate-interview python-fastapi backend senior` |
| **GitHub Copilot** | Attach `.github/prompts/generate-interview.prompt.md` |
| **Windsurf** | `@generate-interview` then describe parameters |

**Parameters:**
- `language`: python-fastapi, nodejs-express, csharp-dotnet, java-springboot, go-gin, ruby-rails, typescript-nestjs
- `role`: backend, fullstack, devops, security, accessibility
- `level`: junior, mid, senior, staff

The command will:
1. Create an interview branch (`interview/<language>-<role>-<level>`)
2. Read specifications from `.specify/`
3. Generate implementation with intentional technical debt
4. Prompt you to review, commit, and push

**After generation:**
- Review code for any accidental interview hints
- Ask the AI to generate an interview guide (save to shared drive, never commit)
- Push branch and share URL with candidate

---

## Detailed Workflow - Running an Interview

### For Interviewers: Setting Up a New Interview

**Step 1: Generate Implementation** (30-45 min, one-time per language/role/level)

**Option A: Use Slash Command (Recommended)**

Run the appropriate command for your AI tool (see Quick Start above). The AI will:
- Create the branch
- Generate all implementation files
- Follow the no-interview-hints guidelines

**Option B: Manual Generation**

1. Create interview branch:
   ```bash
   git checkout main
   git pull
   git checkout -b interview/[language]-[role]-[level]
   # Example: interview/python-backend-senior
   ```

2. Copy the prompt from `.specify/IMPLEMENTATION_PROMPT.md` and paste it to your AI coding agent

3. Answer three questions:
   - **Language/Framework?** - Python/FastAPI, Node.js/Express, Java/Spring Boot, Go/Gin, etc.
   - **Role?** - Backend, Full Stack, DevOps, Security, Accessibility
   - **Experience Level?** - Junior, Mid, Senior, Staff+

4. The agent generates:
   - Complete working code with intentional technical debt
   - Docker Compose setup
   - Mock robot services
   - Tests (mixed quality)
   - UI with accessibility violations

**Step 2: Commit and Push**

```bash
git add .
git commit -m "Add [Language] implementation for [Role] [Level] interviews"
git push -u origin interview/[language]-[role]-[level]
```

**Step 3: Generate Interview Guide Separately**

Ask your AI agent to generate an interview guide with discussion points, file paths, and expected responses. **Save this to company shared drive - never commit to the repository.**

Location: `[Company Drive]/Recruiting/Pie-Shop-Interview-Guides/`

**Step 4: Send Candidate the Branch** (1-2 days before interview)

Email the candidate:
```
Hi [Candidate],

For your upcoming interview on [DATE] at [TIME], please review this codebase:
https://github.com/coforma/pie-shop/tree/interview/python-backend-senior

Allow 1-2 hours to review. The code works but has intentional gaps for discussion.
Come ready to discuss what you observe - strengths, weaknesses, and trade-offs.

Looking forward to our discussion!
```

**Step 3: Prep Interview** (15 min before interview)

1. Download interview guide from company shared drive
2. Review Layer 2 checkpoints (guided tour)
3. Note backup prompts in Layer 3
4. Have code open in one screen, guide in another (private screen)

**Step 4: Conduct Interview** (60 minutes)

- **5 min - Orient**: "This is a pie shop system that orchestrates orders through robot services. Let me give you a quick tour..."
- **15 min - Guided Tour**: Walk through 2-3 critical checkpoints from guide (state machine, service clients, security, etc.)
- **30 min - Candidate Explores**: They navigate and share observations. Use backup prompts if they miss critical areas
- **10 min - Wrap-up**: "What would you prioritize? What would you need before production?"

**Step 5: Post-Interview** (15 min)

1. Document assessment using scoring rubric from guide
2. **Delete guide from local downloads** (security)
3. Share feedback with team

### What to Look For

**Strong Candidates**:
- ✅ Identify specific issues with examples
- ✅ Discuss trade-offs ("Simple but won't scale because...")
- ✅ Proactively mention security, accessibility, operations
- ✅ Prioritize improvements logically
- ✅ Balance pragmatism with quality

**Red Flags**:
- ❌ Can't identify issues without heavy prompting
- ❌ Suggests complete rewrites without reason
- ❌ Misses critical security/accessibility problems
- ❌ Dogmatic about patterns without discussing trade-offs

## Why This Works for Interviews

✅ **Not Obviously Fake**: Realistic distributed system complexity  
✅ **Approachable Domain**: Everyone understands ordering a pie  
✅ **Multi-Dimensional**: Tests architecture, code, security, accessibility, operations  
✅ **Language Agnostic**: Generate in any language  
✅ **Time Efficient**: 1-hour interview, no take-home burden  
✅ **50+ Discussion Points**: Rich conversation opportunities  
✅ **Production-Like**: Real technical debt, not contrived problems

## Intentional Issues Included

### Security
- No authentication/authorization
- Hard-coded secrets
- Incomplete input validation
- No rate limiting

### Observability
- No distributed tracing
- No circuit breakers
- Basic retry logic (no exponential backoff)
- Missing health checks

### Accessibility (UI)
- Missing form labels
- Poor color contrast
- No keyboard navigation
- Improper ARIA usage
- Status indicators using color only

### Code Quality
- Some functions too long
- Hard-coded configuration values
- Inconsistent error handling
- Mixed test quality

### Operations
- No Kubernetes manifests
- Missing container resource limits
- No graceful shutdown
- Outdated documentation

## Philosophy: Specification-Driven Development

This project follows **Specification-Driven Development (SDD)**:

1. **Specifications as Source of Truth**: The spec defines WHAT, implementations define HOW
2. **Language Agnostic**: One spec, many implementations
3. **Executable Specifications**: Detailed enough to generate working code
4. **Deliberately Incomplete**: Gaps create learning opportunities

See `.specify/memory/constitution.md` for complete design philosophy.

## Branch Strategy & Workflow

### Main Branch
- Contains only specifications and templates
- No generated code or interview guides
- Safe to be public
- **Never merge interview branches to main**

### Interview Branches
- One branch per language/role/level combination
- Contains generated implementation code
- Shared directly with candidates via branch URL
- **Never merged to main** (kept separate)
- Can be regenerated if spec changes

### Interview Guides
- Generated alongside implementation
- **Stored in company shared drive ONLY**
- Never committed to any branch (blocked by `.gitignore`)
- Downloaded by interviewers before each interview
- Deleted after interview (security)

See `.specify/WORKFLOW_PROPOSAL.md` for complete workflow details.

## Documentation

- **📋 Feature Spec**: `.specify/features/001-pie-shop-orchestration.md` - Complete requirements
- **🏛️ Constitution**: `.specify/memory/constitution.md` - Design principles  
- **🎯 Implementation Prompt**: `.specify/IMPLEMENTATION_PROMPT.md` - How to generate code
- **📊 Project Summary**: `.specify/PROJECT_SUMMARY.md` - Overview and statistics
- **🔒 Interview Guides**: Stored on company shared drive (not in repo) - See `.specify/INTERVIEW_GUIDE_SHARED_DRIVE_README.md`

## Customization for Different Roles

The generated interview guide includes markers:

- `[CRITICAL]` - Must cover
- `[ROLE: Backend]` - Relevant for backend engineers
- `[ROLE: Full Stack]` - Relevant for full stack engineers
- `[ROLE: DevOps]` - Relevant for SRE/DevOps
- `[ROLE: Security]` - Relevant for security engineers
- `[LEVEL: Senior+]` - For experienced candidates
- `[TOPIC: Security]` - Organized by topic area

## Interview Scoring

### Junior (0-2 years)
- Identifies obvious issues
- Understands basic patterns
- Asks good questions

### Mid (2-5 years)
- Identifies most security/accessibility issues
- Discusses trade-offs
- Shows production experience

### Senior (5-8 years)
- Systematic analysis across all areas
- Provides alternatives with rationale
- Discusses operational concerns

### Staff+ (8+ years)
- All of senior, plus:
- Connects technical to business outcomes
- Proposes migration strategies
- Discusses organizational impacts

## Assessment Dimensions

This system evaluates:
- ✅ Backend Architecture (state machines, service integration, distributed systems)
- ✅ API Design (REST, versioning, error handling, contracts)
- ✅ Code Quality (structure, testing, maintainability)
- ✅ Security (auth, validation, secrets management)
- ✅ Accessibility (WCAG 2.1 AA, Section 508, keyboard navigation)
- ✅ Observability (logging, metrics, tracing, alerting)
- ✅ Operations (deployment, scaling, reliability)
- ✅ Product Thinking (requirements, edge cases, prioritization)

## About Coforma

This interview system aligns with Coforma's values:
- **Ethics-first**: Accessibility and security are never optional
- **Human-centered**: Consider end users in all technical decisions
- **Public service**: Built for government work (Section 508 compliance)
- **Partnerships**: Collaborative assessment, not interrogation

See `AGENTS.md` for complete Coforma context and coding standards.

## Contributing

To improve this interview system:

1. Identify gaps in assessment coverage
2. Suggest additional intentional issues
3. Propose new discussion scenarios
4. Share calibration feedback

See `.specify/memory/constitution.md` for amendment process.

## License

Provided for use by Coforma and partners. Modify as needed for your interview process.

---

**Ready to interview?** Run `/generate-interview` with your AI coding tool to get started!
