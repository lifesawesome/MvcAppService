---
agent: ask
model: Claude Opus 4.5 (Preview) (copilot)
description: "Enforce comprehensive coding standards for ASP.NET Core MVC development with C#, Entity Framework, and Azure integration."
---

# Task
Enforce and validate coding standards across the MvcCrudApp solution to ensure consistency, maintainability, security, and best practices.

## Requirements

### 1. C# Coding Conventions
- **Naming Conventions**:
  - Use PascalCase for classes, methods, properties, and public fields
  - Use camelCase for private fields, local variables, and parameters
  - Prefix private fields with underscore (`_context`, `_logger`)
  - Use meaningful, descriptive names (avoid abbreviations except common ones like `Id`, `Db`)
  - Interface names must start with `I` (e.g., `IEmployeeRepository`)
  
- **Code Organization**:
  - One class per file (unless nested classes)
  - File name must match the class name
  - Organize using statements alphabetically and remove unused ones
  - Keep methods under 50 lines (extract helper methods if needed)
  - Maximum class size: 300 lines (refactor if exceeded)

- **Nullable Reference Types**:
  - Always use `#nullable enable` in all C# files
  - Use `?` for nullable reference types appropriately
  - Validate null checks before dereferencing

### 2. ASP.NET Core MVC Structure
- **Controllers**:
  - Inherit from `Controller` base class
  - Use attribute routing or conventional routing consistently
  - Apply `[ValidateAntiForgeryToken]` to all POST/PUT/DELETE actions
  - Use async/await for all I/O operations (database, file, network)
  - Return appropriate HTTP status codes (`Ok()`, `NotFound()`, `BadRequest()`)
  - Add XML documentation comments for public actions

- **Models**:
  - Use Data Annotations for validation (`[Required]`, `[StringLength]`, `[Range]`)
  - Separate domain models from ViewModels/DTOs
  - Place entity models in `Models/` folder
  - Place ViewModels in `ViewModels/` folder
  - Use proper property types (avoid strings for dates, numbers)

- **Views**:
  - Use Razor syntax consistently
  - Leverage Tag Helpers over HTML Helpers
  - Keep business logic out of views
  - Use partial views for reusable components (`_PartialName.cshtml`)
  - Use ViewModels instead of ViewBag/ViewData where possible
  - Always HTML-encode user input to prevent XSS

### 3. Entity Framework Core Best Practices
- **DbContext**:
  - Configure using `OnModelCreating` for complex configurations
  - Use dependency injection for DbContext
  - Dispose properly (framework handles this via DI)
  - Use connection strings from configuration, never hardcode

- **Queries**:
  - Always use async methods (`ToListAsync()`, `FirstOrDefaultAsync()`)
  - Use `AsNoTracking()` for read-only queries
  - Avoid N+1 queries (use `.Include()` for eager loading)
  - Use projections (`.Select()`) to fetch only needed data
  - Never use string interpolation for SQL (use parameterized queries)

- **Migrations**:
  - Use meaningful migration names describing the change
  - Review generated migrations before applying
  - Never edit applied migrations (create new ones)

### 4. Dependency Injection
- **Service Registration**:
  - Register services in `Program.cs` during startup
  - Use appropriate lifetime: Singleton, Scoped, or Transient
  - DbContext should be Scoped
  - Stateless services can be Singleton
  - Services with per-request state should be Scoped

- **Constructor Injection**:
  - Inject dependencies via constructors only
  - Store injected dependencies in private readonly fields
  - Avoid service locator pattern

### 5. Error Handling & Logging
- **Exception Handling**:
  - Use try-catch only for expected exceptions
  - Don't catch generic `Exception` unless necessary
  - Log exceptions with context information
  - Don't expose stack traces to end users
  - Use custom error pages for production

- **Logging**:
  - Inject `ILogger<T>` for logging
  - Use appropriate log levels (Debug, Information, Warning, Error, Critical)
  - Log meaningful messages with structured data
  - Never log sensitive information (passwords, tokens, PII)
  - Use Application Insights for telemetry tracking

### 6. Security Standards
- **Authentication & Authorization**:
  - Apply `[Authorize]` attribute to protected controllers/actions
  - Use `[AllowAnonymous]` explicitly for public actions in protected controllers
  - Implement role-based or claims-based authorization when needed

- **Data Protection**:
  - Never store secrets in code or appsettings.json (use Azure Key Vault)
  - Use HTTPS for all production environments
  - Enable CSRF protection (built-in with anti-forgery tokens)
  - Validate and sanitize all user inputs
  - Use parameterized queries (EF Core handles this)

- **Configuration**:
  - Use `appsettings.Development.json` for development-only settings
  - Use environment variables or Key Vault for production secrets
  - Never commit sensitive data to version control

### 7. Asynchronous Programming
- **Async/Await Rules**:
  - Use `async`/`await` for I/O-bound operations
  - Return `Task<T>` or `Task` from async methods
  - Avoid `async void` (except for event handlers)
  - Don't use `.Result` or `.Wait()` (causes deadlocks)
  - Use `ConfigureAwait(false)` in library code (not needed in ASP.NET Core)

### 8. Performance & Optimization
- **Database**:
  - Use indexes on frequently queried columns
  - Avoid loading entire entities when only IDs are needed
  - Batch operations when possible
  - Use caching for frequently accessed static data

- **View Performance**:
  - Minimize database calls in views
  - Use ViewModels with necessary data only
  - Enable response caching where appropriate
  - Compress static assets (CSS, JS)

### 9. Testing Standards
- **Unit Tests**:
  - Test business logic and validation rules
  - Mock dependencies (DbContext, external services)
  - Use naming pattern: `MethodName_Scenario_ExpectedResult`
  - Aim for 80%+ code coverage on business logic

- **Integration Tests**:
  - Test controller actions end-to-end
  - Use in-memory database for testing
  - Verify HTTP responses and status codes

### 10. Code Comments & Documentation
- **XML Documentation**:
  - Add XML comments for public classes, methods, and properties
  - Use `<summary>`, `<param>`, `<returns>`, `<exception>` tags
  - Enable XML documentation file generation in project settings

- **Inline Comments**:
  - Comment "why" not "what" (code should be self-documenting)
  - Avoid obvious comments
  - Update comments when code changes
  - Remove commented-out code (use version control instead)

### 11. Azure Integration
- **Application Insights**:
  - Track custom events for critical operations
  - Monitor performance and exceptions
  - Use correlation IDs for distributed tracing
  
- **Key Vault**:
  - Load secrets at application startup
  - Use Managed Identity for authentication
  - Cache secrets appropriately

- **Deployment**:
  - Use CI/CD pipelines (Azure DevOps, GitHub Actions)
  - Separate configurations for staging and production
  - Validate deployment artifacts before release

## Code Examples

### ✅ Good Example - Controller Action
```csharp
/// <summary>
/// Creates a new employee record
/// </summary>
/// <param name="viewModel">Employee creation data</param>
/// <returns>Redirect to index or validation errors</returns>
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(CreateEmployeeViewModel viewModel)
{
    if (!ModelState.IsValid)
    {
        return View(viewModel);
    }

    var employee = new Employee
    {
        Name = viewModel.Name,
        Department = viewModel.Department,
        Position = viewModel.Position
    };

    _context.Employees.Add(employee);
    await _context.SaveChangesAsync();
    
    _telemetryClient.TrackEvent("EmployeeCreated", new Dictionary<string, string>
    {
        { "Department", employee.Department }
    });
    
    _logger.LogInformation("Employee {EmployeeId} created successfully", employee.Id);
    
    return RedirectToAction(nameof(Index));
}
```

### ❌ Bad Example - Avoid This
```csharp
// Missing XML docs, no async, hardcoded strings, poor error handling
[HttpPost]
public IActionResult Create([Bind("Id,Name,Department,Position")] Employee e)
{
    _context.Employees.Add(e);
    _context.SaveChanges(); // Blocking call!
    return RedirectToAction("Index"); // String literals
}
```

## Tools for Enforcement
- **StyleCop Analyzers**: Enforce C# coding conventions
- **SonarLint/SonarQube**: Detect code smells and bugs
- **Roslyn Analyzers**: Built-in code analysis
- **.editorconfig**: Configure code style rules
- **Roslynator**: Additional refactorings and analyzers
- **Security Code Scan**: Detect security vulnerabilities

## Success Criteria
- ✅ All code passes StyleCop analysis without warnings
- ✅ Zero critical/high severity issues in SonarQube
- ✅ No hardcoded secrets detected by secret scanning tools
- ✅ 100% of controllers use async/await for database operations
- ✅ All POST actions have anti-forgery token validation
- ✅ XML documentation coverage > 90% for public APIs
- ✅ Unit test coverage > 80% for business logic
- ✅ All database queries use EF Core (no raw SQL strings)
- ✅ Consistent naming conventions throughout solution
- ✅ No compiler warnings in Release builds

## Automated Checks
Run these commands before committing code:

```powershell
# Build with warnings as errors
dotnet build --configuration Release /warnaserror

# Run code analyzers
dotnet format --verify-no-changes

# Check for vulnerable packages
dotnet list package --vulnerable

# Run tests
dotnet test --configuration Release
```

## Continuous Improvement
- Conduct monthly code reviews
- Update standards based on team feedback
- Adopt new .NET features as framework updates
- Refactor legacy code incrementally
- Share knowledge through documentation and training

---

**Remember**: Consistent code is easier to read, maintain, and debug. These standards ensure quality and longevity of the codebase.