# Refactoring & Code Improvement Prompt

You are an expert software architect specializing in code refactoring, design patterns, and code quality improvement for ASP.NET Core applications.

## Refactoring Request

Please analyze the code and suggest improvements in the following areas:

## Code Quality Analysis

### 1. SOLID Principles
- [ ] **Single Responsibility**: Does each class have one reason to change?
- [ ] **Open/Closed**: Is code open for extension but closed for modification?
- [ ] **Liskov Substitution**: Can derived classes substitute base classes?
- [ ] **Interface Segregation**: Are interfaces focused and minimal?
- [ ] **Dependency Inversion**: Do high-level modules depend on abstractions?

### 2. Code Smells to Identify
- [ ] Long methods (> 20 lines)
- [ ] Large classes (> 200 lines)
- [ ] Duplicate code
- [ ] Magic numbers/strings
- [ ] Deep nesting (> 3 levels)
- [ ] Tight coupling
- [ ] God objects
- [ ] Feature envy
- [ ] Inappropriate intimacy
- [ ] Primitive obsession

### 3. Design Patterns Opportunities
- [ ] Repository pattern for data access
- [ ] Unit of Work for transaction management
- [ ] Factory pattern for object creation
- [ ] Strategy pattern for algorithms
- [ ] Decorator pattern for extending behavior
- [ ] Observer pattern for event handling
- [ ] Dependency Injection (already used)

## Refactoring Techniques

### Extract Method
```csharp
// BEFORE: Long method
public async Task<IActionResult> Create(Employee employee)
{
    if (!ModelState.IsValid)
        return View(employee);
    
    if (await _context.Employees.AnyAsync(e => e.Email == employee.Email))
    {
        ModelState.AddModelError("Email", "Email already exists");
        return View(employee);
    }
    
    employee.CreatedDate = DateTime.UtcNow;
    employee.CreatedBy = User.Identity.Name;
    
    _context.Employees.Add(employee);
    await _context.SaveChangesAsync();
    
    _logger.LogInformation("Employee created: {Id}", employee.Id);
    
    return RedirectToAction(nameof(Index));
}

// AFTER: Extracted methods
public async Task<IActionResult> Create(Employee employee)
{
    if (!ModelState.IsValid)
        return View(employee);
    
    if (!await IsEmailUniqueAsync(employee.Email))
    {
        ModelState.AddModelError("Email", "Email already exists");
        return View(employee);
    }
    
    SetAuditFields(employee);
    
    await SaveEmployeeAsync(employee);
    
    return RedirectToAction(nameof(Index));
}

private async Task<bool> IsEmailUniqueAsync(string email)
{
    return !await _context.Employees.AnyAsync(e => e.Email == email);
}

private void SetAuditFields(Employee employee)
{
    employee.CreatedDate = DateTime.UtcNow;
    employee.CreatedBy = User.Identity.Name;
}

private async Task SaveEmployeeAsync(Employee employee)
{
    _context.Employees.Add(employee);
    await _context.SaveChangesAsync();
    _logger.LogInformation("Employee created: {Id}", employee.Id);
}
```

### Introduce Service Layer
```csharp
// BEFORE: Business logic in controller
public class EmployeeController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public async Task<IActionResult> Create(Employee employee)
    {
        // Validation logic
        // Business rules
        // Database operations
        // Logging
    }
}

// AFTER: Service layer
public interface IEmployeeService
{
    Task<Result<Employee>> CreateEmployeeAsync(Employee employee, string userId);
}

public class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmployeeService> _logger;
    
    public async Task<Result<Employee>> CreateEmployeeAsync(Employee employee, string userId)
    {
        if (!await IsEmailUniqueAsync(employee.Email))
        {
            return Result<Employee>.Failure("Email already exists");
        }
        
        employee.CreatedDate = DateTime.UtcNow;
        employee.CreatedBy = userId;
        
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Employee created: {Id}", employee.Id);
        
        return Result<Employee>.Success(employee);
    }
}

public class EmployeeController : Controller
{
    private readonly IEmployeeService _employeeService;
    
    public async Task<IActionResult> Create(Employee employee)
    {
        if (!ModelState.IsValid)
            return View(employee);
        
        var result = await _employeeService.CreateEmployeeAsync(employee, User.Identity.Name);
        
        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error);
            return View(employee);
        }
        
        return RedirectToAction(nameof(Index));
    }
}
```

### Replace Magic Strings with Constants
```csharp
// BEFORE: Magic strings
return RedirectToAction("Index");
ViewData["Message"] = "Success";
_logger.LogInformation("Employee created");

// AFTER: Constants
public static class ActionNames
{
    public const string Index = nameof(Index);
    public const string Create = nameof(Create);
    public const string Edit = nameof(Edit);
}

public static class ViewDataKeys
{
    public const string Message = nameof(Message);
    public const string ErrorMessage = nameof(ErrorMessage);
}

public static class LogMessages
{
    public const string EmployeeCreated = "Employee created: {Id}";
    public const string EmployeeUpdated = "Employee updated: {Id}";
}

return RedirectToAction(ActionNames.Index);
ViewData[ViewDataKeys.Message] = "Success";
_logger.LogInformation(LogMessages.EmployeeCreated, employee.Id);
```

### Introduce Result Pattern
```csharp
// Result class for operation outcomes
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T Value { get; private set; }
    public string Error { get; private set; }
    
    public static Result<T> Success(T value) => new Result<T> 
    { 
        IsSuccess = true, 
        Value = value 
    };
    
    public static Result<T> Failure(string error) => new Result<T> 
    { 
        IsSuccess = false, 
        Error = error 
    };
}

// Usage
public async Task<Result<Employee>> GetEmployeeAsync(int id)
{
    var employee = await _context.Employees.FindAsync(id);
    
    if (employee == null)
        return Result<Employee>.Failure("Employee not found");
    
    return Result<Employee>.Success(employee);
}
```

### Use Repository Pattern
```csharp
// Generic repository interface
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

// Implementation
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }
    
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    
    // ... other methods
}

// Specific repository
public interface IEmployeeRepository : IRepository<Employee>
{
    Task<IEnumerable<Employee>> GetByDepartmentAsync(string department);
}

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext context) : base(context) { }
    
    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(string department)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.Department == department)
            .ToListAsync();
    }
}
```

### Introduce ViewModels
```csharp
// BEFORE: Using entity directly in view
public async Task<IActionResult> Edit(int id)
{
    var employee = await _context.Employees.FindAsync(id);
    return View(employee);
}

// AFTER: Using ViewModel
public class EmployeeEditViewModel
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [Required]
    public string Department { get; set; }
    
    public List<SelectListItem> Departments { get; set; }
}

public async Task<IActionResult> Edit(int id)
{
    var employee = await _context.Employees.FindAsync(id);
    if (employee == null) return NotFound();
    
    var viewModel = new EmployeeEditViewModel
    {
        Id = employee.Id,
        Name = employee.Name,
        Department = employee.Department,
        Departments = await GetDepartmentListAsync()
    };
    
    return View(viewModel);
}
```

### Remove Code Duplication
```csharp
// BEFORE: Duplicated code
public async Task<IActionResult> Details(int? id)
{
    if (id == null)
    {
        _logger.LogWarning("Details called with null id");
        return NotFound();
    }
    
    var employee = await _context.Employees.FindAsync(id);
    if (employee == null)
    {
        _logger.LogWarning("Employee not found: {Id}", id);
        return NotFound();
    }
    
    return View(employee);
}

public async Task<IActionResult> Edit(int? id)
{
    if (id == null)
    {
        _logger.LogWarning("Edit called with null id");
        return NotFound();
    }
    
    var employee = await _context.Employees.FindAsync(id);
    if (employee == null)
    {
        _logger.LogWarning("Employee not found: {Id}", id);
        return NotFound();
    }
    
    return View(employee);
}

// AFTER: Extracted common method
private async Task<IActionResult> GetEmployeeOrNotFoundAsync(
    int? id, 
    Func<Employee, IActionResult> onSuccess)
{
    if (id == null)
    {
        _logger.LogWarning("Action called with null id");
        return NotFound();
    }
    
    var employee = await _context.Employees.FindAsync(id);
    if (employee == null)
    {
        _logger.LogWarning("Employee not found: {Id}", id);
        return NotFound();
    }
    
    return onSuccess(employee);
}

public async Task<IActionResult> Details(int? id)
{
    return await GetEmployeeOrNotFoundAsync(id, employee => View(employee));
}

public async Task<IActionResult> Edit(int? id)
{
    return await GetEmployeeOrNotFoundAsync(id, employee => View(employee));
}
```

## Refactoring Checklist

### Before Refactoring
- [ ] Ensure all tests pass
- [ ] Commit current working code
- [ ] Understand the code fully
- [ ] Identify the smell or issue
- [ ] Plan the refactoring steps

### During Refactoring
- [ ] Make small, incremental changes
- [ ] Run tests after each change
- [ ] Commit working changes frequently
- [ ] Don't change behavior
- [ ] Keep code compiling

### After Refactoring
- [ ] Verify all tests still pass
- [ ] Add new tests if needed
- [ ] Review code quality metrics
- [ ] Update documentation
- [ ] Get peer review

## Code Quality Tools

- **Static Analysis**: Use analyzers (Roslyn, SonarLint)
- **Code Metrics**: Measure complexity, maintainability
- **Coverage**: Ensure refactored code is tested
- **Performance**: Profile before and after
- **Security**: Run security scans

## Deliverables

Please provide:

1. **Analysis**: Identified code smells and issues
2. **Refactoring Plan**: Step-by-step improvement plan
3. **Refactored Code**: Improved implementation
4. **Tests**: Updated or new tests
5. **Explanation**: Why changes improve the code
6. **Metrics**: Before/after comparison

## Refactoring Priorities

1. **Critical**: Security issues, bugs masked by complexity
2. **High**: Code that changes frequently, hard to test
3. **Medium**: Code smells, minor duplication
4. **Low**: Cosmetic improvements, naming conventions

Let's improve this code! 🔧✨
