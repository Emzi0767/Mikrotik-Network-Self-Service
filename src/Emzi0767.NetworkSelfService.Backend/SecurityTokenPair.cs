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

namespace Emzi0767.NetworkSelfService.Backend;

/// <summary>
/// Represents a pair of security tokens - an authentication token and a refresh token.
/// </summary>
public struct SecurityTokenPair
{
    /// <summary>
    /// Gets whether the pair was issued successfully.
    /// </summary>
    public bool IsSuccessful { get; }

    /// <summary>
    /// Gets the authentication token.
    /// </summary>
    public string Authentication { get; }

    /// <summary>
    /// Gets the refresh token.
    /// </summary>
    public string Refresh { get; }

    /// <summary>
    /// Gets the instant by which the authentication token requires refreshing.
    /// </summary>
    public DateTimeOffset RefreshBy { get; }

    /// <summary>
    /// Creates a new security token pair.
    /// </summary>
    /// <param name="auth">Authentication token.</param>
    /// <param name="refresh">Refresh token.</param>
    /// <param name="refreshBy">The instant by which the authentication token requires refreshing.</param>
    public SecurityTokenPair(string auth, string refresh, DateTimeOffset refreshBy)
    {
        this.IsSuccessful = true;
        this.Authentication = auth;
        this.Refresh = refresh;
        this.RefreshBy = refreshBy;
    }
}
