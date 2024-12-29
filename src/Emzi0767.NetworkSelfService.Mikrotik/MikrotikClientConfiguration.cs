namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Contains options for <see cref="MikrotikClient"/>.
/// </summary>
public sealed class MikrotikClientConfiguration
{
    /// <summary>
    /// Gets or sets the username to authenticate as.
    /// </summary>
    public string Username { get; set; }
    
    /// <summary>
    /// Gets or sets the password to authenticate with.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the options for TLS connections.
    /// </summary>
    public MikrotikTlsOptions TlsOptions { get; set; }
}