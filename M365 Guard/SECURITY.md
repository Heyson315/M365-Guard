# Security Policy

## 🔒 Security Considerations

This application follows security best practices for Microsoft 365 integration and data handling.

---

## ✅ What We DO

### Authentication
- ✅ **Certificate-based authentication** (preferred for production)
- ✅ **Azure CLI credentials** (development only)
- ✅ **Azure Key Vault** for certificate storage
- ✅ **Managed Identity** support (when deployed to Azure)
- ✅ **No client secrets** in code or configuration files

### Data Protection
- ✅ All sensitive data is excluded from Git via `.gitignore`
- ✅ User secrets stored outside the repository
- ✅ Tenant ID and Client ID are considered public (non-secret)
- ✅ Excel exports stay local - not committed to Git

### Least Privilege
- ✅ Only request necessary API permissions
- ✅ Application permissions (not delegated)
- ✅ Read-only access where possible

---

## ❌ What We DON'T DO

### Never Commit These:
- ❌ Client secrets
- ❌ Certificates (*.pfx, *.p12, *.key, *.pem)
- ❌ User credentials
- ❌ Access tokens
- ❌ Excel files with exported data
- ❌ Production configuration files with secrets
- ❌ Azure publish profiles

---

## 🔍 Secret Scanning

This repository uses:
- **GitHub Secret Scanning** (if enabled)
- **Codacy Security Scanning**
- **`.gitignore`** to prevent accidental commits

### Files That Are Safe to Commit:
```json
{
  "AzureAd": {
    "TenantId": "e8b875bc-6cf4-4c2e-9ded-1371aaf26563",  ✅ Public - OK
    "ClientId": "dcbaad46-52ed-4d3f-97a1-495cfae1039f"   ✅ Public - OK
    // NO ClientSecret here! ✅
  }
}
```

---

## 🛡️ Authentication Flow

### Development (Local):
```
1. Developer runs: az login
2. App uses AzureCliCredential
3. No secrets stored anywhere ✅
```

### Production (Azure):
```
1. App uses ManagedIdentity or Certificate
2. Certificate stored in Azure Key Vault
3. Key Vault access via Managed Identity
4. No secrets in code or config ✅
```

---

## 📋 API Permissions Required

### Microsoft Graph API:
| Permission | Type | Purpose | Justification |
|------------|------|---------|---------------|
| `AuditLog.Read.All` | Application | Read sign-in logs | Security auditing & compliance |
| `Directory.Read.All` | Application | Read directory data | User and device information |
| `Device.Read.All` | Application | Read device info | Device compliance monitoring |
| `IdentityRiskyUser.Read.All` | Application | Read risky users | Identity protection |
| `DeviceManagementManagedDevices.Read.All` | Application | Read Intune devices | Device management |
| `Policy.Read.All` | Application | Read CA policies | Conditional Access analysis |
| `User.ReadWrite.All` | Application | Disable user accounts | Security incident response |

**Admin Consent Required:** Yes (all are application permissions)

---

## 🚨 Security Incident Response

### If Secrets Are Accidentally Committed:

1. **Immediate Actions:**
   ```powershell
   # Rotate the client secret IMMEDIATELY in Azure Portal
   az ad app credential reset --id <app-id>
   
   # Revoke all active sessions for the app
   # Go to Azure Portal → App Registration → Exposed APIs → Revoke
   ```

2. **Clean Git History:**
   ```powershell
   # Remove from git history (use with caution!)
   git filter-branch --force --index-filter \
     "git rm --cached --ignore-unmatch <file-with-secret>" \
     --prune-empty --tag-name-filter cat -- --all
   
   # Force push (coordinate with team!)
   git push origin --force --all
   ```

3. **Audit:**
   - Check Azure AD sign-in logs for suspicious activity
   - Review application permissions usage
   - Notify security team

---

## 🔐 Recommended Deployment

### Production Checklist:
- [ ] Use certificate authentication (not client secrets)
- [ ] Store certificates in Azure Key Vault
- [ ] Enable Managed Identity
- [ ] Use Azure App Service or Container Apps
- [ ] Enable Application Insights for monitoring
- [ ] Set up alerts for failed authentication
- [ ] Regular security audits
- [ ] Keep dependencies updated

---

## 📞 Reporting Security Issues

If you discover a security vulnerability:

1. **DO NOT** create a public GitHub issue
2. Contact: security@rahmanfinance.com (if available)
3. Or create a private security advisory in GitHub
4. Include:
   - Description of the vulnerability
   - Steps to reproduce
   - Potential impact
   - Suggested fix (if any)

---

## 🔄 Security Updates

### Dependencies:
- Regularly update NuGet packages
- Monitor security advisories
- Test updates in development first

### Scanning Schedule:
- **On every commit:** Codacy automatic scanning
- **Weekly:** Dependency vulnerability check
- **Monthly:** Manual security review

---

## ✅ Security Verification Checklist

Before committing code:
- [ ] No secrets in code or config files
- [ ] `.gitignore` is comprehensive
- [ ] All passwords are placeholders
- [ ] Certificates not included
- [ ] Excel files not included
- [ ] User secrets outside repository
- [ ] Codacy scan passed
- [ ] Build successful

---

## 📚 Resources

- [Microsoft Identity Platform Best Practices](https://learn.microsoft.com/en-us/azure/active-directory/develop/identity-platform-integration-checklist)
- [Azure Security Best Practices](https://learn.microsoft.com/en-us/azure/security/fundamentals/best-practices-and-patterns)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [GitHub Secret Scanning](https://docs.github.com/en/code-security/secret-scanning/about-secret-scanning)

---

**Last Updated:** 2026-01-03  
**Version:** 1.0.0
