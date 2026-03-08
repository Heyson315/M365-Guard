using M365_Guard.Services;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

// Configure EPPlus license context
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Build configuration - supports Key Vault, certificates, and Azure CLI credentials
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables()
    .Build();

Console.WriteLine("═══════════════════════════════════════════════════════");
Console.WriteLine("   M365 Guard - Security & Audit Suite");
Console.WriteLine("   Rahman Finance and Accounting P.L.LC");
Console.WriteLine("═══════════════════════════════════════════════════════\n");

// Get Azure AD configuration
var tenantId = configuration["AzureAd:TenantId"];
var clientId = configuration["AzureAd:ClientId"];
var keyVaultUri = configuration["KeyVault:VaultUri"];
var isMultiTenant = configuration["ISVConfiguration:IsMultiTenant"]?.ToLower() == "true";

if (isMultiTenant)
{
    Console.WriteLine("🌐 Multi-Tenant Mode: ENABLED");
    Console.WriteLine("   Supports multiple customer tenants\n");
}

// Check configuration - only need TenantId and ClientId (no secrets required!)
bool isConfigValid = !string.IsNullOrEmpty(tenantId) &&
                     !string.IsNullOrEmpty(clientId);

if (!isConfigValid)
{
    Console.WriteLine("⚠️  Configuration not found!");
    Console.WriteLine("\nMinimal configuration required:\n");
    Console.WriteLine("In appsettings.json, set:");
    Console.WriteLine("  • AzureAd:TenantId - Your Azure AD tenant ID");
    Console.WriteLine("  • AzureAd:ClientId - Your app registration client ID\n");

    Console.WriteLine("Authentication Methods (in priority order):");
    Console.WriteLine("1. Certificate from Azure Key Vault (Production) ✅");
    Console.WriteLine("2. Certificate from local certificate store ✅");
    Console.WriteLine("3. Azure CLI credentials (Development) ✅\n");

    Console.WriteLine("No client secrets needed! 🔐\n");

    Console.WriteLine("Debug Info:");
    Console.WriteLine($"Tenant ID found: {!string.IsNullOrEmpty(tenantId)}");
    Console.WriteLine($"Client ID found: {!string.IsNullOrEmpty(clientId)}");

    Console.WriteLine("\n═══════════════════════════════════════════════════════");
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
    return;
}

try
{
    Console.WriteLine("🔧 Initializing services...");
    var authService = new AuthenticationService(configuration);
    var graphService = new GraphService(authService, configuration);
    var intuneService = new IntuneDeviceService(authService, configuration);
    var excelService = new ExcelExportService(configuration);

    Console.WriteLine("🔗 Testing Microsoft Graph connection...\n");
    await graphService.TestConnectionAsync();

    bool continueRunning = true;
    while (continueRunning)
    {
        Console.WriteLine("\n═══════════════════════════════════════════════════════");
        Console.WriteLine("   Main Menu");
        if (isMultiTenant)
        {
            Console.WriteLine("   🌐 Multi-Tenant Mode");
        }
        Console.WriteLine("═══════════════════════════════════════════════════════");

        Console.WriteLine("\n── Sign-In Audits ─────────────────────────────────────");
        Console.WriteLine("1. 📊 Export NonInteractive Sign-Ins to Excel");
        Console.WriteLine("2. 📊 Export Interactive Sign-Ins to Excel");
        Console.WriteLine("3. 📊 Export Failed Sign-Ins to Excel");
        Console.WriteLine("4. 📊 Export Risky Sign-Ins to Excel");
        Console.WriteLine("5. 📊 Export ALL Sign-In Reports");

        Console.WriteLine("\n── Security Audit Agents ──────────────────────────────");
        Console.WriteLine("6. 🔍 Run Intune Device Audit");
        Console.WriteLine("7. 🛡️  Run Entra ID Security Audit");
        Console.WriteLine("8. 🚀 Run Full Security Scan (Parallel)");

        Console.WriteLine("\n── Device Management ──────────────────────────────────");
        Console.WriteLine("9. 📱 Export Managed Devices to Excel");
        Console.WriteLine("10. ⚠️  Export Non-Compliant Devices to Excel");

        Console.WriteLine("\n── User Management ────────────────────────────────────");
        Console.WriteLine("11. 🚫 Disable User Account");
        Console.WriteLine("12. 👥 Export Risky Users to Excel");

        Console.WriteLine("\n── Configuration ──────────────────────────────────────");
        Console.WriteLine("13. ⚙️  View Current Configuration");

        Console.WriteLine("\n0. Exit");
        Console.Write("\nSelect an option: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Console.WriteLine("\n📥 Fetching non-interactive sign-ins...");
                var nonInteractive = await graphService.GetNonInteractiveSignInsAsync(30);
                Console.WriteLine($"   Found: {nonInteractive.Count} records");
                if (nonInteractive.Any())
                {
                    await excelService.ExportNonInteractiveSignInsAsync(nonInteractive);
                }
                break;

            case "2":
                Console.WriteLine("\n📥 Fetching interactive sign-ins...");
                var interactive = await graphService.GetInteractiveSignInsAsync(30);
                Console.WriteLine($"   Found: {interactive.Count} records");
                if (interactive.Any())
                {
                    await excelService.ExportInteractiveSignInsAsync(interactive);
                }
                break;

            case "3":
                Console.WriteLine("\n📥 Fetching failed sign-ins...");
                var failed = await graphService.GetFailedSignInsAsync(30);
                Console.WriteLine($"   Found: {failed.Count} records");
                if (failed.Any())
                {
                    await excelService.ExportFailedSignInsAsync(failed);
                }
                break;

            case "4":
                Console.WriteLine("\n📥 Fetching risky sign-ins...");
                var risky = await graphService.GetRiskySignInsAsync(30);
                Console.WriteLine($"   Found: {risky.Count} records");
                if (risky.Any())
                {
                    await excelService.ExportNonInteractiveSignInsAsync(risky);
                }
                break;

            case "5":
                Console.WriteLine("\n🚀 Exporting ALL Sign-In Reports...");
                var allNonInteractive = await graphService.GetNonInteractiveSignInsAsync(30);
                var allInteractive = await graphService.GetInteractiveSignInsAsync(30);
                var allFailed = await graphService.GetFailedSignInsAsync(30);

                if (allNonInteractive.Any())
                    await excelService.ExportNonInteractiveSignInsAsync(allNonInteractive);
                if (allInteractive.Any())
                    await excelService.ExportInteractiveSignInsAsync(allInteractive);
                if (allFailed.Any())
                    await excelService.ExportFailedSignInsAsync(allFailed);

                Console.WriteLine("\n✅ All sign-in reports exported!");
                break;

            case "6":
                Console.WriteLine("\n🔍 Queuing Intune Device Audit Agent...");
                await intuneService.RunDeviceAuditAsync();
                break;

            case "7":
                Console.WriteLine("\n🛡️  Queuing Entra ID Security Audit Agent...");
                await graphService.RunSecurityAuditAsync();
                break;

            case "8":
                Console.WriteLine("\n🚀 Running Full Security Scan (Parallel Agents)...");
                Console.WriteLine("   Deploying agents to scan devices and audit sign-ins simultaneously...\n");
                var intuneTask = intuneService.RunDeviceAuditAsync();
                var entraTask = graphService.RunSecurityAuditAsync();
                await Task.WhenAll(intuneTask, entraTask);
                Console.WriteLine("\n✅ All security agents completed.");
                break;

            case "9":
                Console.WriteLine("\n📥 Fetching managed devices...");
                var devices = await intuneService.GetManagedDevicesAsync();
                Console.WriteLine($"   Found: {devices.Count} devices");
                if (devices.Any())
                {
                    await excelService.ExportManagedDevicesAsync(devices);
                }
                break;

            case "10":
                Console.WriteLine("\n📥 Fetching non-compliant devices...");
                var nonCompliant = await intuneService.GetNonCompliantDevicesAsync();
                Console.WriteLine($"   Found: {nonCompliant.Count} devices");
                if (nonCompliant.Any())
                {
                    await excelService.ExportManagedDevicesAsync(nonCompliant);
                }
                break;

            case "11":
                await graphService.RunDisableUserWorkflowAsync();
                break;

            case "12":
                Console.WriteLine("\n📥 Fetching risky users...");
                var riskyUsers = await graphService.GetRiskyUsersAsync();
                Console.WriteLine($"   Found: {riskyUsers.Count} risky users");
                if (riskyUsers.Any())
                {
                    await excelService.ExportRiskyUsersAsync(riskyUsers);
                }
                break;

            case "13":
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("   Security Configuration");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                var authMethod = "Unknown";
                if (!string.IsNullOrEmpty(keyVaultUri))
                {
                    authMethod = "Certificate from Azure Key Vault 🔐";
                }
                else if (!string.IsNullOrEmpty(configuration["AzureAd:CertificateThumbprint"]))
                {
                    authMethod = "Certificate from Local Store 🔐";
                }
                else
                {
                    authMethod = "Azure CLI Credentials (Development) 🔑";
                }

                Console.WriteLine($"Authentication Method: {authMethod}");
                if (!string.IsNullOrEmpty(keyVaultUri))
                {
                    Console.WriteLine($"Key Vault URI: {keyVaultUri}");
                }
                Console.WriteLine($"Multi-Tenant: {isMultiTenant}");
                Console.WriteLine($"Tenant Mode: {(tenantId == "common" ? "Common (Multi-tenant)" : "Single Tenant")}");
                Console.WriteLine($"Tenant ID: {tenantId}");
                Console.WriteLine($"Client ID: {clientId}");
                Console.WriteLine($"Excel Output: {configuration["ExcelExport:OutputPath"]}");
                Console.WriteLine("\n✅ No client secrets stored anywhere!");
                break;

            case "0":
                continueRunning = false;
                break;

            default:
                Console.WriteLine("\n⚠️  Invalid option. Please try again.");
                break;
        }

        if (continueRunning && choice != "0")
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    Console.WriteLine("\n═══════════════════════════════════════════════════════");
    Console.WriteLine("Thank you for using M365 Guard!");
    Console.WriteLine("═══════════════════════════════════════════════════════\n");
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Error: {ex.Message}");
    Console.WriteLine($"   Type: {ex.GetType().Name}");

    if (ex.InnerException != null)
    {
        Console.WriteLine($"   Inner: {ex.InnerException.Message}");
    }

    Console.WriteLine("\nPlease verify your configuration and try again.");
    Console.WriteLine("\nTroubleshooting:");
    Console.WriteLine("• Ensure you're logged in: az login");
    Console.WriteLine("• Check Azure AD app registration settings");
    Console.WriteLine("• Verify API permissions (AuditLog.Read.All, DeviceManagementManagedDevices.Read.All)");

    Console.WriteLine("\nPress any key to exit...");
    Console.ReadKey();
}
