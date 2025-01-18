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
using System.Diagnostics.CodeAnalysis;

namespace Emzi0767.NetworkSelfService.Backend.Configuration;

/// <summary>
/// Represents the configuration of a Mikrotik client.
/// </summary>
public sealed class MikrotikConfiguration
{
    /// <summary>
    /// Gets or sets the hostname to connect to.
    /// </summary>
    [Required, NotNull]
    public string Hostname { get; set; }

    /// <summary>
    /// Gets or sets the port to connect to.
    /// </summary>
    [Required, Range(1, 65535)]
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the username to authenticate with.
    /// </summary>
    [Required, NotNull]
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the password to authenticate with.
    /// </summary>
    [Required, NotNull]
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets whether to enable TLS.
    /// </summary>
    public bool EnableTls { get; set; }

    /// <summary>
    /// Gets or sets whether to allow obsolete TLS versions.
    /// </summary>
    public bool AllowObsoleteTlsVersions { get; set; }

    /// <summary>
    /// Gets or sets whether to enable IPv4 connectivity.
    /// </summary>
    public bool EnableIpv4 { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enable IPv6 connectivity.
    /// </summary>
    public bool EnableIpv6 { get; set; }

    /// <summary>
    /// Gets or sets the required minimum signal strength for Wi-Fi ACLs.
    /// </summary>
    public int? SignalRangeLow { get; set; }

    /// <summary>
    /// Gets or sets the required maximum signal strength for Wi-Fi ACLs.
    /// </summary>
    public int? SignalRangeHigh { get; set; }
}
