# MVC CRUD Application

An ASP.NET Core 9.0 MVC application demonstrating CRUD operations with Entity Framework Core and SQL Server.

## Features
- Employee management (Create, Read, Update, Delete)
- Entity Framework Core with SQL Server
- Azure Application Insights integration
- Azure Key Vault integration for secure configuration
- Responsive UI with Bootstrap

## Security

⚠️ **Important**: This application follows OWASP security best practices. Never commit sensitive data like connection strings or API keys to source control.

For instructions on setting up the application securely, see [SECURITY_SETUP.md](SECURITY_SETUP.md).

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server (local or Azure)
- Visual Studio 2022, VS Code, or any text editor

### Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/lifesawesome/MvcAppService.git
   cd MvcAppService
   ```

2. **Configure User Secrets** (see [SECURITY_SETUP.md](SECURITY_SETUP.md) for details)
   ```bash
   cd MvcCrudApp
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING"
   ```

3. **Run database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

The application will be available at `https://localhost:5001` (or the port specified in launchSettings.json).

## Project Structure

```
MvcCrudApp/
├── Controllers/       # MVC Controllers
├── Data/             # Database context and migrations
├── Models/           # Data models
├── Views/            # Razor views
├── wwwroot/          # Static files
├── appsettings.json.template  # Template for configuration
└── Program.cs        # Application entry point
```

## Configuration

The application uses a layered configuration approach:

- **Development**: User Secrets (recommended) or environment variables
- **Production/Staging**: Azure Key Vault

See [SECURITY_SETUP.md](SECURITY_SETUP.md) for detailed configuration instructions.

## Security Guidelines

⚠️ **Important Security Practices:**
- Never commit secrets, credentials, or API keys to source control
- Use User Secrets for development (see [SECURITY_SETUP.md](SECURITY_SETUP.md))
- Use Azure Key Vault for production/staging environments
- Follow OWASP security best practices for all code changes

For detailed security guidelines, see project instructions in `.github/instructions/SecurityInstructions.instructions.md`.

## Contributing

1. Create a feature branch
2. Make your changes
3. Ensure all tests pass
4. Run security scan: `dotnet list package --vulnerable`
5. Submit a pull request

## License

This project is for demonstration purposes.
