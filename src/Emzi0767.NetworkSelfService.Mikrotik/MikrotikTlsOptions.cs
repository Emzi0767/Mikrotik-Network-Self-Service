using System.Net.Security;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Specifies options for TLS connections to the Mikrotik API.
/// </summary>
public readonly struct MikrotikTlsOptions
{
    /// <summary>
    /// Gets whether to use TLS at all.
    /// </summary>
    public bool UseTls { get; init; } = false;

    // /// <summary>
    // /// Gets whether to use anonymous key exchange algorithms. This can be used when the API uses TLS, but does not have
    // /// a certificate set. Note that this is not secure, and allows for man-in-the-middle attacks if enabled.
    // /// </summary>
    // public bool UseAnonymousKeyExchange { get; init; } = false;

    /// <summary>
    /// Gets whether to allow TLS versions older than v1.2. This is insecure, and therefore it is highly recommended
    /// you disable this option, and configure your Mikrotik router to only accept TLS v1.2 and above. 
    /// </summary>
    public bool AllowObsoleteTlsVersions { get; init; } = false;

    /// <summary>
    /// Gets the custom TLS validation callback. This can be used to validate certificates in the event this cannot be
    /// done using system CA store and default validation logic. Do not use this to blindly trust all certificates.
    /// </summary>
    public RemoteCertificateValidationCallback CertificateValidationCallback { get; init; }

    /// <summary>
    /// Creates new Mikrotik TLS options with default values.
    /// </summary>
    public MikrotikTlsOptions()
    { }
}
