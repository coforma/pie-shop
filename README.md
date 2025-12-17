# Tech Challenge Template

A comprehensive GitHub repository template designed for technology challenges and projects. This template provides essential GitHub features and workflows to help teams collaborate effectively from day one.

## How to Use This Template

### Option 1: Use GitHub's Template Feature (Recommended)

1. **Click "Use this template"** - Look for the green "Use this template" button at the top of this repository
2. **Choose "Create a new repository"**
3. **Configure your new repository:**
   - Select the owner (your account or organization)
   - Enter a repository name
   - Choose visibility (public or private)
   - Optionally include all branches (usually just main is sufficient)
4. **Click "Create repository from template"**

### Option 2: Manual Setup

If you prefer to set up manually or need to customize the process:

```bash
# Clone this template
git clone https://github.com/coforma/tech-challenge-template.git your-project-name

# Navigate to the project
cd your-project-name

# Remove the original git history and start fresh
rm -rf .git
git init
git add .
git commit -m "Initial commit from tech-challenge-template"

# Connect to your new remote repository
git remote add origin https://github.com/yourusername/your-project-name.git
git push -u origin main
```

## Features Included

This template comes pre-configured with essential GitHub features to streamline your development workflow:

### Issue Templates

**Bug Report Template** (`.github/ISSUE_TEMPLATE/bug_report.md`)
- Structured format for reporting bugs
- Includes sections for current behavior, reproduction steps, and expected behavior
- Helps maintain consistency in bug reporting

**User Story Template** (`.github/ISSUE_TEMPLATE/user-story-template.md`)
- Follows agile user story format: "As a [persona], I [want to], [so that]"
- Includes Definition of Ready checklist
- Contains acceptance criteria and task tracking
- Links to Definition of Done for consistency

**HCD (Human-Centered Design) Template** (`.github/ISSUE_TEMPLATE/hcd-template.md`)
- Specialized for design and research deliverables
- Includes HCD-specific acceptance criteria
- Pre-labeled with 'hcd' tag for easy filtering
- Contains documentation and deliverable tracking

**Configuration** (`.github/ISSUE_TEMPLATE/config.yml`)
- Disables blank issues to encourage use of structured templates
- Ensures all issues follow established formats

### LLM Configuration Guide

**AGENTS.md** - Comprehensive guide for Large Language Models
- Coforma company context, values, and technical approach
- Professional communication standards (no emojis, government-appropriate tone)
- Code quality guidelines emphasizing accessibility and security
- Collaboration principles and workflow integration
- Customizable section for project-specific requirements
- Template for teams to add challenge-specific rules and constraints

### Pull Request Template

**Comprehensive PR Template** (`.github/PULL_REQUEST_TEMPLATE.md`)
- Summary and context sections
- Issue linking requirements
- Change type categorization (bug fix, feature, documentation)
- Testing instructions
- Comprehensive checklist including:
  - Code style and linting
  - Self-review process
  - Documentation updates
  - Security considerations
  - Test coverage requirements

## Ideal Use Cases

This template is perfect for:

- **Technology challenges and coding assessments**
- **Prototype projects requiring structured collaboration**
- **Small to medium-sized development projects**
- **Projects requiring human-centered design workflows**
- **Teams that value structured issue tracking and PR processes**

## Getting Started After Template Creation

Once you've created your repository from this template:

1. **Update this README** - Replace this content with your project-specific information
2. **Customize AGENTS.md** - Update the fork-specific rules section in `AGENTS.md` with your project requirements, technology stack, evaluation criteria, and any challenge-specific guidelines for LLMs working with your repository
3. **Customize issue templates** - Modify templates in `.github/ISSUE_TEMPLATE/` to match your project needs
4. **Review PR template** - Adjust the pull request template checklist for your workflow
5. **Add project-specific content** - Include your code, documentation, and other project files
6. **Configure repository settings** - Set up branch protection, required reviews, etc.

## Repository Structure

```
.
├── README.md                          # This file
├── AGENTS.md                          # LLM configuration and guidelines
└── .github/
    ├── PULL_REQUEST_TEMPLATE.md       # PR template
    └── ISSUE_TEMPLATE/
        ├── bug_report.md              # Bug report template
        ├── config.yml                 # Issue template configuration
        ├── hcd-template.md            # Human-centered design template
        └── user-story-template.md     # User story template
```

## Contributing

This template is designed to be a starting point. Feel free to:

- Fork and modify for your organization's needs
- Submit issues or PRs to improve the template
- Share feedback on how these templates work for your projects

## License

This template is provided as-is for use by Coforma and collaborating organizations. Modify as needed for your projects.

---

**Ready to start your project?** Click the "Use this template" button above to create your new repository!
