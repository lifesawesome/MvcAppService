---
applyTo: '**/*.{cshtml,css,js}'
---

# Frontend Development Guidelines for ASP.NET Core MVC

## Overview
Guidelines for Razor views, HTML, CSS, JavaScript, and frontend best practices in the MvcCrudApp.

## Razor View Best Practices

### View Structure
```cshtml
@model MvcCrudApp.Models.Employee

@{
    ViewData["Title"] = "Employee Details";
    var pageTitle = "Employee Information";
}

@section Styles {
    <link rel="stylesheet" href="~/css/employee.css" />
}

<div class="container mt-4">
    <h2>@pageTitle</h2>
    
    <div class="card">
        <div class="card-body">
            <!-- Content -->
        </div>
    </div>
</div>

@section Scripts {
    <script src="~/js/employee.js"></script>
    @await Html.PartialAsync("_ValidationScriptsPartial")
}
```

### Tag Helpers (Preferred)

#### Form Tag Helpers
```cshtml
<!-- Use Tag Helpers instead of HTML Helpers -->
<form asp-action="Create" asp-controller="Employee" method="post">
    <div class="form-group">
        <label asp-for="Name" class="control-label"></label>
        <input asp-for="Name" class="form-control" />
        <span asp-validation-for="Name" class="text-danger"></span>
    </div>
    
    <div class="form-group">
        <label asp-for="Department" class="control-label"></label>
        <select asp-for="Department" asp-items="@ViewBag.Departments" class="form-control">
            <option value="">-- Select Department --</option>
        </select>
        <span asp-validation-for="Department" class="text-danger"></span>
    </div>
    
    <button type="submit" class="btn btn-primary">Create</button>
    <a asp-action="Index" class="btn btn-secondary">Cancel</a>
</form>
```

#### Anchor Tag Helpers
```cshtml
<!-- Navigation -->
<a asp-action="Index" asp-controller="Employee" class="btn btn-primary">
    All Employees
</a>

<a asp-action="Details" asp-route-id="@Model.Id" class="btn btn-info">
    View Details
</a>

<a asp-action="Edit" asp-route-id="@Model.Id" class="btn btn-warning">
    Edit
</a>
```

#### Image Tag Helper (with cache busting)
```cshtml
<img src="~/images/logo.png" asp-append-version="true" alt="Company Logo" />
```

#### Environment Tag Helper
```cshtml
<environment include="Development">
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.css" />
    <link rel="stylesheet" href="~/css/site.css" />
</environment>
<environment exclude="Development">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css"
          asp-fallback-href="~/lib/bootstrap/dist/css/bootstrap.min.css"
          asp-fallback-test-class="sr-only" 
          asp-fallback-test-property="position" 
          asp-fallback-test-value="absolute"
          crossorigin="anonymous"
          integrity="sha384-..." />
</environment>
```

### Partial Views

#### Creating Partial Views
```cshtml
<!-- _EmployeeCard.cshtml -->
@model MvcCrudApp.Models.Employee

<div class="card mb-3">
    <div class="card-header">
        <h5>@Model.Name</h5>
    </div>
    <div class="card-body">
        <p><strong>Department:</strong> @Model.Department</p>
        <p><strong>Position:</strong> @Model.Position</p>
    </div>
    <div class="card-footer">
        <a asp-action="Details" asp-route-id="@Model.Id" class="btn btn-sm btn-info">Details</a>
        <a asp-action="Edit" asp-route-id="@Model.Id" class="btn btn-sm btn-warning">Edit</a>
    </div>
</div>
```

#### Using Partial Views
```cshtml
<!-- Synchronous -->
@await Html.PartialAsync("_EmployeeCard", employee)

<!-- With ViewData -->
@await Html.PartialAsync("_EmployeeCard", employee, new ViewDataDictionary(ViewData) 
{ 
    { "ShowActions", true } 
})

<!-- In a loop -->
@foreach (var employee in Model.Employees)
{
    @await Html.PartialAsync("_EmployeeCard", employee)
}
```

### View Components

#### Creating View Component
```csharp
// ViewComponents/EmployeeStatsViewComponent.cs
public class EmployeeStatsViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;
    
    public EmployeeStatsViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IViewComponentResult> InvokeAsync(string department)
    {
        var stats = await _context.Employees
            .Where(e => e.Department == department)
            .GroupBy(e => e.Position)
            .Select(g => new PositionStats
            {
                Position = g.Key,
                Count = g.Count()
            })
            .ToListAsync();
            
        return View(stats);
    }
}
```

#### View Component View
```cshtml
<!-- Views/Shared/Components/EmployeeStats/Default.cshtml -->
@model IEnumerable<PositionStats>

<div class="stats-widget">
    <h4>Employee Statistics</h4>
    <ul>
        @foreach (var stat in Model)
        {
            <li>@stat.Position: @stat.Count</li>
        }
    </ul>
</div>
```

#### Using View Component
```cshtml
@await Component.InvokeAsync("EmployeeStats", new { department = "IT" })
```

### Layout and Sections

#### _Layout.cshtml Best Practices
```cshtml
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - MvcCrudApp</title>
    
    <!-- CSS -->
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
    
    @await RenderSectionAsync("Styles", required: false)
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
            <div class="container">
                <a class="navbar-brand" asp-area="" asp-controller="Home" asp-action="Index">MvcCrudApp</a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-controller="Home" asp-action="Index">Home</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-controller="Employee" asp-action="Index">Employees</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>
    
    <main role="main" class="pb-3">
        @RenderBody()
    </main>
    
    <footer class="border-top footer text-muted">
        <div class="container">
            &copy; @DateTime.Now.Year - MvcCrudApp
        </div>
    </footer>
    
    <!-- JavaScript -->
    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

### Data Display

#### Display Templates
```cshtml
<!-- Views/Shared/DisplayTemplates/Employee.cshtml -->
@model MvcCrudApp.Models.Employee

<dl class="row">
    <dt class="col-sm-3">@Html.DisplayNameFor(model => model.Name)</dt>
    <dd class="col-sm-9">@Html.DisplayFor(model => model.Name)</dd>
    
    <dt class="col-sm-3">@Html.DisplayNameFor(model => model.Department)</dt>
    <dd class="col-sm-9">@Html.DisplayFor(model => model.Department)</dd>
    
    <dt class="col-sm-3">@Html.DisplayNameFor(model => model.Position)</dt>
    <dd class="col-sm-9">@Html.DisplayFor(model => model.Position)</dd>
</dl>

<!-- Usage -->
@Html.DisplayFor(model => model.Employee)
```

#### Editor Templates
```cshtml
<!-- Views/Shared/EditorTemplates/Employee.cshtml -->
@model MvcCrudApp.Models.Employee

<div class="form-group">
    <label asp-for="Name" class="control-label"></label>
    <input asp-for="Name" class="form-control" />
    <span asp-validation-for="Name" class="text-danger"></span>
</div>

<div class="form-group">
    <label asp-for="Department" class="control-label"></label>
    <input asp-for="Department" class="form-control" />
    <span asp-validation-for="Department" class="text-danger"></span>
</div>

<!-- Usage -->
@Html.EditorFor(model => model.Employee)
```

## CSS Guidelines

### Organization
```
wwwroot/css/
├── site.css              # Global styles
├── employee.css          # Employee-specific styles
├── forms.css             # Form styles
└── components/
    ├── cards.css
    ├── tables.css
    └── navigation.css
```

### CSS Best Practices
```css
/* site.css */

/* Use CSS variables for theming */
:root {
    --primary-color: #0d6efd;
    --secondary-color: #6c757d;
    --success-color: #198754;
    --danger-color: #dc3545;
    --warning-color: #ffc107;
    --info-color: #0dcaf0;
    
    --font-family-base: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    --font-size-base: 16px;
    --line-height-base: 1.5;
    
    --border-radius: 0.375rem;
    --box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
}

/* Utility classes */
.text-truncate {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
}

.shadow-sm {
    box-shadow: var(--box-shadow);
}

/* Component-specific styles */
.employee-card {
    border-radius: var(--border-radius);
    transition: transform 0.2s ease-in-out;
}

.employee-card:hover {
    transform: translateY(-2px);
    box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15);
}

/* Responsive design */
@media (max-width: 768px) {
    .employee-card {
        margin-bottom: 1rem;
    }
}

/* Print styles */
@media print {
    .no-print {
        display: none !important;
    }
}
```

### BEM Methodology (Optional but Recommended)
```css
/* Block */
.employee-card { }

/* Element */
.employee-card__header { }
.employee-card__body { }
.employee-card__footer { }

/* Modifier */
.employee-card--highlighted { }
.employee-card--compact { }
```

```cshtml
<div class="employee-card employee-card--highlighted">
    <div class="employee-card__header">
        <h3>@Model.Name</h3>
    </div>
    <div class="employee-card__body">
        <!-- Content -->
    </div>
    <div class="employee-card__footer">
        <!-- Actions -->
    </div>
</div>
```

## JavaScript Best Practices

### File Organization
```
wwwroot/js/
├── site.js               # Global JavaScript
├── employee.js           # Employee-specific
├── validation.js         # Custom validation
└── utils/
    ├── ajax.js
    ├── datepicker.js
    └── formatting.js
```

### Modern JavaScript Patterns
```javascript
// employee.js

// Use ES6+ features
const EmployeeManager = {
    // Initialize
    init() {
        this.bindEvents();
        this.loadEmployees();
    },
    
    // Event binding
    bindEvents() {
        document.addEventListener('DOMContentLoaded', () => {
            const deleteButtons = document.querySelectorAll('.btn-delete');
            deleteButtons.forEach(btn => {
                btn.addEventListener('click', this.handleDelete.bind(this));
            });
        });
    },
    
    // Load employees via AJAX
    async loadEmployees() {
        try {
            const response = await fetch('/api/employees');
            if (!response.ok) throw new Error('Network response was not ok');
            
            const employees = await response.json();
            this.renderEmployees(employees);
        } catch (error) {
            console.error('Error loading employees:', error);
            this.showError('Failed to load employees');
        }
    },
    
    // Handle delete with confirmation
    handleDelete(event) {
        event.preventDefault();
        
        const employeeId = event.target.dataset.employeeId;
        const employeeName = event.target.dataset.employeeName;
        
        if (confirm(`Are you sure you want to delete ${employeeName}?`)) {
            this.deleteEmployee(employeeId);
        }
    },
    
    // Delete employee
    async deleteEmployee(id) {
        try {
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            
            const response = await fetch(`/Employee/Delete/${id}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                }
            });
            
            if (response.ok) {
                this.removeEmployeeFromDOM(id);
                this.showSuccess('Employee deleted successfully');
            } else {
                throw new Error('Delete failed');
            }
        } catch (error) {
            console.error('Error deleting employee:', error);
            this.showError('Failed to delete employee');
        }
    },
    
    // DOM manipulation
    removeEmployeeFromDOM(id) {
        const element = document.querySelector(`[data-employee-id="${id}"]`).closest('.employee-card');
        element?.remove();
    },
    
    // Notifications
    showSuccess(message) {
        this.showNotification(message, 'success');
    },
    
    showError(message) {
        this.showNotification(message, 'danger');
    },
    
    showNotification(message, type) {
        const alert = document.createElement('div');
        alert.className = `alert alert-${type} alert-dismissible fade show`;
        alert.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        document.querySelector('.container').prepend(alert);
        
        setTimeout(() => alert.remove(), 5000);
    },
    
    // Render employees
    renderEmployees(employees) {
        const container = document.getElementById('employees-container');
        container.innerHTML = '';
        
        employees.forEach(employee => {
            const card = this.createEmployeeCard(employee);
            container.appendChild(card);
        });
    },
    
    createEmployeeCard(employee) {
        const template = document.getElementById('employee-card-template');
        const card = template.content.cloneNode(true);
        
        card.querySelector('.employee-name').textContent = employee.name;
        card.querySelector('.employee-department').textContent = employee.department;
        card.querySelector('.employee-position').textContent = employee.position;
        
        return card;
    }
};

// Initialize
EmployeeManager.init();
```

### Form Validation
```javascript
// validation.js

// Custom client-side validation
const FormValidator = {
    init() {
        const forms = document.querySelectorAll('.needs-validation');
        forms.forEach(form => {
            form.addEventListener('submit', this.handleSubmit.bind(this));
        });
    },
    
    handleSubmit(event) {
        const form = event.target;
        
        if (!form.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
        }
        
        form.classList.add('was-validated');
    },
    
    // Custom validation rules
    validateEmail(email) {
        const pattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return pattern.test(email);
    },
    
    validatePhone(phone) {
        const pattern = /^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$/;
        return pattern.test(phone);
    }
};

FormValidator.init();
```

### AJAX with Anti-Forgery Token
```javascript
// Helper function for AJAX requests with CSRF token
async function fetchWithToken(url, options = {}) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    
    const defaultOptions = {
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        }
    };
    
    const mergedOptions = {
        ...defaultOptions,
        ...options,
        headers: {
            ...defaultOptions.headers,
            ...options.headers
        }
    };
    
    const response = await fetch(url, mergedOptions);
    
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    return response;
}

// Usage
async function updateEmployee(id, data) {
    try {
        const response = await fetchWithToken(`/Employee/Update/${id}`, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
        
        return await response.json();
    } catch (error) {
        console.error('Update failed:', error);
        throw error;
    }
}
```

## Bootstrap Integration

### Grid System
```cshtml
<div class="container">
    <div class="row">
        <div class="col-12 col-md-6 col-lg-4">
            <!-- Column content -->
        </div>
        <div class="col-12 col-md-6 col-lg-4">
            <!-- Column content -->
        </div>
        <div class="col-12 col-md-12 col-lg-4">
            <!-- Column content -->
        </div>
    </div>
</div>
```

### Forms
```cshtml
<form>
    <div class="mb-3">
        <label for="employeeName" class="form-label">Employee Name</label>
        <input type="text" class="form-control" id="employeeName" required>
        <div class="invalid-feedback">Please provide a name.</div>
    </div>
    
    <div class="mb-3">
        <label for="department" class="form-label">Department</label>
        <select class="form-select" id="department" required>
            <option value="">Choose...</option>
            <option value="IT">IT</option>
            <option value="HR">HR</option>
        </select>
        <div class="invalid-feedback">Please select a department.</div>
    </div>
    
    <button type="submit" class="btn btn-primary">Submit</button>
</form>
```

### Cards
```cshtml
<div class="card">
    <div class="card-header">
        <h5 class="card-title mb-0">@Model.Name</h5>
    </div>
    <div class="card-body">
        <p class="card-text">Department: @Model.Department</p>
        <p class="card-text">Position: @Model.Position</p>
    </div>
    <div class="card-footer">
        <a asp-action="Edit" asp-route-id="@Model.Id" class="btn btn-warning btn-sm">Edit</a>
        <a asp-action="Delete" asp-route-id="@Model.Id" class="btn btn-danger btn-sm">Delete</a>
    </div>
</div>
```

## Accessibility (A11Y)

### ARIA Attributes
```cshtml
<button type="button" 
        class="btn btn-primary" 
        aria-label="Delete employee"
        aria-describedby="deleteHelp">
    <i class="bi bi-trash" aria-hidden="true"></i> Delete
</button>
<small id="deleteHelp" class="form-text">This action cannot be undone.</small>
```

### Semantic HTML
```cshtml
<nav aria-label="breadcrumb">
    <ol class="breadcrumb">
        <li class="breadcrumb-item"><a href="/">Home</a></li>
        <li class="breadcrumb-item"><a href="/Employee">Employees</a></li>
        <li class="breadcrumb-item active" aria-current="page">Details</li>
    </ol>
</nav>
```

### Form Labels
```cshtml
<!-- Always associate labels with inputs -->
<label for="employeeName">Name</label>
<input type="text" id="employeeName" name="Name" class="form-control" />
```

## Performance Optimization

### Asset Bundling
- Use `asp-append-version="true"` for cache busting
- Minify CSS and JavaScript in production
- Use CDN for third-party libraries with fallbacks
- Lazy load images and non-critical resources

### Image Optimization
```cshtml
<img src="~/images/employee-photo.jpg" 
     alt="Employee photo"
     loading="lazy"
     width="200"
     height="200"
     asp-append-version="true" />
```

### Script Loading
```cshtml
<!-- Defer non-critical scripts -->
<script src="~/js/analytics.js" defer></script>

<!-- Async for independent scripts -->
<script src="~/js/widget.js" async></script>
```

## Security Considerations

### Content Security Policy
```cshtml
<!-- In _Layout.cshtml -->
@{
    // Set CSP via meta tag or HTTP header
}
<meta http-equiv="Content-Security-Policy" 
      content="default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline';">
```

### XSS Prevention
```cshtml
<!-- Razor auto-encodes -->
<p>@Model.UserInput</p>

<!-- For raw HTML (use with caution) -->
@Html.Raw(Model.TrustedHtml)

<!-- Encode in JavaScript -->
<script>
    const userName = '@Html.Raw(Json.Serialize(Model.Name))';
</script>
```

## AI Code Generation Rules

When generating frontend code:

1. ✅ Use Tag Helpers over HTML Helpers
2. ✅ Include proper ARIA attributes for accessibility
3. ✅ Use Bootstrap classes for consistent styling
4. ✅ Include anti-forgery tokens in forms
5. ✅ Use async/await for AJAX calls
6. ✅ Include error handling in JavaScript
7. ✅ Follow ES6+ JavaScript standards
8. ✅ Ensure responsive design
9. ✅ Validate forms on client and server
10. ✅ Never inline sensitive data in JavaScript

---

**Last Updated:** October 27, 2025  
**Review Frequency:** Monthly  
**Document Owner:** Frontend Lead Developer
