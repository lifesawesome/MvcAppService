---
applyTo: '**'
---

# Security Guidelines - OWASP Best Practices for ASP.NET Core MVC Application

## Overview
This document provides security guidelines based on OWASP (Open Web Application Security Project) Top 10 principles for the MvcCrudApp. All code changes, reviews, and new features MUST adhere to these security standards.

---

## 1. INJECTION PREVENTION (OWASP A03:2021)

### SQL Injection Protection
- ✅ **ALWAYS** use Entity Framework Core parameterized queries (already implemented)
- ✅ **NEVER** concatenate user input directly into SQL queries
- ✅ Use LINQ queries or stored procedures with parameters only

**Required Implementation:**
```csharp
// CORRECT - Parameterized with EF Core
var employee = await _context.Employees.FirstOrDefaultAsync(m => m.Id == id);

// WRONG - Never do this
var query = $"SELECT * FROM Employees WHERE Id = {id}";
```

### Input Validation
- ✅ Validate all user inputs on server-side
- ✅ Use Data Annotations on models (`[Required]`, `[StringLength]`, `[Range]`, etc.)
- ✅ Implement custom validation attributes for complex scenarios
- ✅ Sanitize HTML input to prevent XSS

**Example:**
```csharp
public class Employee
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2)]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters")]
    public string Name { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Department { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Position { get; set; }
}
```

---

## 2. BROKEN AUTHENTICATION & SESSION MANAGEMENT (OWASP A07:2021)

### Authentication Requirements
- ✅ Implement ASP.NET Core Identity for user authentication
- ✅ Use multi-factor authentication (MFA) for sensitive operations
- ✅ Enforce strong password policies
- ✅ Implement account lockout after failed login attempts

**Implementation:**
```csharp
// Add to Program.cs
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});
```

### Session Security
- ✅ Use HTTPS only (already implemented with `UseHttpsRedirection()`)
- ✅ Set secure cookie flags (HttpOnly, Secure, SameSite)
- ✅ Implement session timeout
- ✅ Regenerate session IDs after login

---

## 3. SENSITIVE DATA EXPOSURE (OWASP A02:2021)

### **CRITICAL FIX REQUIRED**: Remove Hardcoded Secrets
**⚠️ SECURITY VIOLATION DETECTED in appsettings.json**

- ❌ **NEVER** commit connection strings with credentials to source control
- ❌ **NEVER** commit API keys, passwords, or secrets
- ✅ Use Azure Key Vault for all secrets (production)
- ✅ Use User Secrets for development
- ✅ Use environment variables for CI/CD

**Immediate Action Required:**
```powershell
# Remove appsettings.json from git tracking
git rm --cached MvcCrudApp/appsettings.json

# Add to .gitignore
appsettings.json
appsettings.*.json
!appsettings.Development.json.template
```

**Correct Configuration:**
```json
// appsettings.json (safe version)
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ApplicationInsights": {
    "ConnectionString": ""  // Load from Key Vault
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": ""  // Load from Key Vault
  }
}
```

### Data Encryption
- ✅ Encrypt sensitive data at rest using SQL Server TDE (Transparent Data Encryption)
- ✅ Use HTTPS/TLS for data in transit (already implemented)
- ✅ Never log sensitive information (passwords, credit cards, SSN)
- ✅ Use Data Protection API for encrypting cookies and tokens

**Implementation:**
```csharp
// Add to Program.cs
builder.Services.AddDataProtection()
    .PersistKeysToAzureBlobStorage(new Uri("blob-uri"))
    .ProtectKeysWithAzureKeyVault(new Uri("key-uri"), new DefaultAzureCredential());
```

---

## 4. XML EXTERNAL ENTITIES (XXE) & BROKEN ACCESS CONTROL (OWASP A01:2021)

### Access Control
- ✅ Implement role-based authorization
- ✅ Use `[Authorize]` attribute on controllers/actions
- ✅ Implement resource-based authorization for sensitive operations
- ✅ Verify user permissions at the service layer, not just UI

**Implementation:**
```csharp
[Authorize(Roles = "Admin,Manager")]
public class EmployeeController : Controller
{
    // Only authenticated users with Admin or Manager role can access
    
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        // Only Admins can delete
    }
    
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        // Public access
    }
}
```

### XXE Prevention
- ✅ Disable DTD processing when parsing XML
- ✅ Use secure XML parsers

```csharp
var settings = new XmlReaderSettings
{
    DtdProcessing = DtdProcessing.Prohibit,
    XmlResolver = null
};
```

---

## 5. SECURITY MISCONFIGURATION (OWASP A05:2021)

### Production Hardening Checklist
- ✅ Disable detailed error messages in production
- ✅ Remove development endpoints and debug settings
- ✅ Configure security headers
- ✅ Keep all dependencies up to date
- ✅ Remove unused features and frameworks

**Required Middleware Configuration:**
```csharp
// Program.cs - Add security headers
app.Use(async (context, next) =>
{
    // Security Headers
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
    
    // Content Security Policy
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self'; connect-src 'self'; frame-ancestors 'none'");
    
    // Remove server header
    context.Response.Headers.Remove("Server");
    context.Response.Headers.Remove("X-Powered-By");
    
    await next();
});

// HSTS
app.UseHsts();
app.UseHttpsRedirection();

// CORS - restrictive policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

---

## 6. CROSS-SITE SCRIPTING (XSS) PREVENTION (OWASP A03:2021)

### XSS Protection
- ✅ Razor views automatically HTML-encode output (already implemented)
- ✅ Use `@Html.Raw()` only for trusted content
- ✅ Implement Content Security Policy (CSP) headers
- ✅ Validate and sanitize all user inputs

**Best Practices:**
```cshtml
<!-- SAFE - Auto-encoded -->
<h3>@Model.Name</h3>

<!-- DANGEROUS - Only for trusted HTML -->
<div>@Html.Raw(Model.TrustedHtmlContent)</div>

<!-- USE THIS for user HTML content -->
@using Microsoft.AspNetCore.Html
<div>@Html.Encode(Model.UserContent)</div>
```

### JavaScript Security
- ✅ Use Subresource Integrity (SRI) for CDN resources
- ✅ Avoid inline JavaScript in production
- ✅ Sanitize JSON data rendered in views

---

## 7. CROSS-SITE REQUEST FORGERY (CSRF) (OWASP A04:2021)

### CSRF Protection
- ✅ **ALWAYS** use `[ValidateAntiForgeryToken]` on POST/PUT/DELETE actions (already implemented)
- ✅ Include anti-forgery tokens in all forms
- ✅ Use SameSite cookie attribute

**Required Implementation:**
```csharp
// Controller
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("Id,Name,Department,Position")] Employee employee)
{
    // Protected against CSRF
}

// View
@using (Html.BeginForm())
{
    @Html.AntiForgeryToken()
    <!-- form fields -->
}
```

---

## 8. INSECURE DESERIALIZATION (OWASP A08:2021)

### Serialization Security
- ✅ Validate deserialized objects
- ✅ Use type restrictions when deserializing
- ✅ Avoid deserializing untrusted data

**Safe Deserialization:**
```csharp
var options = new JsonSerializerOptions
{
    MaxDepth = 32,
    PropertyNameCaseInsensitive = false
};

// Deserialize with type checking
var employee = JsonSerializer.Deserialize<Employee>(json, options);
if (employee == null || !ModelState.IsValid)
{
    return BadRequest();
}
```

---

## 9. USING COMPONENTS WITH KNOWN VULNERABILITIES (OWASP A06:2021)

### Dependency Management
- ✅ Regularly update NuGet packages
- ✅ Monitor security advisories for dependencies
- ✅ Use `dotnet list package --vulnerable` to check for vulnerabilities
- ✅ Remove unused dependencies

**Regular Maintenance:**
```powershell
# Check for vulnerable packages
dotnet list package --vulnerable

# Update packages
dotnet outdated
dotnet add package PackageName --version LatestVersion
```

**Current Dependencies to Monitor:**
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.ApplicationInsights
- Azure.Identity

---

## 10. INSUFFICIENT LOGGING & MONITORING (OWASP A09:2021)

### Logging Requirements
- ✅ Log all authentication attempts (success and failure)
- ✅ Log authorization failures
- ✅ Log input validation failures
- ✅ Log application errors
- ❌ **NEVER** log sensitive data (passwords, tokens, PII)
- ✅ Implement centralized logging (Application Insights already configured)

**Implementation:**
```csharp
public class EmployeeController : Controller
{
    private readonly ILogger<EmployeeController> _logger;
    
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Delete attempt for non-existent employee ID: {EmployeeId}", id);
                return NotFound();
            }
            
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Employee deleted: ID={EmployeeId}, User={User}", 
                id, User.Identity?.Name);
            
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting employee ID: {EmployeeId}", id);
            throw;
        }
    }
}
```

### Monitoring & Alerting
- ✅ Configure Application Insights alerts for suspicious activity
- ✅ Monitor for unusual patterns (multiple failed logins, bulk data access)
- ✅ Implement real-time security event monitoring
- ✅ Set up automated incident response

---

## 11. SERVER-SIDE REQUEST FORGERY (SSRF) (OWASP A10:2021)

### SSRF Prevention
- ✅ Validate and sanitize all URLs
- ✅ Use allowlists for external resources
- ✅ Disable redirects on HTTP clients
- ✅ Use network segmentation

**Safe HTTP Client Usage:**
```csharp
var allowedHosts = new[] { "api.trusted-service.com", "api.partner.com" };

if (!allowedHosts.Contains(new Uri(url).Host))
{
    throw new SecurityException("Invalid host");
}
```

---

## 12. ADDITIONAL SECURITY BEST PRACTICES

### API Security (if implementing Web API)
- ✅ Use API versioning
- ✅ Implement rate limiting
- ✅ Use OAuth 2.0 / OpenID Connect for authentication
- ✅ Validate content-type headers
- ✅ Implement API keys management

### File Upload Security
- ✅ Validate file types and extensions
- ✅ Scan files for malware
- ✅ Limit file sizes
- ✅ Store files outside web root
- ✅ Use generated filenames, not user-provided names

**Example:**
```csharp
public async Task<IActionResult> Upload(IFormFile file)
{
    var allowedExtensions = new[] { ".jpg", ".png", ".pdf" };
    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    
    if (!allowedExtensions.Contains(extension))
    {
        return BadRequest("Invalid file type");
    }
    
    if (file.Length > 5 * 1024 * 1024) // 5MB limit
    {
        return BadRequest("File too large");
    }
    
    var fileName = $"{Guid.NewGuid()}{extension}";
    var path = Path.Combine(_uploadPath, fileName);
    
    using (var stream = new FileStream(path, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }
    
    return Ok();
}
```

### Database Security
- ✅ Use principle of least privilege for database accounts
- ✅ Enable database audit logging
- ✅ Use connection pooling securely
- ✅ Regularly backup and test restore procedures
- ✅ Encrypt backups

---

## 13. CODE REVIEW SECURITY CHECKLIST

When reviewing code or making changes, verify:

- [ ] No hardcoded secrets or credentials
- [ ] All user inputs are validated
- [ ] Authorization checks are in place
- [ ] Anti-forgery tokens used on state-changing operations
- [ ] Sensitive data is encrypted
- [ ] Errors are logged (without sensitive data)
- [ ] Dependencies are up to date
- [ ] Security headers are configured
- [ ] HTTPS is enforced
- [ ] SQL injection prevention via parameterized queries
- [ ] XSS prevention via output encoding
- [ ] Proper error handling (no stack traces to users)

---

## 14. ENVIRONMENT-SPECIFIC CONFIGURATIONS

### Development
```csharp
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // Use User Secrets for sensitive data
}
```

### Staging
```csharp
if (app.Environment.IsStaging())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    // Use Azure Key Vault
}
```

### Production
```csharp
if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    // Use Azure Key Vault
    // Enable all security headers
    // Enable rate limiting
    // Enable DDoS protection
}
```

---

## 15. INCIDENT RESPONSE PLAN

### Security Incident Procedures
1. **Detect**: Monitor Application Insights for anomalies
2. **Contain**: Disable affected endpoints/users
3. **Investigate**: Review logs and telemetry
4. **Remediate**: Apply patches/fixes
5. **Document**: Record incident details
6. **Review**: Post-mortem analysis

---

## IMMEDIATE ACTION ITEMS FOR THIS PROJECT

### 🔴 CRITICAL (Fix Immediately)
1. **Remove hardcoded database credentials from appsettings.json**
2. **Move all secrets to Azure Key Vault**
3. **Update .gitignore to exclude appsettings.json**
4. **Rotate compromised database credentials**

### 🟡 HIGH PRIORITY (Fix Within 1 Week)
1. Implement authentication and authorization
2. Add comprehensive input validation to Employee model
3. Configure security headers middleware
4. Implement rate limiting
5. Add comprehensive logging for security events

### 🟢 MEDIUM PRIORITY (Fix Within 1 Month)
1. Implement API versioning (if applicable)
2. Add automated security testing to CI/CD
3. Set up security monitoring and alerting
4. Document security architecture
5. Conduct security training for development team

---

## SECURITY TESTING REQUIREMENTS

### Automated Testing
- ✅ Include security tests in CI/CD pipeline
- ✅ Use static code analysis (e.g., Security Code Scan)
- ✅ Perform dependency vulnerability scanning
- ✅ Run SAST (Static Application Security Testing)

### Manual Testing
- ✅ Penetration testing annually
- ✅ Code review for all PRs with security focus
- ✅ Threat modeling for new features

---

## COMPLIANCE & STANDARDS

This application should comply with:
- ✅ OWASP Top 10 (2021)
- ✅ OWASP ASVS (Application Security Verification Standard)
- ✅ GDPR (if handling EU citizen data)
- ✅ SOC 2 (if applicable)
- ✅ PCI DSS (if handling payment data)

---

## RESOURCES & REFERENCES

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [OWASP Cheat Sheet Series](https://cheatsheetseries.owasp.org/)
- [ASP.NET Core Security Documentation](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [Azure Security Best Practices](https://docs.microsoft.com/en-us/azure/security/)

---

## AI CODE GENERATION RULES

When generating code for this project, AI assistants MUST:

1. ✅ Never generate hardcoded secrets or credentials
2. ✅ Always include input validation
3. ✅ Always use parameterized queries (EF Core)
4. ✅ Always include `[ValidateAntiForgeryToken]` on state-changing actions
5. ✅ Always include authorization checks
6. ✅ Always include appropriate error handling and logging
7. ✅ Always encode user output to prevent XSS
8. ✅ Follow the principle of least privilege
9. ✅ Include security-related comments for complex logic
10. ✅ Suggest security improvements when reviewing code

---

**Last Updated:** October 23, 2025  
**Review Frequency:** Quarterly or after any security incident  
**Document Owner:** Security Team / Lead Developer