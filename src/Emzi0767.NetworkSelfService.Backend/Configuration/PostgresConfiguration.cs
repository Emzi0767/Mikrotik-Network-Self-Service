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
/// Represents configuration for PostgreSQL client.
/// </summary>
public sealed class PostgresConfiguration
{
    /// <summary>
    /// Gets or sets the hostname of the PostgreSQL server.
    /// </summary>
    [Required, NotNull]
    public string Hostname { get; set; }

    /// <summary>
    /// Gets or sets the port to use for communication with PostgreSQL.
    /// </summary>
    [Required, Range(1, 65535)]
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the database name.
    /// </summary>
    [Required, NotNull]
    public string Database { get; set; }

    /// <summary>
    /// Gets or sets the username for connecting to the PostgreSQL server.
    /// </summary>
    [Required, NotNull]
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the password for the PostgreSQL server.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets whether to enable TLS.
    /// </summary>
    public bool EnableTls { get; set; }

    /// <summary>
    /// Gets or sets whether to enable mTLS.
    /// </summary>
    [RequiredIf(nameof(EnableTls), true)]
    public bool MutualTls { get; set; }

    /// <summary>
    /// Gets or sets the path to file containing CA certificate chain.
    /// </summary>
    [RequiredIf(nameof(MutualTls), true)]
    public string CaCertificate { get; set; }

    /// <summary>
    /// Gets or sets the path to file containing client certificate.
    /// </summary>
    [RequiredIf(nameof(MutualTls), true)]
    public string ClientCertificate { get; set; }

    /// <summary>
    /// Gets or sets the path to file containing client private key. Not required if certificate file contains the key.
    /// </summary>
    public string ClientKey { get; set; }

    /// <summary>
    /// Gets or sets the bytes comprising the password to the client private key.
    /// </summary>
    public byte[] ClientKeyPassword { get; set; }
}
