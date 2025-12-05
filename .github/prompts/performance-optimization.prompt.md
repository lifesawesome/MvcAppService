# Performance Optimization Prompt

You are an expert ASP.NET Core MVC performance optimization specialist. I need help optimizing the application.

## Performance Audit Request

Please analyze the application for performance issues in the following areas:

## Areas to Investigate

### 1. Database Performance
- [ ] Identify N+1 query problems
- [ ] Check for missing indexes
- [ ] Review query execution plans
- [ ] Look for excessive data loading
- [ ] Check for inefficient LINQ queries
- [ ] Verify proper use of AsNoTracking()
- [ ] Review connection pooling configuration

### 2. Controller/Action Performance
- [ ] Identify blocking async calls
- [ ] Check for synchronous database calls
- [ ] Review large data processing in controllers
- [ ] Look for inefficient algorithms
- [ ] Verify proper use of async/await

### 3. View Rendering Performance
- [ ] Check for complex view logic
- [ ] Review partial view usage
- [ ] Identify excessive database calls in views
- [ ] Look for large data sets being passed to views
- [ ] Review ViewBag/ViewData usage

### 4. Caching Opportunities
- [ ] Identify frequently accessed data
- [ ] Look for static data that can be cached
- [ ] Review response caching opportunities
- [ ] Check for distributed cache usage
- [ ] Identify memory cache candidates

### 5. Asset Performance
- [ ] Check JavaScript/CSS bundle sizes
- [ ] Review image optimization
- [ ] Verify CDN usage for static assets
- [ ] Check for minification in production
- [ ] Review browser caching headers

### 6. API Performance (if applicable)
- [ ] Review response payload sizes
- [ ] Check for over-fetching data
- [ ] Look for missing compression
- [ ] Review serialization performance
- [ ] Check for proper HTTP caching

## Optimization Techniques

### Database Optimization

#### Use Projections
```csharp
// BEFORE: Loading entire entity
var employees = await _context.Employees.ToListAsync();

// AFTER: Project only needed fields
var employees = await _context.Employees
    .Select(e => new EmployeeListDto
    {
        Id = e.Id,
        Name = e.Name,
        Department = e.Department
    })
    .ToListAsync();
```

#### Implement Pagination
```csharp
// Add pagination to large datasets
public async Task<PagedResult<Employee>> GetEmployeesAsync(int page, int pageSize)
{
    var totalCount = await _context.Employees.CountAsync();
    
    var items = await _context.Employees
        .AsNoTracking()
        .OrderBy(e => e.Name)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    return new PagedResult<Employee>
    {
        Items = items,
        TotalCount = totalCount,
        PageNumber = page,
        PageSize = pageSize
    };
}
```

#### Fix N+1 Queries
```csharp
// BEFORE: N+1 queries
var employees = await _context.Employees.ToListAsync();
foreach (var emp in employees)
{
    var dept = await _context.Departments.FindAsync(emp.DepartmentId);
}

// AFTER: Single query with Include
var employees = await _context.Employees
    .Include(e => e.Department)
    .ToListAsync();
```

#### Add Indexes
```csharp
// In DbContext OnModelCreating
modelBuilder.Entity<Employee>()
    .HasIndex(e => e.Email);

modelBuilder.Entity<Employee>()
    .HasIndex(e => new { e.LastName, e.FirstName });
```

### Caching Implementation

#### Response Caching
```csharp
// Program.cs
builder.Services.AddResponseCaching();
app.UseResponseCaching();

// Controller
[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
public async Task<IActionResult> Index()
{
    var employees = await _context.Employees.ToListAsync();
    return View(employees);
}
```

#### Memory Cache
```csharp
public class EmployeeService
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationDbContext _context;
    
    public async Task<List<Department>> GetDepartmentsAsync()
    {
        return await _cache.GetOrCreateAsync("departments", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return await _context.Departments.ToListAsync();
        });
    }
}
```

#### Distributed Cache (Redis)
```csharp
// Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Usage
public class EmployeeService
{
    private readonly IDistributedCache _cache;
    
    public async Task<Employee> GetEmployeeAsync(int id)
    {
        var cacheKey = $"employee:{id}";
        var cached = await _cache.GetStringAsync(cacheKey);
        
        if (cached != null)
        {
            return JsonSerializer.Deserialize<Employee>(cached);
        }
        
        var employee = await _context.Employees.FindAsync(id);
        
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(employee),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });
        
        return employee;
    }
}
```

### Async/Await Optimization

```csharp
// BEFORE: Blocking call
public IActionResult Index()
{
    var employees = _context.Employees.ToList(); // Blocking!
    return View(employees);
}

// AFTER: Proper async
public async Task<IActionResult> Index()
{
    var employees = await _context.Employees.ToListAsync();
    return View(employees);
}
```

### Response Compression

```csharp
// Program.cs
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
});

app.UseResponseCompression();
```

### Asset Optimization

```cshtml
<!-- Bundle and minify in production -->
<environment include="Development">
    <link rel="stylesheet" href="~/css/site.css" />
</environment>
<environment exclude="Development">
    <link rel="stylesheet" href="~/css/site.min.css" asp-append-version="true" />
</environment>

<!-- Lazy load images -->
<img src="~/images/photo.jpg" loading="lazy" alt="Employee photo" />

<!-- Use CDN with fallback -->
<script src="https://cdn.jsdelivr.net/npm/jquery@3.6.0/dist/jquery.min.js"
        asp-fallback-src="~/lib/jquery/dist/jquery.min.js"
        asp-fallback-test="window.jQuery"
        crossorigin="anonymous"
        integrity="sha384-...">
</script>
```

## Performance Metrics to Monitor

- Response time (aim for < 200ms for most requests)
- Database query count per request
- Database query execution time
- Memory usage
- CPU usage
- Cache hit/miss ratio
- Time to First Byte (TTFB)
- Page load time
- Asset size

## Performance Testing Tools

- Application Insights (already configured)
- SQL Server Profiler
- Entity Framework Core Logging
- Browser DevTools
- Lighthouse
- BenchmarkDotNet for micro-benchmarks

## Deliverables

Please provide:

1. **Performance Analysis**: Detailed findings of performance bottlenecks
2. **Optimization Plan**: Prioritized list of optimizations
3. **Implementation**: Code changes for optimizations
4. **Before/After Metrics**: Performance comparison
5. **Monitoring Recommendations**: What to monitor going forward

## Optimization Priority

1. **High Impact, Low Effort**: Do these first
2. **High Impact, High Effort**: Plan and schedule these
3. **Low Impact, Low Effort**: Do when convenient
4. **Low Impact, High Effort**: Probably skip unless necessary

Let's make this app blazing fast! ⚡🚀
