# MvcAppService - Employee Management System

A modern ASP.NET Core 9.0 MVC web application for managing employee records with full CRUD (Create, Read, Update, Delete) operations.

## Features

- **Employee Management**: Complete CRUD operations for employee records
- **Entity Framework Core**: SQL Server database integration with EF Core 9.0
- **Azure Integration**: 
  - Application Insights for telemetry and monitoring
  - Azure Key Vault integration for secure configuration management
- **Responsive UI**: Modern web interface built with ASP.NET Core MVC
- **Cloud-Ready**: Configured for deployment to Azure App Service

## Technology Stack

- **Framework**: .NET 9.0
- **Web Framework**: ASP.NET Core MVC
- **Database**: SQL Server with Entity Framework Core 9.0
- **Cloud Services**: 
  - Azure Application Insights
  - Azure Key Vault
  - Azure App Service
- **Authentication**: Azure Identity for managed identity support

## Employee Model

Each employee record includes:
- **Name**: Employee's full name (required)
- **Department**: Department assignment (required)
- **Position**: Job title/position (required)

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server or SQL Server LocalDB
- (Optional) Azure subscription for cloud deployment

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/lifesawesome/MvcAppService.git
cd MvcAppService
```

### 2. Configure Database Connection

Update the connection string in `MvcCrudApp/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EmployeeDB;Trusted_Connection=true;MultipleActiveResultSets=true"
}
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Apply Database Migrations

```bash
cd MvcCrudApp
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

The application will start and be accessible at `https://localhost:5001` (or the port specified in the console output).

## Project Structure

```
MvcAppService/
├── MvcCrudApp/
│   ├── Controllers/          # MVC Controllers
│   │   ├── EmployeeController.cs
│   │   └── HomeController.cs
│   ├── Models/              # Data models
│   │   └── Employee.cs
│   ├── Views/               # Razor views
│   │   ├── Employee/
│   │   ├── Home/
│   │   └── Shared/
│   ├── Data/                # Database context
│   │   └── ApplicationDbContext.cs
│   ├── Migrations/          # EF Core migrations
│   ├── wwwroot/            # Static files
│   ├── Program.cs          # Application entry point
│   └── appsettings.json    # Configuration
├── .github/workflows/       # CI/CD pipelines
└── SampleApp.sln           # Visual Studio solution
```

## Configuration

### Development Environment

In development mode, the application uses:
- Local SQL Server (LocalDB)
- Application Insights (optional)
- Console logging

### Production Environment

In production, the application:
- Connects to Azure Key Vault for secure configuration
- Uses Azure Application Insights for monitoring
- Applies HSTS and exception handling middleware

### Azure Key Vault Configuration

For production deployments, configure the Key Vault endpoint in `Program.cs`:

```csharp
var keyVaultEndpoint = new Uri("https://your-keyvault.vault.azure.net/");
```

## Available Operations

### Employee Management

- **View All Employees**: Navigate to `/Employee` or `/Employee/Index`
- **View Employee Details**: `/Employee/Details/{id}`
- **Create New Employee**: `/Employee/Create`
- **Edit Employee**: `/Employee/Edit/{id}`
- **Delete Employee**: `/Employee/Delete/{id}`

## Monitoring

The application integrates with Azure Application Insights to track:
- Custom events (e.g., "EmployeepageVisited")
- Request telemetry
- Dependency calls
- Exceptions and errors
- Performance metrics

## Deployment

The repository includes GitHub Actions workflow for automated deployment to Azure App Service. See `.github/workflows/main_demowebmvcservice.yml` for CI/CD configuration.

### Azure Resources Required

- Azure App Service
- Azure SQL Database
- Azure Key Vault
- Azure Application Insights

## Build and Test

### Build the Project

```bash
dotnet build
```

### Run in Development Mode

```bash
dotnet run --environment Development
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -am 'Add new feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Create a Pull Request

## License

This project is available for educational and demonstration purposes.
