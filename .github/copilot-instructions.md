# Copilot Instructions — M365 Guard

## Project Overview

- **Purpose**: .NET 10 Security & Audit Suite for Microsoft 365 — Sign-in analyses, Conditional Access evaluation, tenant compliance checks
- **Pattern**: No-secrets certificate auth, Excel export, multi-tenant support
- **Architecture**: Application Insights instrumentation, EPPlus Excel generation, Microsoft Graph integration
- **Primary Use Cases**: Sign-in audit export, Conditional Access data analysis, tenant health assessment

## Project Structure

```
M365 Guard/
├── M365 Guard.slnx       # Solution file
├── M365 Guard/           # Main project
│   ├── Program.cs        # Entry point (auth logic, cert chain)
│   ├── M365 Guard.csproj
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── QUICK-START.md    # 30-second setup guide
│   ├── README.md
│   ├── SECURITY.md
│   ├── SECURITY-AUDIT.md
│   ├── SESSION-NOTES.md
│   ├── Services/         # Domain logic
│   │   ├── AuditService.cs           # Sign-in audit operations
│   │   ├── ConditionalAccessService.cs
│   │   ├── ExcelExportService.cs     # EPPlus-based export
│   │   ├── GraphApiService.cs        # Microsoft Graph client
│   │   └── [+ other domain services]
│   ├── Models/           # Data structures
│   ├── Helpers/          # Utilities
│   └── bin/Debug/        # Build output
│
└── .github/
    ├── workflows/        # CI/CD automation
    └── copilot-instructions.md (this file)
```

## Authentication Strategy (No Secrets)

### Certificate-Based Credential Chain ✅

```
Priority 1: Certificate from Azure Key Vault (Production)
   └─ read-only, audit-logged access
   
Priority 2: Certificate from local certificate store (Developer)
   └─ self-signed or imported certificate
   
Priority 3: Azure CLI credentials via `az login` (Fastest)
   └─ interactive browser login, supports MFA
```

### Why This Pattern?

- ✅ **No Client Secrets** in code, config, or repos → no rotation burden
- ✅ **Audit Trail** → Every Key Vault cert access is logged
- ✅ **Developer Experience** → `az login` once, then just F5
- ✅ **Production Safe** → Certificates are single-source-of-truth in Key Vault

## Development Fast-Track (5 minutes)

### Step 1: Azure CLI Auth

```powershell
# Install Azure CLI (if needed)
winget install Microsoft.AzureCLI

# Log in (opens browser for MFA)
az login

# Verify access
az account show
```

### Step 2: Open & Build

```powershell
# From workspace root
# Option A: Visual Studio Enterprise
start M365\ Guard\M365\ Guard.slnx

# Option B: Visual Studio Code
code M365\ Guard\M365\ Guard.slnx

# Or command line
cd "M365 Guard\M365 Guard"
dotnet build
```

### Step 3: Run (Press F5)

```powershell
# In Visual Studio: F5
# Or terminal:
dotnet run

# Expected output:
# ═══════════════════════════════════════════════════════
#    M365 Guard - Security & Audit Suite
#    Rahman Finance and Accounting P.L.LC
# ═══════════════════════════════════════════════════════
# 
# 1. 📊 Export NonInteractive Sign-Ins to Excel
# 2. 📊 Export Interactive Sign-Ins to Excel  ← For CA data!
# ...
```

## Key Workflows

### Export Sign-In Audit (Option 2 - Interactive)

**Fastest path to Conditional Access data:**

```powershell
# 1. Run the app
dotnet run

# 2. Type: 2
# 3. Press Enter
# 4. App exports to: E:\source\Dashboard for Kings.xlsm

# Done! Open in Excel, filter by CA Status
```

**Output columns (Interactive Sign-Ins):**
- **Conditional Access Status** ← Key for CA analysis
- User, Application, Date, Time
- Risk Level, Risk Details
- IP Address, Device State
- [+ 20 additional columns]

### Export Sign-In Audit (Option 1 - NonInteractive)

Use this for app-owned resources and service principal activity:

```powershell
# 1. Run the app
dotnet run

# 2. Type: 1
# 3. Press Enter
# 4. Output: E:\source\Dashboard for Kings.xlsm
```

**Output columns (NonInteractive Sign-Ins):**
- Service Principal Name, Application ID
- Date, Time, Status
- Resource, Resource Principal Name
- [+ additional service account data]

## Configuration Files

### appsettings.json (Shared)

```json
{
  "AzureAd": {
    "TenantId": "e8b875bc-6cf4-4c2e-9ded-1371aaf26563",
    "ClientId": "dcbaad46-52ed-4d3f-97a1-495cfae1039f",
    "Instance": "https://login.microsoftonline.com/",
    "Authority": "https://login.microsoftonline.com/{tenant}/"
  },
  "KeyVault": {
    "VaultUri": "https://m365guard-kv.vault.azure.net/",
    "CertificateName": "m365guard-cert"
  },
  "ISVConfiguration": {
    "IsMultiTenant": "false",
    "RequireMFA": "true",
    "MaxConcurrentRequests": "5"
  },
  "Excel": {
    "OutputPath": "E:\\source\\Dashboard for Kings.xlsm",
    "TimeoutSeconds": "300"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Identity": "Debug"
    }
  },
  "ApplicationInsights": {
    "InstrumentationKey": "[from Key Vault]"
  }
}
```

**Note:** No `ClientSecret` field — credentials resolved via auth chain above.

### appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.Identity": "Debug"
    }
  }
}
```

## Service Architecture

### AuditService
```csharp
// Queries Azure AD Sign-In logs via Microsoft Graph
// Filters by InteractiveSignIns or NonInteractiveSignIns
// Returns: List<SignInRecord> with 20+ columns
```

### ConditionalAccessService
```csharp
// Fetches Conditional Access policies
// Evaluates policy status and impact
// Maps CA Status to sign-in records
```

### ExcelExportService (EPPlus)
```csharp
// Generates Excel workbooks
// Applies formatting: Bold headers, auto-width, freeze panes
// Supports: Charts, pivot tables, conditional formatting
```

### GraphApiService
```csharp
// Base Microsoft Graph client wrapper
// Handles: Token acquisition, retry logic, pagination
// Supported: /auditLogs/signIns, /identity/conditionalAccess/policies
```

## Build & Deployment

### Local Build

```powershell
cd "M365 Guard\M365 Guard"

# Restore packages
dotnet restore

# Build (Debug)
dotnet build

# Build (Release)
dotnet build --configuration Release

# Run from CLI
dotnet run
```

### Publishing (for deployment)

```powershell
# Publish as self-contained exe
dotnet publish -c Release -r win-x64 --self-contained

# Output: bin/Release/net10.0/win-x64/publish/M365Guard.exe
```

### Azure Deployment Options

1. **Azure Container Instances** (Scheduled)
   ```powershell
   # Build Docker image
   docker build -t m365guard:latest .
   
   # Run on ACI (monthly scheduled export)
   az container create --resource-group rg-m365guard --image m365guard:latest ...
   ```

2. **Azure App Service** (Interactive)
   - Deploy via Visual Studio Publish
   - Configure: StartupCommand = `dotnet M365Guard.dll`
   - Slot: Development/Staging for testing

3. **Azure Automation Runbook** (Automated)
   - Schedule monthly exports
   - Notify via email/Teams

## Debugging

### Visual Studio

```powershell
# F5 to debug
# Set breakpoints in:
#   - Program.cs (auth chain)
#   - AuditService.cs (data queries)
#   - ExcelExportService.cs (export logic)

# Debug Console shows:
# - Azure CLI login prompt (if needed)
# - Service initialization logs
# - Graph API responses
# - Excel write progress
```

### VS Code

**Debug configuration (.vscode/launch.json):**
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "M365 Guard",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/M365 Guard/bin/Debug/net10.0/M365Guard.dll",
      "args": [],
      "cwd": "${workspaceFolder}/M365 Guard",
      "stopAtEntry": false,
      "console": "integratedTerminal"
    }
  ]
}
```

### Enable Detailed Logging

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.Identity": "Debug",
      "Microsoft.Graph": "Debug"
    }
  }
}
```

## Common Issues & Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| **"Invalid tenant ID"** | Missing/wrong in appsettings.json | Copy correct ID from [Configuration Files](#configuration-files) |
| **"Certificate not found"** | No local cert + Key Vault inaccessible | Run `az login` first (Priority 3) |
| **"Failed to authenticate"** | Azure CLI session expired | `az logout` then `az login` again |
| **Excel export timeout** | Processing >100k sign-ins | Increase `Excel.TimeoutSeconds` in config |
| **"Permission denied" on Graph API** | App lacks `AuditLog.Read.All` | Ensure app registration has consent + admin grant |
| **NuGet package conflict** | .csproj dependency mismatch | `dotnet clean` + `dotnet restore` |

## Testing

### Unit Tests

```powershell
cd "M365 Guard\M365 Guard.Tests"
dotnet test
```

### Integration Test (Local)

```powershell
# 1. Ensure Azure CLI logged in
az account show

# 2. Run app
dotnet run

# 3. Select option (1 or 2)

# 4. Verify Excel file created and populated
Test-Path "E:\source\Dashboard for Kings.xlsm"
```

## Excel Output Specification

### File Location
```
E:\source\Dashboard for Kings.xlsm
```

### Sheets

**Sheet 1: Interactive Sign-Ins**
- 80k rows max (Excel limit: 1.04M)
- Columns: User, App, Date, Time, CA Status, Risk Level, IP, Device, [+15 more]
- Formatting: Bold header, frozen panes, auto-width

**Sheet 2: NonInteractive Sign-Ins**
- Service accounts, app-owned resources
- Columns: Service Principal, App ID, Date, Time, Resource, [+10 more]

**Sheet 3: Summary**
- Statistics: Total sign-ins, success rate, risk breakdown, CA impact
- Charts: Sign-in trend, risk distribution, top-risk users

## Linking to M365Guard Console

This project runs within the [M365Guard Console App .01](../M365Guard%20Console%20App%20.01) workspace context.

**Entry flow:**
```
Console launcher (appsettings + auth setup)
  ↓
This M365 Guard application (menu selection)
  ↓
Service selection (Sign-in export options)
  ↓
Excel output (E:\source\Dashboard for Kings.xlsm)
```

See [M365Guard Console Copilot Instructions](../M365Guard%20Console%20App%20.01/.github/copilot-instructions.md).

## Security & Compliance

### No Secrets Stored

- ✅ Certificates in Key Vault only
- ✅ No client secrets in code/config
- ✅ Azure CLI credentials session-scoped
- ✅ Token lifetime: 60 minutes (automatic refresh)

### Audit Trail

- ✅ Key Vault access logged
- ✅ Application Insights telemetry
- ✅ Azure AD sign-in log queries (who accessed what, when)
- ✅ Excel export completion timestamps

### MFA Support

```json
{
  "ISVConfiguration": {
    "RequireMFA": "true"
  }
}
```

When enabled, Azure CLI login enforces MFA.

## Quick Reference

| Task | Command |
|------|---------|
| **Login to Azure** | `az login` |
| **Build** | `dotnet build` |
| **Run** | `dotnet run` (or F5 in IDE) |
| **Debug** | F5 Visual Studio / Set breakpoints |
| **Export sign-ins** | Run app → Select 1 or 2 |
| **Clean** | `dotnet clean` |
| **Publish (release)** | `dotnet publish -c Release` |

---

**For launcher/workspace setup:** See [M365Guard Console Copilot Instructions](../M365Guard%20Console%20App%20.01/.github/copilot-instructions.md)  
**For root orchestration:** See [Root Copilot Instructions](../../.github/copilot-instructions.md)
