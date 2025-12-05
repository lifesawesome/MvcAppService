---
applyTo: '**/{Data,Migrations,Models}/**'
---

# Database and Entity Framework Core Guidelines

## Database Architecture

### Technology Stack
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core 9.0
- **Migration Strategy**: Code-First with EF Core Migrations

## Entity Framework Core Best Practices

### DbContext Configuration

#### DbContext Class Structure
```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    // DbSets
    public DbSet<Employee> Employees { get; set; } = default!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Or configure inline
        ConfigureEmployee(modelBuilder);
    }
    
    private void ConfigureEmployee(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Department)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.Position)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("IX_Employee_Email");
        });
    }
}
```

### Entity Configuration Pattern

#### Using IEntityTypeConfiguration (Recommended for Complex Models)
```csharp
// Data/Configurations/EmployeeConfiguration.cs
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)");
            
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.HasIndex(e => e.Email)
            .IsUnique();
            
        builder.Property(e => e.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(e => e.Salary)
            .HasColumnType("decimal(18,2)")
            .HasPrecision(18, 2);
    }
}
```

### Model Design Guidelines

#### Entity Base Class
```csharp
public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime? ModifiedDate { get; set; }
    
    public string? CreatedBy { get; set; }
    
    public string? ModifiedBy { get; set; }
}
```

#### Audit Fields Pattern
```csharp
public interface IAuditable
{
    DateTime CreatedDate { get; set; }
    DateTime? ModifiedDate { get; set; }
    string? CreatedBy { get; set; }
    string? ModifiedBy { get; set; }
}

// In DbContext
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    var entries = ChangeTracker.Entries<IAuditable>();
    
    foreach (var entry in entries)
    {
        if (entry.State == EntityState.Added)
        {
            entry.Entity.CreatedDate = DateTime.UtcNow;
            entry.Entity.CreatedBy = _currentUserService.UserId;
        }
        else if (entry.State == EntityState.Modified)
        {
            entry.Entity.ModifiedDate = DateTime.UtcNow;
            entry.Entity.ModifiedBy = _currentUserService.UserId;
        }
    }
    
    return await base.SaveChangesAsync(cancellationToken);
}
```

### Query Optimization

#### Read-Only Queries
```csharp
// Use AsNoTracking for read-only operations
public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
{
    return await _context.Employees
        .AsNoTracking()
        .OrderBy(e => e.Name)
        .ToListAsync();
}
```

#### Projection for Performance
```csharp
// Project to DTOs/ViewModels to reduce data transfer
public async Task<IEnumerable<EmployeeListDto>> GetEmployeeListAsync()
{
    return await _context.Employees
        .AsNoTracking()
        .Select(e => new EmployeeListDto
        {
            Id = e.Id,
            Name = e.Name,
            Department = e.Department,
            Position = e.Position
        })
        .ToListAsync();
}
```

#### Eager Loading
```csharp
// Use Include for related data
public async Task<Employee?> GetEmployeeWithDepartmentAsync(int id)
{
    return await _context.Employees
        .Include(e => e.Department)
        .ThenInclude(d => d.Manager)
        .FirstOrDefaultAsync(e => e.Id == id);
}
```

#### Pagination
```csharp
public async Task<PagedResult<Employee>> GetEmployeesPagedAsync(
    int pageNumber, 
    int pageSize)
{
    var totalCount = await _context.Employees.CountAsync();
    
    var items = await _context.Employees
        .AsNoTracking()
        .OrderBy(e => e.Name)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    return new PagedResult<Employee>
    {
        Items = items,
        TotalCount = totalCount,
        PageNumber = pageNumber,
        PageSize = pageSize
    };
}
```

### Preventing Common Query Issues

#### N+1 Query Problem
```csharp
// BAD: N+1 queries
var employees = await _context.Employees.ToListAsync();
foreach (var employee in employees)
{
    var department = await _context.Departments
        .FirstOrDefaultAsync(d => d.Id == employee.DepartmentId); // N queries!
}

// GOOD: Single query with Include
var employees = await _context.Employees
    .Include(e => e.Department)
    .ToListAsync();
```

#### Select N+1 Problem
```csharp
// GOOD: Use projection to avoid loading unnecessary data
var employeeNames = await _context.Employees
    .Select(e => new { e.Id, e.Name })
    .ToListAsync();
```

### Migration Best Practices

#### Creating Migrations
```powershell
# Always provide descriptive names
dotnet ef migrations add AddEmployeeEmailColumn
dotnet ef migrations add CreateDepartmentTable
dotnet ef migrations add AddEmployeeDepartmentRelationship

# NOT: Add_Migration1, Update1, etc.
```

#### Migration File Structure
```csharp
public partial class AddEmployeeEmailColumn : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Email",
            table: "Employees",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");
            
        migrationBuilder.CreateIndex(
            name: "IX_Employees_Email",
            table: "Employees",
            column: "Email",
            unique: true);
    }
    
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Employees_Email",
            table: "Employees");
            
        migrationBuilder.DropColumn(
            name: "Email",
            table: "Employees");
    }
}
```

#### Migration Safety Rules
- ✅ Always review generated migrations before applying
- ✅ Test migrations on a copy of production data
- ✅ Create backup before running migrations in production
- ✅ Make migrations reversible (proper Down method)
- ✅ Handle data migration in separate SQL scripts if complex
- ❌ Never modify existing migrations that have been applied
- ❌ Never delete migrations that have been applied to production

#### Data Seeding in Migrations
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.InsertData(
        table: "Departments",
        columns: new[] { "Id", "Name" },
        values: new object[,]
        {
            { 1, "Engineering" },
            { 2, "Human Resources" },
            { 3, "Sales" }
        });
}
```

### Relationship Configuration

#### One-to-Many
```csharp
public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    // Navigation property
    public ICollection<Employee> Employees { get; set; }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    // Foreign key
    public int DepartmentId { get; set; }
    
    // Navigation property
    public Department Department { get; set; }
}

// Configuration
modelBuilder.Entity<Employee>()
    .HasOne(e => e.Department)
    .WithMany(d => d.Employees)
    .HasForeignKey(e => e.DepartmentId)
    .OnDelete(DeleteBehavior.Restrict);
```

#### Many-to-Many
```csharp
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public ICollection<Project> Projects { get; set; }
}

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public ICollection<Employee> Employees { get; set; }
}

// EF Core 5+ automatically creates join table
// For custom join table:
public class EmployeeProject
{
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    
    public DateTime AssignedDate { get; set; }
}

// Configuration
modelBuilder.Entity<EmployeeProject>()
    .HasKey(ep => new { ep.EmployeeId, ep.ProjectId });
```

### Concurrency Control

#### Optimistic Concurrency
```csharp
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    [Timestamp]
    public byte[] RowVersion { get; set; }
}

// Handling concurrency conflicts
try
{
    await _context.SaveChangesAsync();
}
catch (DbUpdateConcurrencyException ex)
{
    var entry = ex.Entries.Single();
    var databaseValues = await entry.GetDatabaseValuesAsync();
    
    if (databaseValues == null)
    {
        // Entity was deleted
        ModelState.AddModelError("", "Unable to save. The record was deleted by another user.");
    }
    else
    {
        // Entity was modified
        ModelState.AddModelError("", "The record was modified by another user. Please refresh and try again.");
    }
}
```

### Transaction Management

#### Explicit Transactions
```csharp
using var transaction = await _context.Database.BeginTransactionAsync();

try
{
    var employee = new Employee { Name = "John Doe" };
    _context.Employees.Add(employee);
    await _context.SaveChangesAsync();
    
    var audit = new AuditLog { Action = "Employee Created", EmployeeId = employee.Id };
    _context.AuditLogs.Add(audit);
    await _context.SaveChangesAsync();
    
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

### Database Indexes

#### Creating Indexes
```csharp
// Single column index
modelBuilder.Entity<Employee>()
    .HasIndex(e => e.Email)
    .IsUnique();

// Composite index
modelBuilder.Entity<Employee>()
    .HasIndex(e => new { e.LastName, e.FirstName });

// Include columns (SQL Server)
modelBuilder.Entity<Employee>()
    .HasIndex(e => e.Email)
    .IncludeProperties(e => new { e.Name, e.Department });

// Filtered index
modelBuilder.Entity<Employee>()
    .HasIndex(e => e.Email)
    .HasFilter("[IsActive] = 1");
```

### Value Conversions

#### Custom Value Converters
```csharp
// Enum to string conversion
modelBuilder.Entity<Employee>()
    .Property(e => e.Status)
    .HasConversion<string>();

// Custom conversion
var converter = new ValueConverter<DateTime, DateTime>(
    v => v.ToUniversalTime(),
    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

modelBuilder.Entity<Employee>()
    .Property(e => e.CreatedDate)
    .HasConversion(converter);
```

### Raw SQL Queries

#### Parameterized Raw SQL (Safe)
```csharp
// Use FromSqlRaw with parameters
var employees = await _context.Employees
    .FromSqlRaw("SELECT * FROM Employees WHERE Department = {0}", department)
    .ToListAsync();

// Or with named parameters
var employees = await _context.Employees
    .FromSqlInterpolated($"SELECT * FROM Employees WHERE Department = {department}")
    .ToListAsync();
```

#### Stored Procedures
```csharp
var department = "IT";
var employees = await _context.Employees
    .FromSqlRaw("EXEC GetEmployeesByDepartment @Department = {0}", department)
    .ToListAsync();
```

### Performance Monitoring

#### Query Logging
```csharp
// In Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }
});
```

### Database Connection Management

#### Connection String Security
```csharp
// NEVER hardcode connection strings
// Use User Secrets for development
// Use Azure Key Vault for production

// Program.cs
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```

#### Connection Resilience
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));
```

## Data Validation

### Model Validation
```csharp
public class Employee : IValidatableObject
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Range(18, 120)]
    public int Age { get; set; }
    
    // Custom validation
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Email.EndsWith("@tempmail.com"))
        {
            yield return new ValidationResult(
                "Temporary email addresses are not allowed.",
                new[] { nameof(Email) });
        }
    }
}
```

## Common Pitfalls to Avoid

- ❌ Using `Include()` without `AsNoTracking()` for read-only queries
- ❌ Loading entire tables into memory before filtering
- ❌ Not disposing DbContext properly (use DI with scoped lifetime)
- ❌ Executing queries in loops (N+1 problem)
- ❌ Using `FirstOrDefault()` when `SingleOrDefault()` is appropriate
- ❌ Not handling `DbUpdateException` and `DbUpdateConcurrencyException`
- ❌ Modifying entities tracked by different DbContext instances
- ❌ Using string concatenation for SQL queries

## AI Code Generation Rules

When generating database-related code:

1. ✅ Always use parameterized queries or LINQ
2. ✅ Use async/await for all database operations
3. ✅ Include proper error handling for database exceptions
4. ✅ Use AsNoTracking() for read-only queries
5. ✅ Implement proper entity configurations
6. ✅ Add appropriate indexes for frequently queried columns
7. ✅ Use projections to reduce data transfer
8. ✅ Handle concurrency conflicts appropriately
9. ✅ Follow migration naming conventions
10. ✅ Never expose connection strings or credentials

---

**Last Updated:** October 27, 2025  
**Review Frequency:** Quarterly  
**Document Owner:** Database Administrator / Lead Developer
