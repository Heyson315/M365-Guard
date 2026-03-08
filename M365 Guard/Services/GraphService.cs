using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace M365_Guard.Services;

/// <summary>
/// Core Graph API service for querying Microsoft 365 data
/// Handles sign-ins, users, devices, and security audits
/// </summary>
public class GraphService
{
    private readonly GraphServiceClient _graphClient;
    private readonly IConfiguration _configuration;

    public GraphService(AuthenticationService authService, IConfiguration configuration)
    {
        _graphClient = authService.GetGraphClient();
        _configuration = configuration;
    }

    public GraphServiceClient GetGraphClient() => _graphClient;

    #region Organization & Basic Info

    public async Task<Organization?> GetOrganizationAsync()
    {
        try
        {
            var organization = await _graphClient.Organization.GetAsync();
            return organization?.Value?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting organization: {ex.Message}");
            return null;
        }
    }

    public async Task TestConnectionAsync()
    {
        try
        {
            var org = await GetOrganizationAsync();
            if (org != null)
            {
                Console.WriteLine($"✓ Successfully connected to: {org.DisplayName}");
                Console.WriteLine($"  Tenant ID: {org.Id}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Connection failed: {ex.Message}");
        }
    }

    #endregion

    #region Sign-In Logs

    /// <summary>
    /// Get all sign-ins for a specified date range
    /// </summary>
    public async Task<List<SignIn>> GetSignInsAsync(int daysBack = 7)
    {
        var allSignIns = new List<SignIn>();
        try
        {
            var startDate = DateTime.UtcNow.AddDays(-daysBack);
            var filter = $"createdDateTime ge {startDate:yyyy-MM-ddTHH:mm:ssZ}";

            var response = await _graphClient.AuditLogs.SignIns
                .GetAsync(config =>
                {
                    config.QueryParameters.Filter = filter;
                    config.QueryParameters.Top = 999;
                    config.QueryParameters.Orderby = new[] { "createdDateTime desc" };
                });

            var pageIterator = PageIterator<SignIn, SignInCollectionResponse>
                .CreatePageIterator(_graphClient, response!, (signIn) =>
                {
                    allSignIns.Add(signIn);
                    return true;
                });

            await pageIterator.IterateAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting sign-ins: {ex.Message}");
        }

        return allSignIns;
    }

    /// <summary>
    /// Get non-interactive sign-ins (service principals, apps, etc.)
    /// </summary>
    public async Task<List<SignIn>> GetNonInteractiveSignInsAsync(int daysBack = 7)
    {
        var signIns = await GetSignInsAsync(daysBack);
        return signIns.Where(s => s.IsInteractive == false).ToList();
    }

    /// <summary>
    /// Get interactive user sign-ins
    /// </summary>
    public async Task<List<SignIn>> GetInteractiveSignInsAsync(int daysBack = 7)
    {
        var signIns = await GetSignInsAsync(daysBack);
        return signIns.Where(s => s.IsInteractive == true).ToList();
    }

    /// <summary>
    /// Get failed sign-in attempts
    /// </summary>
    public async Task<List<SignIn>> GetFailedSignInsAsync(int daysBack = 7)
    {
        var signIns = await GetSignInsAsync(daysBack);
        return signIns.Where(s => s.Status?.ErrorCode != 0).ToList();
    }

    /// <summary>
    /// Get risky sign-ins
    /// </summary>
    public async Task<List<SignIn>> GetRiskySignInsAsync(int daysBack = 7)
    {
        var signIns = await GetSignInsAsync(daysBack);
        return signIns.Where(s => 
            s.RiskLevelDuringSignIn != RiskLevel.None && 
            s.RiskLevelDuringSignIn != null).ToList();
    }

    #endregion

    #region Users

    public async Task<List<User>> GetUsersAsync(int top = 999)
    {
        try
        {
            var users = await _graphClient.Users
                .GetAsync(config =>
                {
                    config.QueryParameters.Top = top;
                    config.QueryParameters.Select = new[] { "displayName", "mail", "userPrincipalName", "accountEnabled", "createdDateTime" };
                });

            return users?.Value ?? new List<User>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting users: {ex.Message}");
            return new List<User>();
        }
    }

    public async Task<List<RiskyUser>> GetRiskyUsersAsync()
    {
        try
        {
            var riskyUsers = await _graphClient.IdentityProtection.RiskyUsers.GetAsync();
            return riskyUsers?.Value ?? new List<RiskyUser>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting risky users: {ex.Message}");
            return new List<RiskyUser>();
        }
    }

    #endregion

    #region Devices

    public async Task<List<Device>> GetDevicesAsync()
    {
        try
        {
            var devices = await _graphClient.Devices.GetAsync();
            return devices?.Value ?? new List<Device>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting devices: {ex.Message}");
            return new List<Device>();
        }
    }

    public async Task<List<Device>> GetNonCompliantEntraDevicesAsync()
    {
        try
        {
            var devices = await _graphClient.Devices
                .GetAsync(config =>
                {
                    config.QueryParameters.Filter = "isCompliant eq false";
                });
            return devices?.Value ?? new List<Device>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting non-compliant devices: {ex.Message}");
            return new List<Device>();
        }
    }

    #endregion

    #region Security Audit

    public async Task RunSecurityAuditAsync()
    {
        Console.WriteLine("\n═══════════════════════════════════════════════════════");
        Console.WriteLine("   🛡️  Entra ID Security Audit - Agent Scan Results");
        Console.WriteLine("═══════════════════════════════════════════════════════\n");

        var deviceTask = ScanEntraDevicesAsync();
        var riskyUserTask = ScanRiskyUsersAsync();
        var signInTask = ScanFailedSignInsAsync();

        await Task.WhenAll(deviceTask, riskyUserTask, signInTask);

        Console.WriteLine("\n═══════════════════════════════════════════════════════");
        Console.WriteLine("   ✅ Security Audit Complete");
        Console.WriteLine("═══════════════════════════════════════════════════════");
    }

    private async Task ScanEntraDevicesAsync()
    {
        Console.WriteLine("💻 Scanning Entra ID registered devices...");
        var devices = await GetDevicesAsync();
        var nonCompliant = await GetNonCompliantEntraDevicesAsync();

        Console.WriteLine($"   Total Devices: {devices.Count}");
        Console.WriteLine($"   Non-Compliant: {nonCompliant.Count}");

        if (nonCompliant.Any())
        {
            Console.WriteLine("\n   ⚠️  Non-Compliant Devices:");
            foreach (var device in nonCompliant.Take(5))
            {
                Console.WriteLine($"      • {device.DisplayName} ({device.OperatingSystem})");
            }
        }
    }

    private async Task ScanRiskyUsersAsync()
    {
        Console.WriteLine("\n👤 Scanning for risky users...");
        var riskyUsers = await GetRiskyUsersAsync();

        Console.WriteLine($"   Risky Users Found: {riskyUsers.Count}");

        if (riskyUsers.Any())
        {
            Console.WriteLine("\n   ⚠️  High-Risk Users:");
            foreach (var user in riskyUsers.Where(u => u.RiskLevel == RiskLevel.High).Take(5))
            {
                Console.WriteLine($"      • {user.UserDisplayName} ({user.UserPrincipalName}) - {user.RiskState}");
            }
        }
    }

    private async Task ScanFailedSignInsAsync()
    {
        Console.WriteLine("\n🔐 Scanning failed sign-in attempts...");
        var failed = await GetFailedSignInsAsync(7);

        Console.WriteLine($"   Failed Attempts (7 days): {failed.Count}");

        if (failed.Any())
        {
            var grouped = failed.GroupBy(s => s.UserPrincipalName).OrderByDescending(g => g.Count()).Take(5);
            Console.WriteLine("\n   ⚠️  Top Users with Failed Sign-Ins:");
            foreach (var group in grouped)
            {
                Console.WriteLine($"      • {group.Key}: {group.Count()} attempts");
            }
        }
    }

    #endregion

    #region User Management

    public async Task RunDisableUserWorkflowAsync()
    {
        Console.WriteLine("\n═══════════════════════════════════════════════════════");
        Console.WriteLine("   🚫 Disable User Account Workflow");
        Console.WriteLine("═══════════════════════════════════════════════════════\n");

        Console.Write("Enter User Principal Name (UPN) to disable: ");
        var upn = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(upn))
        {
            Console.WriteLine("⚠️  Invalid UPN provided.");
            return;
        }

        try
        {
            // Find user
            var users = await _graphClient.Users
                .GetAsync(config =>
                {
                    config.QueryParameters.Filter = $"userPrincipalName eq '{upn}'";
                });

            var user = users?.Value?.FirstOrDefault();
            if (user == null)
            {
                Console.WriteLine($"⚠️  User not found: {upn}");
                return;
            }

            Console.WriteLine($"\nFound: {user.DisplayName} ({user.UserPrincipalName})");
            Console.Write("Are you sure you want to disable this account? (yes/no): ");
            var confirm = Console.ReadLine();

            if (confirm?.ToLower() != "yes")
            {
                Console.WriteLine("❌ Operation cancelled.");
                return;
            }

            // Disable the account
            var updateUser = new User
            {
                AccountEnabled = false
            };

            await _graphClient.Users[user.Id].PatchAsync(updateUser);
            Console.WriteLine($"✅ Account disabled: {user.DisplayName}");

            // Revoke all sessions
            await _graphClient.Users[user.Id].RevokeSignInSessions.PostAsRevokeSignInSessionsPostResponseAsync();
            Console.WriteLine("✅ All active sessions revoked");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    #endregion
}
