using System.Collections.Generic;

namespace M365_Guard.Services;

/// <summary>
/// Provides context-aware help for the M365 Guard application
/// </summary>
public class HelpService
{
    private readonly Dictionary<string, string> _helpTopics = new()
    {
        { "sign-in", "📊 SIGN-IN AUDITS\n" +
            "Export and analyze Microsoft 365 sign-in events:\n" +
            "  • NonInteractive: Service principals, automated processes\n" +
            "  • Interactive: User logins with context\n" +
            "  • Failed: Authentication failures (security audit)\n" +
            "  • Risky: Sign-ins flagged by identity protection\n\n" +
            "💡 TIP: Use option 5 to export all reports at once" },

        { "conditional-access", "🔐 CONDITIONAL ACCESS\n" +
            "Audit and analyze Conditional Access policies:\n" +
            "  • View all CA policies (enabled/disabled)\n" +
            "  • Analyze CA coverage gaps\n" +
            "  • Detect CA failures in sign-ins\n" +
            "  • Identify security recommendations\n\n" +
            "💡 TIP: Run this audit regularly to ensure MFA is enforced" },

        { "device-audit", "📱 DEVICE MANAGEMENT\n" +
            "Monitor Intune-managed devices:\n" +
            "  • View managed devices inventory\n" +
            "  • Identify non-compliant devices\n" +
            "  • Find stale devices (30+ days inactive)\n" +
            "  • Export detailed device reports\n\n" +
            "💡 TIP: Run device audit weekly to catch compliance issues early" },

        { "security-scan", "🛡️  FULL SECURITY SCAN\n" +
            "Run comprehensive security checks in parallel:\n" +
            "  • Intune device audit\n" +
            "  • Entra ID security scan\n" +
            "  • Risky user detection\n" +
            "  • All operations run simultaneously\n\n" +
            "💡 TIP: This can take 2-3 minutes. You can ask questions while it runs!" },

        { "user-management", "👥 USER MANAGEMENT\n" +
            "Manage user accounts and security:\n" +
            "  • Disable compromised accounts\n" +
            "  • Export risky users identified by Entra ID\n" +
            "  • Revoke active sessions\n" +
            "  • View identity protection recommendations\n\n" +
            "⚠️  WARNING: Disabling users is permanent. Verify before confirming" },

        { "excel-export", "📊 EXCEL EXPORTS\n" +
            "All reports automatically export to:\n" +
            "  📁 E:\\source\\Dashboard for Kings.xlsm\n\n" +
            "Reports included:\n" +
            "  • NonInteractive Sign-Ins\n" +
            "  • Interactive Sign-Ins (with CA status)\n" +
            "  • Failed Sign-Ins\n" +
            "  • Risky Sign-Ins\n" +
            "  • Managed Devices\n" +
            "  • Risky Users\n\n" +
            "💡 TIP: Open the file in Excel while the app is running for live updates" },

        { "authentication", "🔐 AUTHENTICATION\n" +
            "M365 Guard uses certificate-based auth:\n\n" +
            "Priority order:\n" +
            "  1. Azure Key Vault certificate (production)\n" +
            "  2. Local certificate store\n" +
            "  3. Azure CLI credentials (development)\n\n" +
            "✅ NO CLIENT SECRETS required!\n\n" +
            "💡 Quick start: Run 'az login' then start the app" },

        { "shortcuts", "⌨️  KEYBOARD SHORTCUTS\n" +
            "  ? or help    - Show this help menu\n" +
            "  0            - Exit application\n" +
            "  Ctrl+C       - Cancel current operation\n\n" +
            "💡 TIP: Type '?' anytime from the main menu for help" },

        { "performance", "⚡ PERFORMANCE TIPS\n" +
            "Make M365 Guard faster:\n\n" +
            "  • First run may take 2-3 minutes (API calls)\n" +
            "  • Subsequent runs are cached\n" +
            "  • Run during off-hours for large tenants\n" +
            "  • Use 'Full Security Scan' for parallel execution\n" +
            "  • Close other apps to free up resources\n\n" +
            "💡 TIP: The app shows progress while running" },

        { "troubleshooting", "🔧 TROUBLESHOOTING\n" +
            "Common issues and fixes:\n\n" +
            "❌ \"Configuration not found\"\n" +
            "   → Set AzureAd:TenantId and AzureAd:ClientId in appsettings.json\n\n" +
            "❌ \"Authentication failed\"\n" +
            "   → Run 'az login' and authenticate\n" +
            "   → Check app registration permissions\n\n" +
            "❌ \"Excel file locked\"\n" +
            "   → Close Dashboard for Kings.xlsm in Excel\n" +
            "   → Try again\n\n" +
            "❌ \"Connection timeout\"\n" +
            "   → Check internet connection\n" +
            "   → Try again in a few moments\n\n" +
            "💡 TIP: Check SECURITY.md for detailed documentation" },

        { "permissions", "🔑 REQUIRED PERMISSIONS\n" +
            "M365 Guard requires these Microsoft Graph permissions:\n\n" +
            "  • AuditLog.Read.All - Read sign-in logs\n" +
            "  • Directory.Read.All - Read directory data\n" +
            "  • Device.Read.All - Read device info\n" +
            "  • Policy.Read.All - Read CA policies\n" +
            "  • IdentityRiskyUser.Read.All - Read risky users\n" +
            "  • DeviceManagementManagedDevices.Read.All - Read Intune devices\n" +
            "  • User.ReadWrite.All - Disable accounts\n\n" +
            "💡 TIP: Grant admin consent in Azure AD app registration" }
    };

    /// <summary>
    /// Display main help menu
    /// </summary>
    public void ShowMainHelp()
    {
        Console.Clear();
        Console.WriteLine("═══════════════════════════════════════════════════════");
        Console.WriteLine("   ❓ M365 Guard Help & Documentation");
        Console.WriteLine("═══════════════════════════════════════════════════════\n");

        Console.WriteLine("📚 Help Topics:\n");
        Console.WriteLine("  1. 📊 Sign-In Audits");
        Console.WriteLine("  2. 🔐 Conditional Access");
        Console.WriteLine("  3. 📱 Device Management");
        Console.WriteLine("  4. 🛡️  Full Security Scan");
        Console.WriteLine("  5. 👥 User Management");
        Console.WriteLine("  6. 📊 Excel Exports");
        Console.WriteLine("  7. 🔐 Authentication");
        Console.WriteLine("  8. ⌨️  Keyboard Shortcuts");
        Console.WriteLine("  9. ⚡ Performance Tips");
        Console.WriteLine("  10. 🔧 Troubleshooting");
        Console.WriteLine("  11. 🔑 Required Permissions");
        Console.WriteLine("  12. 📖 View README.md");
        Console.WriteLine("\n  0. Back to Main Menu\n");

        Console.Write("Select a topic (0-12): ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1": DisplayTopic("sign-in"); break;
            case "2": DisplayTopic("conditional-access"); break;
            case "3": DisplayTopic("device-audit"); break;
            case "4": DisplayTopic("security-scan"); break;
            case "5": DisplayTopic("user-management"); break;
            case "6": DisplayTopic("excel-export"); break;
            case "7": DisplayTopic("authentication"); break;
            case "8": DisplayTopic("shortcuts"); break;
            case "9": DisplayTopic("performance"); break;
            case "10": DisplayTopic("troubleshooting"); break;
            case "11": DisplayTopic("permissions"); break;
            case "12": DisplayReadme(); break;
            case "0": break;
            default:
                Console.WriteLine("❌ Invalid option");
                break;
        }
    }

    /// <summary>
    /// Display a specific help topic
    /// </summary>
    public void DisplayTopic(string topic)
    {
        Console.Clear();
        Console.WriteLine("═══════════════════════════════════════════════════════");

        if (_helpTopics.TryGetValue(topic.ToLower(), out var content))
        {
            Console.WriteLine(content);
        }
        else
        {
            Console.WriteLine($"❌ Help topic '{topic}' not found");
        }

        Console.WriteLine("\n═══════════════════════════════════════════════════════");
        Console.Write("Press any key to return to help menu...");
        Console.ReadKey();
        ShowMainHelp();
    }

    /// <summary>
    /// Display quick help for a menu option
    /// </summary>
    public void ShowQuickHelp(int menuOption)
    {
        var quickHelp = menuOption switch
        {
            1 => "📊 Exports non-interactive sign-ins (service principals, scheduled tasks)",
            2 => "📊 Exports interactive sign-ins with Conditional Access status",
            3 => "📊 Exports failed authentication attempts (potential security issues)",
            4 => "📊 Exports risky sign-ins flagged by Entra ID protection",
            5 => "📊 Exports all sign-in reports in one operation",
            6 => "📱 Scans Intune managed devices, compliance, and stale devices",
            7 => "🛡️  Scans Entra ID for risky users and security issues",
            8 => "🚀 Runs all security audits in parallel (takes 2-3 minutes)",
            9 => "📱 Exports inventory of all Intune-managed devices",
            10 => "⚠️  Exports devices that don't meet compliance policies",
            11 => "🚫 Disables a user account (security incident response)",
            12 => "👥 Exports users flagged as risky by Entra ID",
            13 => "🔐 Audits Conditional Access policies and coverage gaps",
            14 => "⚙️  Shows current configuration (tenant, auth method, etc.)",
            _ => "❓ Unknown option"
        };

        Console.WriteLine($"\n💡 {quickHelp}");
    }

    /// <summary>
    /// Display README.md
    /// </summary>
    private void DisplayReadme()
    {
        Console.Clear();
        Console.WriteLine("═══════════════════════════════════════════════════════");
        Console.WriteLine("   📖 README.md");
        Console.WriteLine("═══════════════════════════════════════════════════════\n");
        Console.WriteLine("📁 Location: M365 Guard/README.md\n");
        Console.WriteLine("The README contains:\n");
        Console.WriteLine("  ✅ Project overview");
        Console.WriteLine("  ✅ Setup instructions");
        Console.WriteLine("  ✅ Authentication methods");
        Console.WriteLine("  ✅ Menu options reference");
        Console.WriteLine("  ✅ Excel output format");
        Console.WriteLine("  ✅ Security best practices");
        Console.WriteLine("\n💡 Open README.md in your text editor for full details\n");
        Console.WriteLine("═══════════════════════════════════════════════════════");
        Console.Write("Press any key to return to help menu...");
        Console.ReadKey();
        ShowMainHelp();
    }

    /// <summary>
    /// Show context-aware help hint
    /// </summary>
    public void ShowHintForMenu()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("💡 Tip: Type '?' at any menu for detailed help");
        Console.ResetColor();
    }
}
