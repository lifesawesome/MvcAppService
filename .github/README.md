# GitHub Workspace Configuration

This directory contains custom instruction files, prompt templates, and chat modes to enhance your development workflow with AI assistants like GitHub Copilot.

## 📁 Directory Structure

```
.github/
├── instructions/         # Auto-applied coding guidelines
├── prompts/             # Reusable prompt templates
├── chatmodes/           # Specialized AI assistant modes
└── README.md           # This file
```

## 📋 Instructions Files

Instruction files are automatically applied based on file patterns. They guide AI assistants on how to write code for your project.

### Available Instructions

| File | Applies To | Purpose |
|------|-----------|---------|
| `SecurityInstructions.instructions.md` | All files (`**`) | OWASP security best practices, critical security rules |
| `AspNetCoreInstructions.instructions.md` | All C# files (`**/*.cs`) | ASP.NET Core MVC patterns, naming conventions, architecture |
| `DatabaseInstructions.instructions.md` | Data/Models/Migrations | Entity Framework Core, database design, query optimization |
| `FrontendInstructions.instructions.md` | Views/CSS/JS files | Razor views, HTML, CSS, JavaScript, Bootstrap, accessibility |

### How Instruction Files Work

1. **Automatic Application**: AI assistants automatically load relevant instruction files based on the file you're working with
2. **Context-Aware**: Different instructions apply to different file types
3. **Cumulative**: Multiple instruction files can apply to the same file

### Creating New Instructions

```markdown
---
applyTo: '**/*.cs'  # Glob pattern for file matching
---

# Your Instruction Title

Your guidelines and rules here...
```

## 🎯 Prompt Files

Prompt templates for common development tasks. Use these to get consistent, high-quality assistance.

### Available Prompts

| Prompt | Use Case | Key Features |
|--------|----------|--------------|
| `coding_standards.prompt.md` | General coding | Basic coding standards and practices |
| `code-review.prompt.md` | Code reviews | Comprehensive code review checklist with security focus |
| `feature-implementation.prompt.md` | New features | Step-by-step feature development workflow |
| `bug-fix.prompt.md` | Debugging | Systematic bug investigation and fixing process |
| `performance-optimization.prompt.md` | Performance | Database, caching, and performance improvements |
| `refactoring.prompt.md` | Code improvement | SOLID principles, design patterns, code smells |

### How to Use Prompts

1. Open the prompt file in VS Code
2. Read through the template
3. Copy the relevant sections
4. Paste into your chat with the AI assistant
5. Fill in your specific details

**Example:**
```
I need help with [feature/bug/optimization]

[Paste prompt template here]

Specific details:
- [Your requirement 1]
- [Your requirement 2]
```

## 🤖 Chat Modes

Specialized AI assistant personas with domain expertise. Activate these to get role-specific guidance.

### Available Chat Modes

| Chat Mode | Expertise | Best For |
|-----------|-----------|----------|
| `MVC Security Guardian.chatmode.md` | Security auditing | Security reviews, vulnerability detection |
| `ASP.NET Core Developer.chatmode.md` | Full-stack .NET development | General MVC development, architecture |
| `Database Architect.chatmode.md` | Database design & EF Core | Schema design, query optimization, migrations |
| `Frontend Developer.chatmode.md` | UI/UX, Razor, JavaScript | Views, responsive design, accessibility |
| `Testing Specialist.chatmode.md` | Testing & QA | Unit tests, integration tests, TDD |
| `DevOps Engineer.chatmode.md` | Azure deployment & CI/CD | Deployment, Docker, pipelines, monitoring |

### How to Use Chat Modes

1. **In GitHub Copilot Chat**: Reference the chat mode in your conversation
   ```
   @workspace Using the ASP.NET Core Developer chat mode, help me create a new controller
   ```

2. **Manual Activation**: Copy the chat mode content and paste it as context at the start of your conversation

3. **Combine with Prompts**: Use chat modes together with prompt templates for best results
   ```
   @workspace As a Database Architect, review this migration using the code-review prompt
   ```

## 🚀 Usage Examples

### Example 1: Implementing a New Feature

```markdown
@workspace Using the ASP.NET Core Developer chat mode

I need help implementing a new feature.

[Paste content from feature-implementation.prompt.md]

Feature Requirements:
- Add a Department entity
- Create CRUD operations
- Establish relationship with Employee
```

### Example 2: Security Review

```markdown
@workspace Using the MVC Security Guardian chat mode

Please review my recent changes for security issues.

[Paste content from code-review.prompt.md]

Focus areas:
- Authentication implementation
- Input validation
- SQL injection prevention
```

### Example 3: Database Optimization

```markdown
@workspace As a Database Architect

Help me optimize database queries.

[Paste content from performance-optimization.prompt.md]

Issues I'm seeing:
- Slow employee list page
- N+1 queries on details page
```

### Example 4: Bug Fixing

```markdown
@workspace As an ASP.NET Core Developer

I have a bug to fix.

[Paste content from bug-fix.prompt.md]

Bug Description:
- Edit employee form not saving changes
- No error messages displayed
- Happens only for certain employees
```

## 📝 Best Practices

### For Instructions Files

✅ **Do:**
- Keep instructions clear and concise
- Include code examples
- Focus on rules that apply consistently
- Update regularly as standards evolve
- Use glob patterns correctly for `applyTo`

❌ **Don't:**
- Make instructions too verbose
- Include project-specific details that change often
- Duplicate information across multiple instruction files
- Use instructions for one-time guidance

### For Prompt Files

✅ **Do:**
- Create templates for repeatable tasks
- Include checklists and structured sections
- Provide code examples
- Keep prompts focused on one task type
- Update based on what works well

❌ **Don't:**
- Make prompts too prescriptive
- Include outdated information
- Create prompts for rare, one-off tasks
- Copy-paste without understanding

### For Chat Modes

✅ **Do:**
- Define clear expertise areas
- Include relevant best practices
- Specify response style expectations
- Update with project context
- Create modes for distinct roles

❌ **Don't:**
- Make modes too similar to each other
- Include excessive project details that change
- Create modes for temporary needs
- Forget to update when frameworks change

## 🔄 Maintenance

### Regular Reviews (Monthly)

- [ ] Review instruction files for accuracy
- [ ] Update framework versions and best practices
- [ ] Add new prompts for common tasks
- [ ] Retire unused chat modes
- [ ] Gather team feedback on effectiveness

### When to Update

- **Immediately**: Security vulnerabilities or critical errors
- **Sprint/Iteration**: New patterns or standards adopted
- **Quarterly**: Framework updates (e.g., .NET version bump)
- **Annually**: Complete review and refresh

## 🎓 Learning Resources

### Understanding These Files

- **Instruction Files**: Automatically applied guidelines for code generation
- **Prompt Files**: Templates you actively use in conversations
- **Chat Modes**: Persona definitions for specialized assistance

### Related Documentation

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Azure Documentation](https://docs.microsoft.com/en-us/azure/)

## 🤝 Contributing

When adding new files:

1. **Instructions**: Add when you have rules that apply broadly across file types
2. **Prompts**: Add for repeatable workflows you use often
3. **Chat Modes**: Add for distinct roles/expertise areas

### File Naming Conventions

- **Instructions**: `[Topic]Instructions.instructions.md`
- **Prompts**: `[task-name].prompt.md` (lowercase with hyphens)
- **Chat Modes**: `[Role Name].chatmode.md` (Title Case with spaces)

## 📊 Metrics & Success

Track the effectiveness of these configurations:

- Fewer security vulnerabilities in code reviews
- More consistent code quality
- Faster feature development
- Better test coverage
- Reduced deployment issues

## 🐛 Troubleshooting

### Instructions Not Being Applied

- Check the `applyTo` glob pattern syntax
- Verify the file extension matches
- Ensure the frontmatter is properly formatted
- Check VS Code settings for GitHub Copilot

### Prompts Not Working Well

- Make prompts more specific
- Add more examples
- Break down complex prompts into smaller parts
- Combine with appropriate chat mode

### Chat Modes Not Effective

- Verify the role definition is clear
- Add more context about the project
- Include concrete examples
- Update expertise areas to match actual needs

---

## 📞 Support

For questions or issues:
1. Review this README
2. Check the individual file documentation
3. Consult team leads or architects
4. Update files based on learnings

---

**Last Updated**: October 27, 2025  
**Maintained By**: Development Team  
**Review Frequency**: Monthly or as needed
