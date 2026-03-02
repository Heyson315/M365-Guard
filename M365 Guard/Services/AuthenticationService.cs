using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System.Security.Cryptography.X509Certificates;

namespace M365_Guard.Services;

/// <summary>
/// Handles authentication with Microsoft Graph API using Certificate-based authentication
/// Supports Azure Key Vault for certificate storage (no client secrets needed)
/// Uses Managed Identity in Azure, Azure CLI credentials for local development
/// </summary>
public class AuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly string _tenantId;
    private readonly string _clientId;
    private readonly string? _keyVaultUri;
    private readonly bool _isMultiTenant;

    public AuthenticationService(IConfiguration configuration)
    {
        _configuration = configuration;
        _tenantId = configuration["AzureAd:TenantId"] ?? "common";
        _clientId = configuration["AzureAd:ClientId"] ?? string.Empty;
        _keyVaultUri = configuration["KeyVault:VaultUri"];
        _isMultiTenant = configuration["ISVConfiguration:IsMultiTenant"]?.ToLower() == "true";
    }

    /// <summary>
    /// Get an authenticated GraphServiceClient using certificate authentication
    /// Priority: Certificate from Key Vault > Local Certificate > Managed Identity
    /// </summary>
    public GraphServiceClient GetGraphClient()
    {
        TokenCredential credential;

        // Try certificate authentication first
        var certificate = GetCertificate();
        if (certificate != null)
        {
            Console.WriteLine("🔐 Using certificate authentication");
            credential = new ClientCertificateCredential(_tenantId, _clientId, certificate);
        }
        else
        {
            // Fallback to Azure CLI credentials for local development
            Console.WriteLine("🔑 Using Azure CLI credentials (development mode)");
            credential = new AzureCliCredential();
        }

        return new GraphServiceClient(credential);
    }

    /// <summary>
    /// Get GraphServiceClient for a specific tenant (multi-tenant scenarios)
    /// </summary>
    public GraphServiceClient GetGraphClientForTenant(string tenantId)
    {
        TokenCredential credential;

        var certificate = GetCertificate();
        if (certificate != null)
        {
            credential = new ClientCertificateCredential(tenantId, _clientId, certificate);
        }
        else
        {
            credential = new AzureCliCredential(new AzureCliCredentialOptions { TenantId = tenantId });
        }

        return new GraphServiceClient(credential);
    }

    /// <summary>
    /// Get certificate from Key Vault or local certificate store
    /// </summary>
    private X509Certificate2? GetCertificate()
    {
        try
        {
            // Try Key Vault first
            if (!string.IsNullOrEmpty(_keyVaultUri))
            {
                var certName = _configuration["KeyVault:CertificateName"] ?? "M365Guard-Cert";
                return GetCertificateFromKeyVault(_keyVaultUri, certName);
            }

            // Try local certificate store
            var certThumbprint = _configuration["AzureAd:CertificateThumbprint"];
            if (!string.IsNullOrEmpty(certThumbprint))
            {
                return GetCertificateFromStore(certThumbprint);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Certificate not found: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Retrieve certificate from Azure Key Vault
    /// </summary>
    private X509Certificate2? GetCertificateFromKeyVault(string keyVaultUri, string certificateName)
    {
        try
        {
            // Use Managed Identity or Azure CLI credentials
            var credential = new DefaultAzureCredential();
            var client = new SecretClient(new Uri(keyVaultUri), credential);

            // Get certificate from Key Vault (stored as secret in PFX format)
            var secret = client.GetSecret($"{certificateName}-pfx").Value;
            
            if (secret?.Value != null)
            {
                var certBytes = Convert.FromBase64String(secret.Value);
                return new X509Certificate2(certBytes);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Could not retrieve certificate from Key Vault: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Retrieve certificate from local certificate store
    /// </summary>
    private X509Certificate2? GetCertificateFromStore(string thumbprint)
    {
        using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);

        var certs = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false);
        
        return certs.Count > 0 ? certs[0] : null;
    }

    public bool IsMultiTenant() => _isMultiTenant;
    public string GetTenantId() => _tenantId;
}
