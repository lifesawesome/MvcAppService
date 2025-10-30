# MVC CRUD Application

A sample ASP.NET Core MVC application demonstrating CRUD operations with Entity Framework Core.

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server LocalDB (for development) or Azure SQL Database (for production)
- Azure subscription (for production deployment)

### Initial Setup

1. **Configure Secrets** - See [SECURITY_SETUP.md](SECURITY_SETUP.md) for detailed instructions on configuring connection strings and secrets.

2. **Database Setup**:
   ```bash
   cd MvcCrudApp
   dotnet ef database update
   ```

3. **Run the Application**:
   ```bash
   dotnet run
   ```

## Security

**IMPORTANT**: This application uses secure secret management:
- **Development**: User Secrets (recommended) or local appsettings files (excluded from git)
- **Production**: Azure Key Vault

See [SECURITY_SETUP.md](SECURITY_SETUP.md) for complete setup instructions.

## Project Structure

- `MvcCrudApp/` - Main application
  - `Controllers/` - MVC controllers
  - `Models/` - Data models
  - `Views/` - Razor views
  - `Data/` - Entity Framework DbContext
  - `Migrations/` - Database migrations

## Deployment

The application is configured for Azure App Service deployment with:
- Managed Identity for Key Vault access
- Application Insights for monitoring
- Automated CI/CD via GitHub Actions
