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

using System.Collections.Generic;

namespace Emzi0767.NetworkSelfService.Backend.Data;

/// <summary>
/// Represents a user of the system.
/// </summary>
public sealed class DbUser
{
    /// <summary>
    /// Gets or sets the username for the user.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the password hash for the user.
    /// </summary>
    public string PasswordHash { get; set; }

    /// <summary>
    /// Gets or sets the name of the network segment associated with the user.
    /// </summary>
    public string NetworkName { get; set; }

    /// <summary>
    /// Gets or sets the network segment associated with the user.
    /// </summary>
    public DbNetwork Network { get; set; }

    /// <summary>
    /// Gets or sets the sessions associated with this user.
    /// </summary>
    public IEnumerable<DbSession> Sessions { get; set; }
}
