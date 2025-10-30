# Security Configuration Guide

## Overview

This application uses Azure Key Vault for production secrets and User Secrets for local development. **Never commit appsettings.json or appsettings.Development.json files with sensitive data.**

## Local Development Setup

### Option 1: User Secrets (Recommended)

1. Initialize User Secrets for the project:
   ```bash
   cd MvcCrudApp
   dotnet user-secrets init
   ```

2. Set the required secrets:
   ```bash
   # Database Connection String
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\mssqllocaldb;Database=EmployeeDB;Trusted_Connection=true;MultipleActiveResultSets=true"
   
   # Application Insights (optional for local dev)
   dotnet user-secrets set "ApplicationInsights:ConnectionString" "YOUR_APP_INSIGHTS_CONNECTION_STRING"
   ```

3. View configured secrets:
   ```bash
   dotnet user-secrets list
   ```

### Option 2: Local appsettings Files

1. Copy the template files:
   ```bash
   cp appsettings.json.template appsettings.json
   cp appsettings.Development.json.template appsettings.Development.json
   ```

2. Edit the files and add your local connection strings and secrets.

3. **IMPORTANT**: These files are excluded from git via .gitignore. Never commit them.

## Production Setup

### Azure Key Vault Configuration

The application is configured to use Azure Key Vault in production (non-Development environments).

1. Create an Azure Key Vault (if not exists):
   ```bash
   az keyvault create --name demowebmvc-keyvault --resource-group your-resource-group --location eastus
   ```

2. Add secrets to Key Vault:
   ```bash
   # Database Connection String
   az keyvault secret set --vault-name demowebmvc-keyvault --name "ConnectionStrings--DefaultConnection" --value "YOUR_PRODUCTION_CONNECTION_STRING"
   
   # Application Insights Connection String
   az keyvault secret set --vault-name demowebmvc-keyvault --name "ApplicationInsights--ConnectionString" --value "YOUR_APP_INSIGHTS_CONNECTION_STRING"
   ```

3. Grant access to your application:
   - Use Managed Identity in Azure App Service
   - Or configure a Service Principal with appropriate permissions

## Required Secrets

### ConnectionStrings:DefaultConnection
- **Development**: SQL LocalDB connection string
- **Production**: Azure SQL Database connection string
- **Format**: `Server=your-server;Database=your-db;User Id=your-user;Password=your-password`

### ApplicationInsights:ConnectionString
- **Purpose**: Application monitoring and telemetry
- **Format**: `InstrumentationKey=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`
- **Get from**: Azure Portal → Application Insights → Properties

## Security Best Practices

1. ✅ **Use User Secrets** for local development
2. ✅ **Use Azure Key Vault** for production
3. ❌ **Never** commit appsettings.json or appsettings.Development.json with real secrets
4. ❌ **Never** commit connection strings or API keys to source control
5. ✅ **Rotate secrets** immediately if they are accidentally committed
6. ✅ **Use Managed Identity** in Azure for Key Vault access

## Troubleshooting

### Missing Configuration Error
If you see errors about missing configuration:
1. Verify User Secrets are set (run `dotnet user-secrets list`)
2. Or verify local appsettings files exist and contain required values
3. For production, verify Key Vault is accessible and contains required secrets

### Key Vault Access Issues
1. Verify Managed Identity is enabled on your App Service
2. Verify the Managed Identity has "Get" and "List" permissions on Key Vault secrets
3. Check the Key Vault URL in Program.cs matches your Key Vault name

## Documentation References

- [ASP.NET Core User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Azure Key Vault Configuration Provider](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration)
- [Managed Identity in App Service](https://docs.microsoft.com/en-us/azure/app-service/overview-managed-identity)
