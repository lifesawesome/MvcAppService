---
applyTo: '**/*.cs'
---

# ASP.NET Core MVC Development Guidelines

## Project Overview
This is an ASP.NET Core 9.0 MVC application with Entity Framework Core, following standard MVC architecture patterns.

## Architecture Principles

### Model-View-Controller Pattern
- **Models**: Data entities and business logic (`Models/`)
- **Views**: Razor views for UI rendering (`Views/`)
- **Controllers**: Handle HTTP requests and coordinate between models and views (`Controllers/`)

### Folder Structure
```
MvcCrudApp/
├── Controllers/       # MVC Controllers
├── Models/           # Data models and entities
├── ViewModels/       # View-specific models
├── Views/            # Razor views
├── Services/         # Business logic services
├── Data/             # Database context and migrations
└── wwwroot/          # Static files (CSS, JS, images)
```

## Coding Standards

### Naming Conventions
- **Classes/Interfaces**: PascalCase (e.g., `EmployeeController`, `IEmployeeService`)
- **Methods**: PascalCase (e.g., `GetEmployeeById`)
- **Properties**: PascalCase (e.g., `FirstName`, `LastName`)
- **Private fields**: camelCase with underscore prefix (e.g., `_context`, `_logger`)
- **Parameters**: camelCase (e.g., `employeeId`, `userName`)
- **Local variables**: camelCase (e.g., `employee`, `result`)

### Controller Guidelines

#### Controller Structure
```csharp
public class EmployeeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmployeeController> _logger;
    
    public EmployeeController(
        ApplicationDbContext context,
        ILogger<EmployeeController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    // Actions follow RESTful conventions
    public async Task<IActionResult> Index() { }
    public async Task<IActionResult> Details(int? id) { }
    public IActionResult Create() { }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,...")] Employee employee) { }
    // ... Edit, Delete
}
```

#### Action Method Best Practices
- ✅ Use async/await for database operations
- ✅ Return appropriate IActionResult types
- ✅ Validate parameters (check for null)
- ✅ Use [HttpPost], [HttpGet] attributes explicitly
- ✅ Use [ValidateAntiForgeryToken] on POST/PUT/DELETE
- ✅ Use [Bind] attribute to prevent over-posting
- ✅ Return proper HTTP status codes (Ok, NotFound, BadRequest, etc.)

### Model Guidelines

#### Entity Models
```csharp
public class Employee
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Full Name")]
    public string Name { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Department { get; set; }
    
    [DataType(DataType.Date)]
    [Display(Name = "Hire Date")]
    public DateTime? HireDate { get; set; }
}
```

#### View Models
- Create separate ViewModels for complex views
- Don't expose entity models directly to views when they contain sensitive data
- Use ViewModels for forms that combine multiple entities

```csharp
public class EmployeeCreateViewModel
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Department { get; set; }
    
    public List<SelectListItem> Departments { get; set; }
}
```

### View Guidelines

#### Razor Syntax
```cshtml
@model MvcCrudApp.Models.Employee

@{
    ViewData["Title"] = "Employee Details";
}

<h2>@ViewData["Title"]</h2>

<div>
    <dl class="row">
        <dt class="col-sm-3">@Html.DisplayNameFor(model => model.Name)</dt>
        <dd class="col-sm-9">@Html.DisplayFor(model => model.Name)</dd>
    </dl>
</div>
```

#### Best Practices
- ✅ Use strongly-typed models (`@model TypeName`)
- ✅ Use Tag Helpers over HTML Helpers for new code
- ✅ Keep business logic out of views
- ✅ Use partial views for reusable UI components
- ✅ Use ViewData/ViewBag sparingly, prefer strongly-typed models
- ✅ Always HTML-encode user input (Razor does this automatically)

### Service Layer Pattern

#### When to Create Services
- Complex business logic
- Multiple data operations
- External API calls
- Reusable logic across controllers

```csharp
// IEmployeeService.cs
public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAllEmployeesAsync();
    Task<Employee?> GetEmployeeByIdAsync(int id);
    Task<bool> CreateEmployeeAsync(Employee employee);
    Task<bool> UpdateEmployeeAsync(Employee employee);
    Task<bool> DeleteEmployeeAsync(int id);
}

// EmployeeService.cs
public class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmployeeService> _logger;
    
    public EmployeeService(
        ApplicationDbContext context,
        ILogger<EmployeeService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await _context.Employees
            .OrderBy(e => e.Name)
            .ToListAsync();
    }
    
    // ... other methods
}

// Register in Program.cs
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
```

### Entity Framework Core Best Practices

#### DbContext
```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Employee> Employees { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure entity relationships and constraints
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });
    }
}
```

#### Query Best Practices
- ✅ Use async methods (`ToListAsync`, `FirstOrDefaultAsync`)
- ✅ Use `.AsNoTracking()` for read-only queries
- ✅ Project only needed columns with `.Select()`
- ✅ Use eager loading with `.Include()` when needed
- ✅ Avoid N+1 queries
- ❌ Never execute raw SQL with string concatenation

```csharp
// Good: Async with projection
var employees = await _context.Employees
    .AsNoTracking()
    .Select(e => new EmployeeListViewModel
    {
        Id = e.Id,
        Name = e.Name,
        Department = e.Department
    })
    .ToListAsync();

// Good: Eager loading
var employee = await _context.Employees
    .Include(e => e.Department)
    .FirstOrDefaultAsync(e => e.Id == id);
```

### Dependency Injection

#### Service Registration in Program.cs
```csharp
// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddTransient<IReportGenerator, ReportGenerator>();

// Built-in services
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
```

#### Service Lifetimes
- **Transient**: Created each time they're requested
- **Scoped**: Created once per HTTP request
- **Singleton**: Created once for application lifetime

### Error Handling

#### Global Error Handling
```csharp
// Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
```

#### Controller Error Handling
```csharp
public async Task<IActionResult> Delete(int? id)
{
    if (id == null)
    {
        _logger.LogWarning("Delete called with null id");
        return NotFound();
    }
    
    try
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            _logger.LogWarning("Employee not found: {Id}", id);
            return NotFound();
        }
        
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Employee deleted: {Id}", id);
        return RedirectToAction(nameof(Index));
    }
    catch (DbUpdateException ex)
    {
        _logger.LogError(ex, "Database error deleting employee: {Id}", id);
        ModelState.AddModelError("", "Unable to delete employee. Try again later.");
        return View();
    }
}
```

### Logging Best Practices

#### Structured Logging
```csharp
// Good
_logger.LogInformation("Employee created with ID {EmployeeId} by user {UserId}", 
    employee.Id, userId);

// Bad
_logger.LogInformation($"Employee created with ID {employee.Id} by user {userId}");
```

#### Log Levels
- **Trace**: Very detailed, typically only enabled in development
- **Debug**: Debug information, short-term usefulness
- **Information**: General information about application flow
- **Warning**: Abnormal or unexpected events
- **Error**: Errors and exceptions
- **Critical**: Critical failures requiring immediate attention

### Configuration Management

#### appsettings.json Structure
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": ""  // Load from secrets
  },
  "AppSettings": {
    "PageSize": 10,
    "MaxUploadSize": 5242880
  }
}
```

#### Accessing Configuration
```csharp
// Using IConfiguration
private readonly IConfiguration _configuration;

var pageSize = _configuration.GetValue<int>("AppSettings:PageSize");
var connectionString = _configuration.GetConnectionString("DefaultConnection");

// Using Options Pattern (preferred)
public class AppSettings
{
    public int PageSize { get; set; }
    public int MaxUploadSize { get; set; }
}

// Program.cs
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

// In controller
private readonly AppSettings _appSettings;

public HomeController(IOptions<AppSettings> appSettings)
{
    _appSettings = appSettings.Value;
}
```

### Testing Guidelines

#### Unit Test Structure
```csharp
[Fact]
public async Task Create_ReturnsViewResult_WhenModelStateIsInvalid()
{
    // Arrange
    var controller = new EmployeeController(_context, _logger);
    controller.ModelState.AddModelError("Name", "Required");
    var employee = new Employee();
    
    // Act
    var result = await controller.Create(employee);
    
    // Assert
    var viewResult = Assert.IsType<ViewResult>(result);
    Assert.Equal(employee, viewResult.Model);
}
```

### Performance Considerations

- ✅ Use async/await for I/O operations
- ✅ Implement pagination for large datasets
- ✅ Use response caching where appropriate
- ✅ Optimize database queries (avoid N+1, use projections)
- ✅ Use CDN for static assets
- ✅ Enable response compression
- ✅ Minimize view complexity

### Code Organization Rules

#### File Placement
- Controllers → `Controllers/`
- Models → `Models/`
- ViewModels → `ViewModels/`
- Services → `Services/`
- Data/Repositories → `Data/`
- Extensions → `Extensions/`
- Utilities → `Utilities/`

#### One Class Per File
- Each class should be in its own file
- File name should match the class name
- Use nested folders for logical grouping

### Documentation Requirements

#### XML Documentation
```csharp
/// <summary>
/// Creates a new employee record in the database.
/// </summary>
/// <param name="employee">The employee data to create.</param>
/// <returns>Redirects to Index on success, returns view on failure.</returns>
/// <exception cref="DbUpdateException">Thrown when database update fails.</exception>
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("Id,Name,...")] Employee employee)
{
    // Implementation
}
```

## AI Code Generation Rules

When generating or modifying code:

1. ✅ Follow all naming conventions
2. ✅ Include proper error handling and logging
3. ✅ Use async/await for database operations
4. ✅ Include data annotations on models
5. ✅ Add XML documentation for public methods
6. ✅ Follow security best practices (see SecurityInstructions.instructions.md)
7. ✅ Use dependency injection for all services
8. ✅ Include appropriate HTTP attributes on action methods
9. ✅ Validate all inputs
10. ✅ Return appropriate status codes

## Common Patterns

### Repository Pattern (Optional)
```csharp
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

### Unit of Work Pattern (Optional)
```csharp
public interface IUnitOfWork : IDisposable
{
    IEmployeeRepository Employees { get; }
    Task<int> CompleteAsync();
}
```

---

**Last Updated:** October 27, 2025  
**Review Frequency:** Monthly  
**Document Owner:** Development Team Lead
