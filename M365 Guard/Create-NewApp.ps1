# Create M365Guard Conditional Access Analyzer App
# Quick setup script - creates new app with all permissions

Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "   Creating M365Guard-CA-Analyzer-2026" -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

# Create the app registration
Write-Host "📝 Creating app registration..." -ForegroundColor Yellow
$app = az ad app create --display-name "M365Guard-CA-Analyzer-2026" --query "{appId:appId, id:id}" -o json | ConvertFrom-Json

$appId = $app.appId
$objectId = $app.id

Write-Host "✅ App created!" -ForegroundColor Green
Write-Host "   App Name: M365Guard-CA-Analyzer-2026" -ForegroundColor White
Write-Host "   App ID: $appId" -ForegroundColor White
Write-Host ""

# Create client secret
Write-Host "🔐 Creating client secret..." -ForegroundColor Yellow
$secret = az ad app credential reset --id $appId --append --query password -o tsv

Write-Host "✅ Secret created!" -ForegroundColor Green
Write-Host ""

# Add Microsoft Graph API permissions
Write-Host "🔑 Adding API permissions..." -ForegroundColor Yellow

# Microsoft Graph App ID
$graphAppId = "00000003-0000-0000-c000-000000000000"

# Application permissions (requires admin consent)
$permissions = @(
    "df021288-bdef-4463-88db-98f22de89214", # User.Read.All
    "7ab1d382-f21e-4acd-a863-ba3e13f7da61", # Directory.Read.All
    "b0afded3-3588-46d8-8b3d-9842eff778da", # AuditLog.Read.All
    "5ac13192-7ace-4fcf-b828-1a26f28068ee", # Device.Read.All
    "dc5007c0-2d7d-4c42-879c-2dab87571379", # IdentityRiskyUser.Read.All
    "9e640839-a198-48fb-8b9a-013fd6f6cbcd", # DeviceManagementManagedDevices.Read.All
    "246dd0d5-5bd0-4def-940b-0421030a5b68"  # Policy.Read.All (for CA policies)
)

foreach ($permission in $permissions) {
    az ad app permission add --id $appId --api $graphAppId --api-permissions "$permission=Role" | Out-Null
}

Write-Host "✅ Permissions added!" -ForegroundColor Green
Write-Host ""

# Grant admin consent
Write-Host "👑 Granting admin consent..." -ForegroundColor Yellow
az ad app permission admin-consent --id $appId

Write-Host "✅ Admin consent granted!" -ForegroundColor Green
Write-Host ""

# Get tenant ID
$tenantId = az account show --query tenantId -o tsv

# Display results
Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Green
Write-Host "   ✅ Setup Complete!" -ForegroundColor Green
Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Green
Write-Host ""
Write-Host "📋 YOUR CREDENTIALS:" -ForegroundColor Yellow
Write-Host ""
Write-Host "Tenant ID:     " -NoNewline -ForegroundColor White
Write-Host "$tenantId" -ForegroundColor Cyan
Write-Host "Client ID:     " -NoNewline -ForegroundColor White
Write-Host "$appId" -ForegroundColor Cyan
Write-Host "Client Secret: " -NoNewline -ForegroundColor White
Write-Host "$secret" -ForegroundColor Cyan
Write-Host ""

Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "   Copy these into appsettings.json" -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

Write-Host @"
{
  "AzureAd": {
    "TenantId": "$tenantId",
    "ClientId": "$appId",
    "ClientSecret": "$secret"
  }
}
"@ -ForegroundColor Yellow

Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
