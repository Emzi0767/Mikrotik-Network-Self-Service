// This file is part of Network Self-Service Project.
// Copyright © 2024-2025 Mateusz Brawański <Emzi0767>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

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
