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

using System.ComponentModel.DataAnnotations;

namespace Emzi0767.NetworkSelfService.Backend.Configuration;

/// <summary>
/// Represents the configuration of a single HTTP endpoint.
/// </summary>
public sealed class HttpEndpointConfiguration
{
    /// <summary>
    /// Gets or sets the IP address or UNIX socket to bind to.
    /// </summary>
    [Required]
    public string Address { get; set; }

    /// <summary>
    /// Gets or sets the hostname of this endpoint.
    /// </summary>
    [RequiredIf(nameof(EnableTls), true)]
    public string Hostname { get; set; }

    /// <summary>
    /// Gets or sets the port to listen on.
    /// </summary>
    [Required, Range(1, 65535)]
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets whether to enable TLS on this endpoint.
    /// </summary>
    [Required]
    public bool EnableTls { get; set; }

    /// <summary>
    /// Gets or sets whether mTLS is to be enabled on the given endpoint.
    /// </summary>
    [RequiredIf(nameof(EnableTls), true)]
    public bool MutualTls { get; set; }

    /// <summary>
    /// Gets or sets the path to file containing CA certificate chain.
    /// </summary>
    [RequiredIf(nameof(EnableTls), true)]
    [RequiredIf(nameof(MutualTls), true)]
    public string CaCertificate { get; set; }

    /// <summary>
    /// Gets or sets the path to file containing server certificate.
    /// </summary>
    [RequiredIf(nameof(EnableTls), true)]
    public string ServerCertificate { get; set; }

    /// <summary>
    /// Gets or sets the path to file containing server private key. Not required if certificate file contains the key.
    /// </summary>
    public string ServerKey { get; set; }

    /// <summary>
    /// Gets or sets the password bytes for the server key.
    /// </summary>
    public byte[] ServerKeyPassword { get; set; }
}

