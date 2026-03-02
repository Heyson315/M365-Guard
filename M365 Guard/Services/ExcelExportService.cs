using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace M365_Guard.Services;

/// <summary>
/// Service for exporting Microsoft 365 data to Excel
/// Exports to Dashboard for Kings.xlsm in E:\source
/// This file also serves as a relationship table builder for other reporting
/// </summary>
public class ExcelExportService
{
    private readonly IConfiguration _configuration;
    private readonly string _excelFilePath;

    public ExcelExportService(IConfiguration configuration)
    {
        _configuration = configuration;
        
        // Use configured path, or default to E:\source location
        _excelFilePath = configuration["ExcelExport:OutputPath"] ?? 
            @"E:\source\Dashboard for Kings.xlsm";
        
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    #region Sign-Ins Export

    /// <summary>
    /// Export NonInteractive Sign-Ins to Excel
    /// </summary>
    public async Task ExportNonInteractiveSignInsAsync(List<SignIn> signIns)
    {
        Console.WriteLine($"\n📊 Exporting {signIns.Count} non-interactive sign-ins to Excel...");

        FileInfo fileInfo = new(_excelFilePath);
        using var package = new ExcelPackage(fileInfo);

        // Remove existing worksheet if it exists
        var existingWorksheet = package.Workbook.Worksheets["NonInteractiveSignIns"];
        if (existingWorksheet != null)
        {
            package.Workbook.Worksheets.Delete(existingWorksheet);
        }

        // Create new worksheet
        var worksheet = package.Workbook.Worksheets.Add("NonInteractiveSignIns");

        // Add headers
        string[] headers = {
            "Date/Time", "User", "UPN", "Application", "Client App", 
            "IP Address", "City", "State", "Country", "Status", 
            "Error Code", "Device", "OS", "Browser", "Risk Level"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
        }

        // Style headers
        using (var headerRange = worksheet.Cells[1, 1, 1, headers.Length])
        {
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(68, 114, 196));
            headerRange.Style.Font.Color.SetColor(Color.White);
        }

        // Add data
        int row = 2;
        foreach (var signIn in signIns)
        {
            worksheet.Cells[row, 1].Value = signIn.CreatedDateTime?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            worksheet.Cells[row, 2].Value = signIn.UserDisplayName;
            worksheet.Cells[row, 3].Value = signIn.UserPrincipalName;
            worksheet.Cells[row, 4].Value = signIn.AppDisplayName;
            worksheet.Cells[row, 5].Value = signIn.ClientAppUsed;
            worksheet.Cells[row, 6].Value = signIn.IpAddress;
            worksheet.Cells[row, 7].Value = signIn.Location?.City;
            worksheet.Cells[row, 8].Value = signIn.Location?.State;
            worksheet.Cells[row, 9].Value = signIn.Location?.CountryOrRegion;
            worksheet.Cells[row, 10].Value = signIn.Status?.ErrorCode == 0 ? "Success" : "Failed";
            worksheet.Cells[row, 11].Value = signIn.Status?.ErrorCode;
            worksheet.Cells[row, 12].Value = signIn.DeviceDetail?.DisplayName;
            worksheet.Cells[row, 13].Value = signIn.DeviceDetail?.OperatingSystem;
            worksheet.Cells[row, 14].Value = signIn.DeviceDetail?.Browser;
            worksheet.Cells[row, 15].Value = signIn.RiskLevelDuringSignIn?.ToString() ?? "None";

            row++;
        }

        // Auto-fit columns
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        // Add filters
        worksheet.Cells[1, 1, row - 1, headers.Length].AutoFilter = true;

        // Freeze header row
        worksheet.View.FreezePanes(2, 1);

        await package.SaveAsync();
        Console.WriteLine($"✅ Exported to: {_excelFilePath}");
    }

    /// <summary>
    /// Export Interactive Sign-Ins to Excel
    /// </summary>
    public async Task ExportInteractiveSignInsAsync(List<SignIn> signIns)
    {
        Console.WriteLine($"\n📊 Exporting {signIns.Count} interactive sign-ins to Excel...");

        FileInfo fileInfo = new(_excelFilePath);
        using var package = new ExcelPackage(fileInfo);

        var existingWorksheet = package.Workbook.Worksheets["InteractiveSignIns"];
        if (existingWorksheet != null)
        {
            package.Workbook.Worksheets.Delete(existingWorksheet);
        }

        var worksheet = package.Workbook.Worksheets.Add("InteractiveSignIns");

        // Similar structure as NonInteractive but optimized for user logins
        string[] headers = {
            "Date/Time", "User", "UPN", "Application", "Status", 
            "IP Address", "Location", "Device", "Risk Level", "Conditional Access"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
        }

        // Style headers
        using (var headerRange = worksheet.Cells[1, 1, 1, headers.Length])
        {
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(68, 114, 196));
            headerRange.Style.Font.Color.SetColor(Color.White);
        }

        int row = 2;
        foreach (var signIn in signIns)
        {
            worksheet.Cells[row, 1].Value = signIn.CreatedDateTime?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            worksheet.Cells[row, 2].Value = signIn.UserDisplayName;
            worksheet.Cells[row, 3].Value = signIn.UserPrincipalName;
            worksheet.Cells[row, 4].Value = signIn.AppDisplayName;
            worksheet.Cells[row, 5].Value = signIn.Status?.ErrorCode == 0 ? "Success" : "Failed";
            worksheet.Cells[row, 6].Value = signIn.IpAddress;
            
            var location = signIn.Location;
            var locationStr = location != null 
                ? $"{location.City}, {location.CountryOrRegion}"
                : "Unknown";
            worksheet.Cells[row, 7].Value = locationStr;
            
            worksheet.Cells[row, 8].Value = signIn.DeviceDetail?.DisplayName ?? signIn.DeviceDetail?.OperatingSystem;
            worksheet.Cells[row, 9].Value = signIn.RiskLevelDuringSignIn?.ToString() ?? "None";
            worksheet.Cells[row, 10].Value = signIn.ConditionalAccessStatus?.ToString() ?? "N/A";

            row++;
        }

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        worksheet.Cells[1, 1, row - 1, headers.Length].AutoFilter = true;
        worksheet.View.FreezePanes(2, 1);

        await package.SaveAsync();
        Console.WriteLine($"✅ Exported to: {_excelFilePath}");
    }

    /// <summary>
    /// Export Failed Sign-Ins to Excel
    /// </summary>
    public async Task ExportFailedSignInsAsync(List<SignIn> signIns)
    {
        Console.WriteLine($"\n📊 Exporting {signIns.Count} failed sign-ins to Excel...");

        FileInfo fileInfo = new(_excelFilePath);
        using var package = new ExcelPackage(fileInfo);

        var existingWorksheet = package.Workbook.Worksheets["FailedSignIns"];
        if (existingWorksheet != null)
        {
            package.Workbook.Worksheets.Delete(existingWorksheet);
        }

        var worksheet = package.Workbook.Worksheets.Add("FailedSignIns");

        string[] headers = {
            "Date/Time", "User", "UPN", "Application", "Error Code", 
            "Failure Reason", "IP Address", "Location", "Device"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
        }

        // Red header for failed sign-ins
        using (var headerRange = worksheet.Cells[1, 1, 1, headers.Length])
        {
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 0, 0));
            headerRange.Style.Font.Color.SetColor(Color.White);
        }

        int row = 2;
        foreach (var signIn in signIns)
        {
            worksheet.Cells[row, 1].Value = signIn.CreatedDateTime?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            worksheet.Cells[row, 2].Value = signIn.UserDisplayName;
            worksheet.Cells[row, 3].Value = signIn.UserPrincipalName;
            worksheet.Cells[row, 4].Value = signIn.AppDisplayName;
            worksheet.Cells[row, 5].Value = signIn.Status?.ErrorCode;
            worksheet.Cells[row, 6].Value = signIn.Status?.FailureReason;
            worksheet.Cells[row, 7].Value = signIn.IpAddress;
            
            var location = signIn.Location;
            var locationStr = location != null 
                ? $"{location.City}, {location.CountryOrRegion}"
                : "Unknown";
            worksheet.Cells[row, 8].Value = locationStr;
            
            worksheet.Cells[row, 9].Value = signIn.DeviceDetail?.DisplayName ?? signIn.DeviceDetail?.OperatingSystem;

            row++;
        }

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        worksheet.Cells[1, 1, row - 1, headers.Length].AutoFilter = true;
        worksheet.View.FreezePanes(2, 1);

        await package.SaveAsync();
        Console.WriteLine($"✅ Exported to: {_excelFilePath}");
    }

    #endregion

    #region Risky Users Export

    public async Task ExportRiskyUsersAsync(List<RiskyUser> riskyUsers)
    {
        Console.WriteLine($"\n📊 Exporting {riskyUsers.Count} risky users to Excel...");

        FileInfo fileInfo = new(_excelFilePath);
        using var package = new ExcelPackage(fileInfo);

        var existingWorksheet = package.Workbook.Worksheets["RiskyUsers"];
        if (existingWorksheet != null)
        {
            package.Workbook.Worksheets.Delete(existingWorksheet);
        }

        var worksheet = package.Workbook.Worksheets.Add("RiskyUsers");

        string[] headers = {
            "User", "UPN", "Risk Level", "Risk State", "Risk Detail", "Last Updated"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
        }

        using (var headerRange = worksheet.Cells[1, 1, 1, headers.Length])
        {
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 165, 0)); // Orange
            headerRange.Style.Font.Color.SetColor(Color.White);
        }

        int row = 2;
        foreach (var user in riskyUsers)
        {
            worksheet.Cells[row, 1].Value = user.UserDisplayName;
            worksheet.Cells[row, 2].Value = user.UserPrincipalName;
            worksheet.Cells[row, 3].Value = user.RiskLevel?.ToString();
            worksheet.Cells[row, 4].Value = user.RiskState?.ToString();
            worksheet.Cells[row, 5].Value = user.RiskDetail?.ToString();
            worksheet.Cells[row, 6].Value = user.RiskLastUpdatedDateTime?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");

            row++;
        }

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        worksheet.Cells[1, 1, row - 1, headers.Length].AutoFilter = true;
        worksheet.View.FreezePanes(2, 1);

        await package.SaveAsync();
        Console.WriteLine($"✅ Exported to: {_excelFilePath}");
    }

    #endregion

    #region Devices Export

    public async Task ExportManagedDevicesAsync(List<ManagedDevice> devices)
    {
        Console.WriteLine($"\n📊 Exporting {devices.Count} managed devices to Excel...");

        FileInfo fileInfo = new(_excelFilePath);
        using var package = new ExcelPackage(fileInfo);

        var existingWorksheet = package.Workbook.Worksheets["ManagedDevices"];
        if (existingWorksheet != null)
        {
            package.Workbook.Worksheets.Delete(existingWorksheet);
        }

        var worksheet = package.Workbook.Worksheets.Add("ManagedDevices");

        string[] headers = {
            "Device Name", "User", "OS", "OS Version", "Compliance State", 
            "Last Sync", "Serial Number", "Model", "Manufacturer"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
        }

        using (var headerRange = worksheet.Cells[1, 1, 1, headers.Length])
        {
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(68, 114, 196));
            headerRange.Style.Font.Color.SetColor(Color.White);
        }

        int row = 2;
        foreach (var device in devices)
        {
            worksheet.Cells[row, 1].Value = device.DeviceName;
            worksheet.Cells[row, 2].Value = device.UserPrincipalName;
            worksheet.Cells[row, 3].Value = device.OperatingSystem;
            worksheet.Cells[row, 4].Value = device.OsVersion;
            worksheet.Cells[row, 5].Value = device.ComplianceState?.ToString();
            worksheet.Cells[row, 6].Value = device.LastSyncDateTime?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            worksheet.Cells[row, 7].Value = device.SerialNumber;
            worksheet.Cells[row, 8].Value = device.Model;
            worksheet.Cells[row, 9].Value = device.Manufacturer;

            row++;
        }

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        worksheet.Cells[1, 1, row - 1, headers.Length].AutoFilter = true;
        worksheet.View.FreezePanes(2, 1);

        await package.SaveAsync();
        Console.WriteLine($"✅ Exported to: {_excelFilePath}");
    }

    #endregion
}
