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

namespace Emzi0767.NetworkSelfService.Backend;

/// <summary>
/// Provides constants for various security token claim types.
/// </summary>
public static class TokenClaimTypes
{
    /// <summary>
    /// Gets the User ID claim.
    /// </summary>
    public const string UserId = "uid";

    /// <summary>
    /// Gets the User Name claim.
    /// </summary>
    public const string UserName = "nam";

    /// <summary>
    /// Gets the Remember Me claim.
    /// </summary>
    public const string IsRemembered = "rmb";

    /// <summary>
    /// Gets the Session ID claim.
    /// </summary>
    public const string SessionId = "sid";

    /// <summary>
    /// Gets the Token Kind claim.
    /// </summary>
    public const string TokenKind = "tki";
}
