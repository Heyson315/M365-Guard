using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace M365_Guard.Services;

/// <summary>
/// Service for auditing and analyzing Conditional Access policies
/// Provides comprehensive CA policy reporting and failure analysis
/// </summary>
public class ConditionalAccessService
{
    private readonly GraphServiceClient _graphClient;
    private readonly IConfiguration _configuration;

    public ConditionalAccessService(AuthenticationService authService, IConfiguration configuration)
    {
        _graphClient = authService.GetGraphClient();
        _configuration = configuration;
    }

    #region CA Policy Queries

    /// <summary>
    /// Get all Conditional Access policies in the tenant
    /// </summary>
    public async Task<List<ConditionalAccessPolicy>> GetAllPoliciesAsync()
    {
        try
        {
            var policies = await _graphClient.Identity.ConditionalAccess.Policies.GetAsync();
            return policies?.Value ?? new List<ConditionalAccessPolicy>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting CA policies: {ex.Message}");
            return new List<ConditionalAccessPolicy>();
        }
    }

    /// <summary>
    /// Run comprehensive Conditional Access audit
    /// </summary>
    public async Task RunConditionalAccessAuditAsync()
    {
        Console.WriteLine("\n═══════════════════════════════════════════════════════");
        Console.WriteLine("   🔐 Conditional Access Audit - Policy Analysis");
        Console.WriteLine("═══════════════════════════════════════════════════════\n");

        // Get all policies
        Console.WriteLine("📋 Analyzing CA policies...");
        var policies = await GetAllPoliciesAsync();

        var enabledCount = policies.Count(p => p.State == ConditionalAccessPolicyState.Enabled);
        var disabledCount = policies.Count(p => p.State == ConditionalAccessPolicyState.Disabled);

        // Display policy summary
        Console.WriteLine($"\n   Total Policies: {policies.Count}");
        Console.WriteLine($"   ✅ Enabled: {enabledCount}");
        Console.WriteLine($"   ❌ Disabled: {disabledCount}");

        if (policies.Any())
        {
            Console.WriteLine("\n   📌 Active Policies:");
            foreach (var policy in policies.Where(p => p.State == ConditionalAccessPolicyState.Enabled).Take(5))
            {
                Console.WriteLine($"      • {policy.DisplayName}");
            }
        }

        // Analyze coverage
        Console.WriteLine("\n🛡️  Analyzing CA coverage gaps...");
        var coverage = AnalyzeCoverageGaps(policies);
        
        Console.WriteLine($"\n   MFA Enforced: {(coverage.MFAEnforced ? "✅ Yes" : "❌ No")}");
        Console.WriteLine($"   Legacy Auth Blocked: {(coverage.BlockLegacyAuthEnabled ? "✅ Yes" : "❌ No")}");
        Console.WriteLine($"   External Access Controlled: {(coverage.ExternalAccessControlled ? "✅ Yes" : "❌ No")}");

        if (coverage.CoverageGaps.Any())
        {
            Console.WriteLine("\n   ⚠️  Coverage Gaps Found:");
            foreach (var gap in coverage.CoverageGaps)
            {
                Console.WriteLine($"      {gap}");
            }
        }
        else
        {
            Console.WriteLine("\n   ✅ No coverage gaps detected!");
        }

        Console.WriteLine("\n═══════════════════════════════════════════════════════");
        Console.WriteLine("   ✅ Conditional Access Audit Complete");
        Console.WriteLine("═══════════════════════════════════════════════════════");
    }

    #endregion

    #region Analysis Methods

    /// <summary>
    /// Analyze CA coverage gaps in the tenant
    /// </summary>
    private CACoverageAnalysis AnalyzeCoverageGaps(List<ConditionalAccessPolicy> policies)
    {
        var analysis = new CACoverageAnalysis();
        var enabledPolicies = policies.Where(p => p.State == ConditionalAccessPolicyState.Enabled).ToList();

        // Check if MFA is enforced
        var mfaPolicies = enabledPolicies
            .Where(p => p.GrantControls?.BuiltInControls != null &&
                       p.GrantControls.BuiltInControls.Any(c => c?.ToString()?.Contains("mfa", StringComparison.OrdinalIgnoreCase) == true))
            .ToList();
        analysis.MFAEnforced = mfaPolicies.Count > 0;
        analysis.MFAPolicyCount = mfaPolicies.Count;

        // Check if block legacy auth is enabled
        var blockLegacyPolicies = enabledPolicies
            .Where(p => p.GrantControls?.BuiltInControls != null &&
                       p.GrantControls.BuiltInControls.Any(c => c?.ToString()?.Contains("block", StringComparison.OrdinalIgnoreCase) == true))
            .ToList();
        analysis.BlockLegacyAuthEnabled = blockLegacyPolicies.Count > 0;

        // Identify gaps
        if (!analysis.MFAEnforced)
            analysis.CoverageGaps.Add("⚠️  MFA not universally enforced");
        
        if (!analysis.BlockLegacyAuthEnabled)
            analysis.CoverageGaps.Add("⚠️  Legacy authentication not blocked");

        if (enabledPolicies.Count < 3)
            analysis.CoverageGaps.Add("⚠️  Fewer than 3 CA policies enabled (consider adding more layers)");

        return analysis;
    }

    #endregion
}

#region Supporting Classes

/// <summary>
/// Analysis of Conditional Access coverage and gaps
/// </summary>
public class CACoverageAnalysis
{
    public bool MFAEnforced { get; set; }
    public int MFAPolicyCount { get; set; }
    public bool BlockLegacyAuthEnabled { get; set; }
    public bool ExternalAccessControlled { get; set; }
    public List<string> CoverageGaps { get; set; } = new();
}

#endregion
