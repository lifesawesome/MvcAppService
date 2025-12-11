# Security Configuration Setup Guide

## Overview
This guide explains how to configure sensitive application settings securely for the MvcCrudApp.

## ⚠️ Important Security Notice
**NEVER commit `appsettings.json` with real connection strings, passwords, or API keys to source control.**

---

## Development Environment Setup

### Using .NET User Secrets (Recommended for Local Development)

User Secrets is a secure way to store sensitive data during development without committing it to source control.

#### Step 1: Initialize User Secrets

Navigate to the project directory and run:

```bash
cd MvcCrudApp
dotnet user-secrets init
```

This command adds a `UserSecretsId` to your `.csproj` file and creates a secrets storage location on your machine.

#### Step 2: Set Connection String

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;"
```

**Example:**
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=tcp:demoservercrud.database.windows.net,1433;Initial Catalog=EmployeeDB;User ID=YOUR_USER_ID;Password=YOUR_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30"
```

#### Step 3: Set Application Insights Connection String (Optional)

```bash
dotnet user-secrets set "ApplicationInsights:ConnectionString" "InstrumentationKey=YOUR_INSTRUMENTATION_KEY"
```

#### Step 4: Verify Secrets

List all configured secrets:

```bash
dotnet user-secrets list
```

#### Step 5: Run the Application

```bash
dotnet run
```

The application will automatically load secrets from User Secrets in Development environment.

### Alternative: Environment Variables

You can also use environment variables for local development:

#### Windows (PowerShell)
```powershell
$env:ConnectionStrings__DefaultConnection="Server=YOUR_SERVER;Database=YOUR_DATABASE;..."
$env:ApplicationInsights__ConnectionString="InstrumentationKey=YOUR_KEY"
dotnet run
```

#### Linux/macOS (Bash)
```bash
export ConnectionStrings__DefaultConnection="Server=YOUR_SERVER;Database=YOUR_DATABASE;..."
export ApplicationInsights__ConnectionString="InstrumentationKey=YOUR_KEY"
dotnet run
```

**Note:** Use double underscores `__` to represent nested configuration levels.

---

## Production/Staging Environment Setup

### Using Azure Key Vault

The application is already configured to use Azure Key Vault for non-development environments (see `Program.cs`).

#### Prerequisites
- Azure subscription
- Azure Key Vault created: `https://demowebmvc-keyvault.vault.azure.net/`
- Appropriate permissions (Key Vault Secrets Officer or similar)

#### Step 1: Add Secrets to Azure Key Vault

Using Azure CLI:

```bash
# Login to Azure
az login

# Set the Key Vault name
KEYVAULT_NAME="demowebmvc-keyvault"

# Add Connection String
az keyvault secret set --vault-name $KEYVAULT_NAME --name "ConnectionStrings--DefaultConnection" --value "Server=tcp:demoservercrud.database.windows.net,1433;Initial Catalog=EmployeeDB;User ID=YOUR_USER_ID;Password=YOUR_PASSWORD;..."

# Add Application Insights Connection String
az keyvault secret set --vault-name $KEYVAULT_NAME --name "ApplicationInsights--ConnectionString" --value "InstrumentationKey=YOUR_KEY"
```

**Important:** 
- Use double dashes `--` in secret names to represent nested configuration (e.g., `ConnectionStrings--DefaultConnection`)
- Key Vault secret names cannot contain colons or periods

#### Step 2: Configure Managed Identity

For Azure App Service:

1. Enable System-assigned Managed Identity on your App Service
2. Grant the Managed Identity access to the Key Vault:

```bash
az keyvault set-policy --name demowebmvc-keyvault --object-id <MANAGED_IDENTITY_OBJECT_ID> --secret-permissions get list
```

#### Step 3: Deploy Application

The application will automatically connect to Key Vault using `DefaultAzureCredential` when not in Development environment.

---

## Configuration Priority

ASP.NET Core loads configuration in this order (later sources override earlier ones):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. User Secrets (Development only)
4. Environment Variables
5. Azure Key Vault (Production/Staging)
6. Command-line arguments

---

## Security Best Practices

### ✅ DO:
- Use User Secrets for local development
- Use Azure Key Vault for production/staging
- Use environment variables for CI/CD pipelines
- Rotate credentials regularly
- Use strong, unique passwords
- Enable Azure Key Vault soft-delete and purge protection
- Use Managed Identities instead of service principals when possible
- Monitor access to Key Vault using Azure Monitor

### ❌ DON'T:
- Commit `appsettings.json` with real secrets to source control
- Share User Secrets files or Key Vault credentials
- Use the same credentials for development and production
- Store secrets in plain text files
- Email or message secrets
- Reuse passwords across environments

---

## Troubleshooting

### Error: "A connection was successfully established with the server, but then an error occurred"

**Solution:** Check that your connection string is correctly formatted and the credentials are valid.

### Error: "Key Vault secrets not loading"

**Possible causes:**
1. Managed Identity not configured
2. Insufficient Key Vault permissions
3. Incorrect Key Vault URL in `Program.cs`
4. Environment variable not set correctly

**Solution:** Verify Managed Identity has "Get" and "List" secret permissions in Key Vault.

### Error: "Unable to find User Secrets"

**Solution:** Run `dotnet user-secrets init` in the project directory.

---

## Credential Rotation Guide

### After a Security Incident

If credentials were exposed (e.g., committed to Git):

1. **Immediately rotate all exposed credentials:**
   - Database passwords
   - Application Insights connection strings/instrumentation keys
   - Any other API keys or secrets

2. **Update Azure Key Vault:**
   ```bash
   az keyvault secret set --vault-name demowebmvc-keyvault --name "ConnectionStrings--DefaultConnection" --value "NEW_CONNECTION_STRING"
   ```

3. **Update User Secrets locally:**
   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "NEW_CONNECTION_STRING"
   ```

4. **Review Git history:**
   - Consider using tools like `git-filter-branch` or `BFG Repo-Cleaner` to remove secrets from history
   - Rotate credentials even if removed from current commit

5. **Audit access logs:**
   - Check database access logs for unauthorized access
   - Review Application Insights for suspicious activity

---

## References

- [Safe storage of app secrets in development](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Azure Key Vault configuration provider](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration)
- [OWASP Top 10 - Sensitive Data Exposure](https://owasp.org/www-project-top-ten/)
- [Azure Key Vault Best Practices](https://docs.microsoft.com/en-us/azure/key-vault/general/best-practices)

---

## Contact

For security-related questions or to report a security incident, contact the Security Team.

**Last Updated:** December 11, 2025
