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
/// Represents overall configuration of the application.
/// </summary>
public sealed class ApplicationConfiguration
{
    /// <summary>
    /// Gets or sets whether to log sensitive information.
    /// </summary>
    public bool LogSensitive { get; set; } = false;

    /// <summary>
    /// Gets or sets the Bcrypt hash factor for saving passwords.
    /// </summary>
    [Required]
    public int BcryptFactor { get; set; }

    /// <summary>
    /// Gets or sets the configuration for HTTP components of the application.
    /// </summary>
    [Required, NotNull]
    public HttpConfiguration Http { get; set; }

    /// <summary>
    /// Gets or sets the configuration for authentication components of the application.
    /// </summary>
    [Required, NotNull]
    public AuthenticationConfiguration Authentication { get; set; }

    /// <summary>
    /// Gets or sets the configuration for PostgreSQL client.
    /// </summary>
    [Required, NotNull]
    public PostgresConfiguration Postgres { get; set; }
}
