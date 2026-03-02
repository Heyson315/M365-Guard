using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace M365_Guard.Services;

/// <summary>
/// Service for managing and auditing Intune-managed devices
/// </summary>
public class IntuneDeviceService
{
    private readonly GraphServiceClient _graphClient;
    private readonly IConfiguration _configuration;

    public IntuneDeviceService(AuthenticationService authService, IConfiguration configuration)
    {
        _graphClient = authService.GetGraphClient();
        _configuration = configuration;
    }

    #region Device Queries

    public async Task<List<ManagedDevice>> GetManagedDevicesAsync()
    {
        try
        {
            var devices = await _graphClient.DeviceManagement.ManagedDevices.GetAsync();
            return devices?.Value ?? new List<ManagedDevice>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting managed devices: {ex.Message}");
            return new List<ManagedDevice>();
        }
    }

    public async Task<List<ManagedDevice>> GetNonCompliantDevicesAsync()
    {
        try
        {
            var devices = await _graphClient.DeviceManagement.ManagedDevices
                .GetAsync(config =>
                {
                    config.QueryParameters.Filter = "complianceState eq 'noncompliant'";
                });
            return devices?.Value ?? new List<ManagedDevice>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting non-compliant devices: {ex.Message}");
            return new List<ManagedDevice>();
        }
    }

    public async Task<List<ManagedDevice>> GetStaleDevicesAsync(int daysInactive = 30)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysInactive);
            var devices = await _graphClient.DeviceManagement.ManagedDevices
                .GetAsync(config =>
                {
                    config.QueryParameters.Filter = $"lastSyncDateTime lt {cutoffDate:yyyy-MM-ddTHH:mm:ssZ}";
                });
            return devices?.Value ?? new List<ManagedDevice>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting stale devices: {ex.Message}");
            return new List<ManagedDevice>();
        }
    }

    #endregion

    #region Device Audit

    public async Task RunDeviceAuditAsync()
    {
        Console.WriteLine("\n═══════════════════════════════════════════════════════");
        Console.WriteLine("   🔍 Intune Device Audit - Agent Scan Results");
        Console.WriteLine("═══════════════════════════════════════════════════════\n");

        await Task.WhenAll(
            ScanManagedDevicesAsync(),
            ScanNonCompliantDevicesAsync(),
            ScanStaleDevicesAsync()
        );

        Console.WriteLine("\n═══════════════════════════════════════════════════════");
        Console.WriteLine("   ✅ Device Audit Complete");
        Console.WriteLine("═══════════════════════════════════════════════════════");
    }

    private async Task ScanManagedDevicesAsync()
    {
        Console.WriteLine("📱 Scanning managed devices...");
        var devices = await GetManagedDevicesAsync();

        Console.WriteLine($"   Total Managed Devices: {devices.Count}");
        Console.WriteLine($"   Windows: {devices.Count(d => d.OperatingSystem == "Windows")}");
        Console.WriteLine($"   iOS: {devices.Count(d => d.OperatingSystem == "iOS")}");
        Console.WriteLine($"   Android: {devices.Count(d => d.OperatingSystem == "Android")}");
    }

    private async Task ScanNonCompliantDevicesAsync()
    {
        Console.WriteLine("\n⚠️  Scanning non-compliant devices...");
        var nonCompliant = await GetNonCompliantDevicesAsync();

        Console.WriteLine($"   Non-Compliant Devices: {nonCompliant.Count}");

        if (nonCompliant.Any())
        {
            Console.WriteLine("\n   Critical Non-Compliant Devices:");
            foreach (var device in nonCompliant.Take(5))
            {
                Console.WriteLine($"      • {device.DeviceName} ({device.OperatingSystem}) - User: {device.UserPrincipalName}");
            }
        }
    }

    private async Task ScanStaleDevicesAsync()
    {
        Console.WriteLine("\n🕒 Scanning stale devices (30+ days)...");
        var stale = await GetStaleDevicesAsync(30);

        Console.WriteLine($"   Stale Devices: {stale.Count}");

        if (stale.Any())
        {
            Console.WriteLine("\n   Devices Requiring Attention:");
            foreach (var device in stale.Take(5))
            {
                var daysSinceSync = (DateTime.UtcNow - device.LastSyncDateTime.GetValueOrDefault()).Days;
                Console.WriteLine($"      • {device.DeviceName} - Last sync: {daysSinceSync} days ago");
            }
        }
    }

    #endregion
}
