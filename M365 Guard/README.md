# M365 Guard - Security & Audit Suite

## 🎯 Overview
Comprehensive Microsoft 365 security monitoring and audit solution for **Rahman Finance and Accounting P.L.LC**. This application provides advanced security auditing, device management, and automated Excel reporting capabilities.

**Built with:** .NET 10, Microsoft Graph SDK, Azure Identity, EPPlus

---

## ✨ Features Migrated from Console App .01

### 📊 Sign-In Audits
- **NonInteractive Sign-Ins** - Service principals, automated apps, background processes
- **Interactive Sign-Ins** - User logins with full context
- **Failed Sign-Ins** - Security audit for failed authentication attempts
- **Risky Sign-Ins** - Identity protection insights

### 🔍 Security Audit Agents
- **Intune Device Audit** - Scan managed devices, compliance status, stale devices
- **Entra ID Security Audit** - Risky users, failed sign-ins, non-compliant devices
- **Full Security Scan** - Parallel execution of all security agents

### 📱 Device Management
- Managed devices inventory
- Non-compliant devices tracking
- Stale devices detection (30+ days inactive)

### 👥 User Management
- Disable user accounts workflow
- Revoke all active sessions
- Risky users identification and export

### 📈 Excel Export
All reports export directly to:
```
E:\source\Dashboard for Kings.xlsm
```

---

## 🚀 Setup Instructions

### 1. Azure AD App Registration
Create an app registration in Azure AD with the following API permissions:

**Microsoft Graph API Permissions:**
- `AuditLog.Read.All` - Read sign-in logs
- `DeviceManagementManagedDevices.Read.All` - Read Intune devices
- `Directory.Read.All` - Read directory data
- `User.ReadWrite.All` - Manage users (for disable workflow)
- `IdentityRiskyUser.Read.All` - Read risky users
- `Device.Read.All` - Read devices
- `Policy.Read.All` - Read Conditional Access policies

### 2. Authentication (No Secrets Required!)

This application uses **certificate-based authentication** or **Azure CLI credentials** (no client secrets needed):

```powershell
# Log in with Azure CLI
az login

# Run the application
dotnet run
```

The app will automatically use your Azure CLI credentials in development mode.

### 3. Run the Application

```powershell
cd "M365 Guard"
dotnet run
```

---

## 📋 Menu Options

```
── Sign-In Audits ─────────────────────────────────────
1. 📊 Export NonInteractive Sign-Ins to Excel
2. 📊 Export Interactive Sign-Ins to Excel
3. 📊 Export Failed Sign-Ins to Excel
4. 📊 Export Risky Sign-Ins to Excel
5. 📊 Export ALL Sign-In Reports

── Security Audit Agents ──────────────────────────────
6. 🔍 Run Intune Device Audit
7. 🛡️  Run Entra ID Security Audit
8. 🚀 Run Full Security Scan (Parallel)

── Device Management ──────────────────────────────────
9. 📱 Export Managed Devices to Excel
10. ⚠️  Export Non-Compliant Devices to Excel

── User Management ────────────────────────────────────
11. 🚫 Disable User Account
12. 👥 Export Risky Users to Excel

── Configuration ──────────────────────────────────────
13. ⚙️  View Current Configuration
```

---

## 🗂️ Project Structure

```
M365 Guard/
├── Program.cs                          # Main application with menu system
├── appsettings.json                    # Configuration (NO SECRETS)
├── appsettings.Development.json        # Development overrides
├── .gitignore                          # Security: excludes secrets
├── README.md                           # This file
├── QUICK-START.md                      # Quick start guide
└── Services/
    ├── AuthenticationService.cs        # Azure AD authentication
    ├── GraphService.cs                 # Microsoft Graph API queries
    ├── IntuneDeviceService.cs          # Intune device management
    └── ExcelExportService.cs           # Excel report generation
```

---

## 🔐 Security Best Practices

### ✅ **What's Committed to Git:**
- ✅ Source code
- ✅ Configuration templates (no secrets)
- ✅ Documentation
- ✅ Public IDs (Tenant ID, Client ID)

### ❌ **What's NEVER Committed:**
- ❌ Client secrets
- ❌ Certificates (*.pfx, *.p12)
- ❌ User secrets
- ❌ Production configuration with secrets
- ❌ Excel files with data (*.xlsm, *.xlsx)

### 🔒 **Secret Management:**
- **Development:** Azure CLI credentials (no secrets stored)
- **Production:** Certificates in Azure Key Vault
- **Never:** Client secrets in code or config

---

## 📊 Excel Output

All reports are exported to **Dashboard for Kings.xlsm** with:
- Professional formatting (color-coded headers)
- Auto-filters enabled
- Frozen header rows
- Auto-fit columns
- Multiple worksheets for different data types

### Worksheets Created:
- `NonInteractiveSignIns` - Automated sign-ins
- `InteractiveSignIns` - User logins (includes Conditional Access status)
- `FailedSignIns` - Failed authentication attempts
- `RiskyUsers` - Identity protection alerts
- `ManagedDevices` - Intune device inventory

---

## 🛠️ Troubleshooting

### "Configuration not found"
- Ensure you've run `az login`
- Verify your Azure AD app registration

### "Insufficient permissions"
- Check API permissions in Azure AD app registration
- Ensure admin consent is granted

### "Cannot find Excel file"
- Update the path in `appsettings.json` under `ExcelExport:OutputPath`
- Ensure the directory exists

---

## 📚 Dependencies & Citations

### Core Frameworks:
- **.NET 10** - [Microsoft .NET](https://dot.net)
- **Microsoft Graph SDK v5.102.0** - [NuGet](https://www.nuget.org/packages/Microsoft.Graph/) | [Docs](https://learn.microsoft.com/en-us/graph/sdks/sdks-overview)
- **Azure.Identity v1.17.1** - [NuGet](https://www.nuget.org/packages/Azure.Identity/) | [Docs](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme)

### Excel Library:
- **EPPlus v7.5.3** - [NuGet](https://www.nuget.org/packages/EPPlus/) | [License: Polyform Noncommercial](https://www.epplussoftware.com/en/Home/LgplToPolyform)
  - Licensed under Polyform Noncommercial 1.0.0
  - For commercial use, a license is required from [EPPlus Software](https://www.epplussoftware.com/)

### Configuration:
- **Microsoft.Extensions.Configuration** v10.0.3 - [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Configuration/)
- **Microsoft.Extensions.Configuration.UserSecrets** v10.0.3 - [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.UserSecrets/)

### Azure Services:
- **Azure.Extensions.AspNetCore.Configuration.Secrets** v1.4.0 - [NuGet](https://www.nuget.org/packages/Azure.Extensions.AspNetCore.Configuration.Secrets/)

---

## 📝 License & Attribution

**Application License:** Proprietary - Rahman Finance and Accounting P.L.LC

**Third-Party Licenses:**
- EPPlus: Polyform Noncommercial 1.0.0 (see [EPPlus License](https://www.epplussoftware.com/en/Home/LgplToPolyform))
- Microsoft SDKs: MIT License
- Azure SDKs: MIT License

**Code Quality:**
- Automated code review powered by [Codacy](https://www.codacy.com/)
- Security scanning enabled
- Best practices enforcement

---

## 🎉 Features Comparison

| Feature | Old Console App | This Version |
|---------|----------------|--------------|
| NonInteractive Sign-Ins | ✅ | ✅ |
| Interactive Sign-Ins | ✅ | ✅ |
| Failed Sign-Ins | ✅ | ✅ |
| Security Audits | ✅ | ✅ |
| Device Management | ✅ | ✅ |
| Excel Export | ✅ | ✅ |
| User Secrets | ✅ | ✅ |
| Multi-Tenant | ✅ | ✅ |
| Parallel Execution | ✅ | ✅ |
| **Certificate Auth** | ❌ | ✅ |
| **No Client Secrets** | ❌ | ✅ |
| **Azure CLI Auth** | ❌ | ✅ |

---

## 👨‍💻 Developer

**Rahman Finance and Accounting P.L.LC**
- Contact: Hassan Rahman
- Built with: GitHub Copilot assistance

---

## 🔄 Version History

### v1.0.0 (2026-01-03)
- Initial release
- Migrated features from M365Guard Console App .01
- Added certificate-based authentication
- Added Azure CLI credential support
- Removed client secret requirements
- Enhanced security with .gitignore
- Added comprehensive documentation
