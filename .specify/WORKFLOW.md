# Pie Shop Interview Workflow

## Overview

This repository uses a **branch-per-language** strategy where:
- **Main branch** = Template with specifications only (no code)
- **Interview branches** = Generated implementations, one per language/role/level
- **Interview guides** = Stored on company shared drive (never committed)

## Branch Strategy

### Main Branch
**Contains:**
- Feature specifications (`.specify/features/`)
- Constitution and principles (`.specify/memory/`)
- Implementation generation prompt (`.specify/IMPLEMENTATION_PROMPT.md`)
- This workflow guide

**Does NOT contain:**
- Implementation code
- Interview guides (any discussion points, expected answers, or rubrics)

### Interview Branches
**Naming:** `interview/[language]-[role]-[level]`

Examples:
- `interview/python-backend-senior`
- `interview/nodejs-fullstack-mid`
- `interview/csharp-backend-senior`

**Contains:**
- Everything from main branch
- Generated implementation code
- Docker setup, tests, mocks
- Language-specific README

**Does NOT contain:**
- Interview guides (blocked by `.gitignore`)

**Lifecycle:**
- Never merged to main
- Shared directly with candidates via branch URL
- Regenerated when specifications change

## Workflow: Conducting an Interview

### 1. Generate Implementation (One-time per language/role)

```bash
# Create interview branch
git checkout main
git pull
git checkout -b interview/python-backend-senior

# Generate implementation
# Copy .specify/IMPLEMENTATION_PROMPT.md and paste to modern-architect-engineer agent
# Answer: Language (Python/FastAPI), Role (Backend), Level (Senior)

# Commit implementation only (NOT the guide)
git add src/ tests/ docker/ ui/ docker-compose.yml README.md
git commit -m "Add Python implementation for Senior Backend Engineer interviews"
git push -u origin interview/python-backend-senior

# Store interview guide on company shared drive
# Save generated INTERVIEW_GUIDE_PYTHON.md to:
# [Company Drive]/Recruiting/Pie-Shop-Interview-Guides/
```

### 2. Share with Candidate (1-2 days before)

Send candidate the branch URL:
```
https://github.com/coforma/pie-shop/tree/interview/python-backend-senior
```

Email template:
```
Hi [Candidate],

For your upcoming interview on [DATE], please review this codebase:
https://github.com/coforma/pie-shop/tree/interview/python-backend-senior

Allow 1-2 hours. The code works but has intentional gaps for discussion.

See you then!
```

### 3. Prepare for Interview (15 min before)

1. Download interview guide from company shared drive
2. Review key checkpoints and backup prompts
3. Have code open in one screen, guide in another (private)

### 4. Conduct Interview (60 min)

- **5 min**: Orient candidate to system
- **15 min**: Guided tour of 2-3 key areas (state machine, service clients, security)
- **30 min**: Candidate explores and discusses observations
- **10 min**: Wrap-up ("What would you prioritize?")

### 5. After Interview

1. Document assessment
2. Delete guide from local machine (security)
3. Share feedback with team

## Security: Interview Guides

**Storage:**
- All interview guides stored on company shared drive only
- Location: `[Company Drive]/Recruiting/Pie-Shop-Interview-Guides/`
- Includes generic guide + language-specific guides

**Never Commit:**
- `.gitignore` blocks `INTERVIEW_GUIDE*.md` patterns
- Guides contain discussion points, expected answers, and rubrics
- If guide is in repo, candidates could prep specific talking points

**Access:**
- Restricted to hiring managers and interviewers only
- Download before interview, delete after

## Maintenance

### When Specifications Change

```bash
# Update spec in main
git checkout main
# Edit .specify/features/001-pie-shop-orchestration.md
git commit -m "Update specification: [description]"
git push

# Regenerate each interview branch
git checkout interview/python-backend-senior
git rebase main
# Re-run implementation generation with modern-architect-engineer
git add .
git commit -m "Regenerate implementation from updated spec"
git push --force

# Update interview guide on shared drive
```

### Adding New Language/Role

```bash
# Create new branch from main
git checkout main
git checkout -b interview/go-devops-senior

# Follow "Generate Implementation" steps above
```

## Quick Reference

**To generate implementation:**
```bash
git checkout -b interview/[lang]-[role]-[level]
# Use .specify/IMPLEMENTATION_PROMPT.md with modern-architect-engineer
git add [code files only]
git commit && git push
# Upload guide to shared drive
```

**To share with candidate:**
```
https://github.com/coforma/pie-shop/tree/interview/[lang]-[role]-[level]
```

**Interview guide location:**
```
[Company Drive]/Recruiting/Pie-Shop-Interview-Guides/
```

## What Candidates See vs. Interviewers See

**Candidates (public repo/branches):**
- ✅ Feature specifications
- ✅ Implementation code with intentional issues
- ✅ Docker setup, tests, mocks
- ❌ NO interview guides

**Interviewers (company shared drive):**
- 🔒 Generic interview guide (50+ discussion topics)
- 🔒 Language-specific guides (file paths, expected responses, rubrics)
- 🔒 User manual for conducting interviews

---

**Questions?** See `.specify/IMPLEMENTATION_PROMPT.md` for detailed generation instructions.
