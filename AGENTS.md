# AGENTS.md - LLM Configuration Guide

This document provides context and guidelines for Large Language Models (LLMs) working with repositories created from this Coforma tech challenge template.

## About Coforma

### Company Overview
Coforma is a minority-owned small business founded in 2017 by CEO Eduardo Ortiz. We are a Service-Disabled Veteran-Owned Small Business (SDVOSB) with 8(a) certification from the Small Business Administration. Our 160+ person team specializes in digital transformation for government, nonprofit, and select commercial partners.

### Mission and Values
- **Public servants at heart**: We are Americans, immigrants, civil servants, and Veterans committed to building technology that helps people
- **Ethics-first approach**: Everything we do is framed in ethics, from coding to design to research
- **Human-centered design**: We design with and not for people, embedding with partner teams and including end users in design and research processes
- **Accessibility by design**: We create inclusive, accessible solutions that consider the digital divide and diverse user needs
- **Partnerships over consulting**: We co-create progress with our partners rather than providing traditional consulting services

### Focus Areas
1. **Healthcare**: Human-centered approach to addressing pain points in care, systems, and technologies
2. **Public Interest**: Empathy-led design to center diverse public needs and improve government processes
3. **Veterans Services**: Dedicated to streamlining access to support and services for Veteran communities

### Technical Approach
- **Agile methodology**: Modern, agile, mission-driven, and user-centered development practices
- **Cross-functional teams**: Diverse expertise working toward common goals (designers, content strategists, engineers, product managers)
- **Open-source technology**: Leverage open-source solutions to deliver custom products that change lives
- **API-driven architectures**: Modern system integrations and cloud-native solutions
- **DevSecOps**: Security-first development and deployment practices

## Repository Context

### Template Purpose
This repository template is designed for technology challenges and collaborative projects. It includes pre-configured GitHub features to support structured development workflows from day one.

### Included Features
- **Issue Templates**: Structured formats for bug reports, user stories, and human-centered design (HCD) deliverables
- **Pull Request Template**: Comprehensive checklist including code quality, testing, documentation, and security considerations
- **GitHub Workflow Integration**: Templates configured to support agile development and collaboration

## LLM Guidelines and Rules

### Professional Communication Standards
- **No emojis**: Maintain professional tone in all communications, documentation, and code comments
- **Clear, concise language**: Use straightforward language that is accessible to diverse audiences
- **Inclusive terminology**: Use inclusive language that reflects Coforma's commitment to diversity and accessibility
- **Government-appropriate tone**: Remember that many projects serve government and public sector clients
- **NO INTERVIEW HINTS IN CANDIDATE-FACING FILES**: 
  - **CRITICAL**: Candidates see the code - it must look like a real production codebase
  - README.md should be professional and helpful (not mention "intentional issues")
  - Code comments should use normal developer patterns:
    - ✅ Good: `// TODO: Add authentication middleware`
    - ✅ Good: `// TODO: Implement circuit breaker pattern`
    - ✅ Good: `// TODO: Move secrets to Key Vault`
    - ❌ Bad: `// SECURITY ISSUE: No authentication!`
    - ❌ Bad: `// intentional for interview`
    - ❌ Bad: `// This has technical debt for discussion`
  - Issues should be discovered through code review, not telegraphed
  - Interview guides (which ARE gitignored) contain all the discussion points

### Code Quality Standards
- **Accessibility first**: Ensure all code follows WCAG 2.2 guidelines and accessibility best practices
- **Security by design**: Consider security implications at every step of development
- **Documentation**: Thoroughly document code, particularly complex or hard-to-understand sections
- **Testing**: Implement comprehensive testing strategies (unit, integration, accessibility)
- **Code style consistency**: Follow established linting and formatting standards for the project

### Collaboration Principles
- **User-centered approach**: Always consider the end user impact of technical decisions
- **Cross-functional thinking**: Consider how changes affect design, content, accessibility, and user experience
- **Ethical considerations**: Evaluate whether technical choices align with doing good and avoiding harm
- **Iterative improvement**: Favor incremental, well-tested changes over large refactors

### Issue and PR Management
- **Use provided templates**: Always use the structured issue and PR templates
- **Link issues to PRs**: Connect pull requests to relevant issues using the "Fixes #(issue)" format
- **Comprehensive PR descriptions**: Include context, testing instructions, and impact assessment
- **Review checklist compliance**: Ensure all checklist items in PR template are addressed

### Documentation Standards
- **User-focused documentation**: Write documentation that serves the end user, not just developers
- **Accessibility in documentation**: Ensure documentation is accessible to users with disabilities
- **Maintenance notes**: Include information about ongoing maintenance and update requirements
- **Decision records**: Document significant architectural and design decisions

## Project-Specific Customization

### Pie Shop Interview System - Repository Workflow

**This repository is an interview assessment tool, not a production application.**

#### Repository Structure

**Main Branch:**
- Contains specifications, templates, and instructions only
- No implementation code
- No interview guides
- Safe to be public

**Interview Branches:**
- Branch naming: `interview/[language]-[role]-[level]`
- Examples: `interview/python-backend-senior`, `interview/nodejs-fullstack-mid`
- Contains generated implementation code with intentional technical debt
- Shared directly with candidates via branch URL
- **Never merged to main**

**Interview Guides:**
- Stored on company shared drive ONLY (never in repository)
- Contain discussion points, expected answers, and scoring rubrics
- Blocked by `.gitignore` (pattern: `INTERVIEW_GUIDE*.md`)

#### Workflow for Agents

When asked to work with this repository:

1. **Check current branch:**
   ```bash
   git branch --show-current
   ```

2. **If on main branch:**
   - You can update specifications (`.specify/features/`)
   - You can update constitution (`.specify/memory/`)
   - You can update workflow docs (`.specify/WORKFLOW.md`)
   - **DO NOT** generate implementation code
   - **DO NOT** commit interview guides

3. **If on interview branch (interview/*):**
   - You can modify implementation code
   - You can update tests, mocks, UI
   - You can regenerate code from specifications
   - **DO NOT** commit files matching `INTERVIEW_GUIDE*.md`
   - **NEVER** merge this branch to main

4. **To generate new interview implementation:**
   - Create new branch: `git checkout -b interview/[language]-[role]-[level]`
   - Use prompt from `.specify/IMPLEMENTATION_PROMPT.md`
   - Generate code with intentional technical debt
   - Commit code only (never the generated interview guide)
   - Guide should be saved to company shared drive

5. **Security Rules:**
   - Interview guides contain sensitive assessment materials
   - `.gitignore` blocks `INTERVIEW_GUIDE*.md` patterns
   - If user asks to commit a guide, remind them it should go to shared drive
   - If you accidentally generate a guide in the repo, alert the user

#### Interview Branch Lifecycle

**Creation:**
```bash
git checkout main
git checkout -b interview/python-backend-senior
# Generate implementation
git add src/ tests/ docker/ ui/ docker-compose.yml
git commit -m "Add Python implementation for Senior Backend interviews"
git push -u origin interview/python-backend-senior
```

**Sharing with Candidates:**
- Send candidate the branch URL directly (public repo)
- Candidates can clone/browse the specific branch
- They see code with intentional issues but no interview guide

**Maintenance:**
- When specifications change, rebase interview branch from main
- Regenerate implementation using updated spec
- Force push to update branch

**Never:**
- Never merge interview branches to main
- Never commit interview guides to any branch
- Never share interview guide with candidates

See `.specify/WORKFLOW.md` for complete workflow documentation.

### Common Challenge Patterns

When working on tech challenges using this template, consider these common requirements:

#### Assessment Criteria
- **Functionality**: Does the solution solve the stated problem effectively?
- **Code Quality**: Is the code well-structured, documented, and maintainable?
- **User Experience**: Is the solution accessible and user-friendly?
- **Technical Approach**: Are technical decisions well-reasoned and appropriate?
- **Documentation**: Is the solution well-documented for users and maintainers?

#### Best Practices for Tech Challenges
- **README-driven development**: Start with a clear README explaining the problem and approach
- **Incremental commits**: Make frequent, well-described commits showing development progression
- **Test coverage**: Include appropriate testing to demonstrate code quality
- **Deployment readiness**: Ensure the solution can be easily run and evaluated
- **Assumptions documentation**: Clearly document any assumptions made during development

## Getting Help

### Resources
- [Coforma Website](https://coforma.io): Learn more about our company and approach
- [Coforma Case Studies](https://coforma.io/case-studies): Examples of our work and methodology
- [Coforma Blog](https://coforma.io/perspectives): Insights on digital transformation and public service technology

### Contact
For questions about this template or Coforma's approach to technology challenges, refer to the repository maintainers or contact information provided in the specific challenge documentation.

---

*This AGENTS.md file should be updated when the template is forked for specific tech challenges to include project-specific requirements and guidelines.*