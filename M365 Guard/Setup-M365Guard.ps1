# M365 Guard Setup Script
# This script helps configure user secrets for the M365 Guard application

Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "   M365 Guard - Setup Wizard" -ForegroundColor Cyan
Write-Host "   Rahman Finance and Accounting P.L.LC" -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

# Check if we're in the right directory
if (-not (Test-Path "M365 Guard.csproj")) {
    Write-Host "❌ Error: M365 Guard.csproj not found" -ForegroundColor Red
    Write-Host "   Please run this script from the 'M365 Guard' directory" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "This wizard will help you configure Azure AD credentials using User Secrets" -ForegroundColor Green
Write-Host "(User Secrets are stored securely outside your project directory)" -ForegroundColor Gray
Write-Host ""

# Prompt for Azure AD details
Write-Host "Please enter your Azure AD App Registration details:" -ForegroundColor Yellow
Write-Host ""

$tenantId = Read-Host "Tenant ID (or 'common' for multi-tenant)"
$clientId = Read-Host "Client ID (Application ID)"
$clientSecret = Read-Host "Client Secret (App Secret Value)" -AsSecureString

# Convert SecureString to plain text for dotnet user-secrets
$BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($clientSecret)
$clientSecretPlain = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)

Write-Host ""
Write-Host "🔧 Configuring user secrets..." -ForegroundColor Cyan

# Set user secrets
try {
    dotnet user-secrets set "AzureAd:TenantId" "$tenantId"
    dotnet user-secrets set "AzureAd:ClientId" "$clientId"
    dotnet user-secrets set "AzureAd:ClientSecret" "$clientSecretPlain"
    
    Write-Host "✅ Configuration complete!" -ForegroundColor Green
    Write-Host ""
    
    # Ask about multi-tenant
    $multiTenant = Read-Host "Is this a multi-tenant application? (y/n)"
    if ($multiTenant -eq "y" -or $multiTenant -eq "Y") {
        dotnet user-secrets set "ISVConfiguration:IsMultiTenant" "true"
        Write-Host "✅ Multi-tenant mode enabled" -ForegroundColor Green
    } else {
        dotnet user-secrets set "ISVConfiguration:IsMultiTenant" "false"
        Write-Host "✅ Single-tenant mode enabled" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host "   Setup Complete!" -ForegroundColor Green
    Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Next Steps:" -ForegroundColor Yellow
    Write-Host "1. Ensure your Azure AD app has the required API permissions" -ForegroundColor White
    Write-Host "2. Grant admin consent for the permissions" -ForegroundColor White
    Write-Host "3. Run the application: dotnet run" -ForegroundColor White
    Write-Host ""
    
    Write-Host "Required API Permissions:" -ForegroundColor Yellow
    Write-Host "  • AuditLog.Read.All" -ForegroundColor White
    Write-Host "  • DeviceManagementManagedDevices.Read.All" -ForegroundColor White
    Write-Host "  • Directory.Read.All" -ForegroundColor White
    Write-Host "  • User.ReadWrite.All" -ForegroundColor White
    Write-Host "  • IdentityRiskyUser.Read.All" -ForegroundColor White
    Write-Host "  • Device.Read.All" -ForegroundColor White
    Write-Host ""
    
} catch {
    Write-Host "❌ Error configuring secrets: $_" -ForegroundColor Red
    Write-Host ""
}

# Clean up sensitive data
$clientSecretPlain = $null
[System.Runtime.InteropServices.Marshal]::ZeroFreeBSTR($BSTR)

Read-Host "Press Enter to exit"
