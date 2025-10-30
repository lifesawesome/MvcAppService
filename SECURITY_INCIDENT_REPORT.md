# Security Incident Report - Hardcoded Secrets Removed

## Date
October 30, 2025

## Issue
Hardcoded secrets were found committed to the repository in version control history.

## Compromised Credentials

### 1. Application Insights Instrumentation Key
- **Key ID**: `d6e0f8e3-59f2-446e-bc7e-d8a9ee200981`
- **Location**: `MvcCrudApp/appsettings.json`
- **First Committed**: October 6, 2025 (commit `0ba8ab8`)
- **Removed**: October 30, 2025 (commit `e32e5f5`)
- **Public Exposure**: Yes (committed to public/private repository)

### 2. Database Connection String
- **Type**: SQL LocalDB with Windows Authentication
- **Connection String**: `Server=(localdb)\\mssqllocaldb;Database=EmployeeDB;Trusted_Connection=true;MultipleActiveResultSets=true`
- **Risk Level**: Low (development-only, uses Windows Authentication)
- **Action Required**: None (safe for local development)

## Actions Taken

1. ✅ Removed hardcoded secrets from appsettings.json
2. ✅ Removed appsettings.json from git tracking
3. ✅ Updated .gitignore to prevent future commits of sensitive files
4. ✅ Created template files with placeholder values
5. ✅ Created comprehensive security documentation (SECURITY_SETUP.md)
6. ✅ Updated README.md with security setup instructions

## Actions Required

### CRITICAL - Rotate Application Insights Key

The Application Insights instrumentation key `d6e0f8e3-59f2-446e-bc7e-d8a9ee200981` was exposed in git history and **MUST be rotated immediately**.

#### Steps to Rotate:

1. **Create a new Application Insights resource** (recommended) or **regenerate the instrumentation key**:
   
   **Option A: Azure Portal**
   - Go to Azure Portal → Application Insights → Your Resource
   - Navigate to "API Access" or "Properties"
   - Create a new key or resource

   **Option B: Azure CLI**
   ```bash
   # Create a new Application Insights resource
   az monitor app-insights component create \
     --app demowebmvc-appinsights-new \
     --location eastus \
     --resource-group your-resource-group
   
   # Get the new connection string
   az monitor app-insights component show \
     --app demowebmvc-appinsights-new \
     --resource-group your-resource-group \
     --query connectionString
   ```

2. **Update Key Vault with new connection string**:
   ```bash
   az keyvault secret set \
     --vault-name demowebmvc-keyvault \
     --name "ApplicationInsights--ConnectionString" \
     --value "YOUR_NEW_CONNECTION_STRING"
   ```

3. **Update local development User Secrets** (if used):
   ```bash
   cd MvcCrudApp
   dotnet user-secrets set "ApplicationInsights:ConnectionString" "YOUR_NEW_CONNECTION_STRING"
   ```

4. **Restart all deployed applications** to pick up the new key from Key Vault

5. **Monitor for unauthorized usage** of the old key for 30 days

6. **Deactivate the old Application Insights resource** after confirming the new one is working

## Prevention Measures Implemented

1. ✅ `.gitignore` updated to exclude all appsettings*.json files
2. ✅ Template files created with empty/placeholder values
3. ✅ Documentation added for proper secret management
4. ✅ CI/CD pipeline should use Key Vault for production secrets
5. ✅ Developer guide created for using User Secrets locally

## Lessons Learned

1. Never commit configuration files containing secrets to version control
2. Use environment-specific configuration with proper secret management
3. Use User Secrets for local development, Key Vault for production
4. Implement pre-commit hooks to detect secrets (future improvement)
5. Regular security audits of committed code

## Future Improvements

1. **Implement pre-commit hooks** to scan for secrets before commits
2. **Add secret scanning** to CI/CD pipeline (e.g., GitHub Secret Scanning)
3. **Use tools like git-secrets or TruffleHog** to prevent secret commits
4. **Regular security training** for development team
5. **Automated secret rotation** policies

## References

- [OWASP Secrets Management Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Secrets_Management_Cheat_Sheet.html)
- [Azure Key Vault Best Practices](https://learn.microsoft.com/en-us/azure/key-vault/general/best-practices)
- [GitHub Secret Scanning](https://docs.github.com/en/code-security/secret-scanning/about-secret-scanning)

## Sign-off

- **Remediation Completed By**: Copilot Agent
- **Date**: October 30, 2025
- **Status**: ⚠️ AWAITING KEY ROTATION
