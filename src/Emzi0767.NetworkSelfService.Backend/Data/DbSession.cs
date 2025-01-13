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

using System;

namespace Emzi0767.NetworkSelfService.Backend.Data;

/// <summary>
/// Represents a logged-in user session.
/// </summary>
public class DbSession
{
    /// <summary>
    /// Gets or sets the id of the session.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the username the session is for.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets whether the user checked the "remember me" box.
    /// </summary>
    public bool IsRemembered { get; set; }

    /// <summary>
    /// Gets or sets the instant the session expires at.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the instant the session was created at.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets the user the session is for.
    /// </summary>
    public DbUser User { get; set; }
}
