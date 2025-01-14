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

using Microsoft.AspNetCore.Authorization;

namespace Emzi0767.NetworkSelfService.Backend;

/// <summary>
/// Provides various token access policies.
/// </summary>
public static class TokenPolicies
{
    /// <summary>
    /// Gets the name of any user policy.
    /// </summary>
    public const string AnyUserPolicy = nameof(AnyUser);

    /// <summary>
    /// Gets the name of refresh only policy.
    /// </summary>
    public const string RefreshOnlyPolicy = nameof(RefreshOnly);

    /// <summary>
    /// Indicates that only an authentication token for any user can be used to access the resource.
    /// </summary>
    /// <param name="policyBuilder">Policy builder.</param>
    public static void AnyUser(AuthorizationPolicyBuilder policyBuilder)
        => policyBuilder
            .RequireClaim(TokenClaimTypes.TokenKind, TokenConstants.TokenKindAuthentication)
            .RequireClaim(TokenClaimTypes.UserId)
            .RequireClaim(TokenClaimTypes.SessionId);

    /// <summary>
    /// Indicates that only a refresh token can be used to access the resource.
    /// </summary>
    /// <param name="policyBuilder"></param>
    public static void RefreshOnly(AuthorizationPolicyBuilder policyBuilder)
        => policyBuilder
            .RequireClaim(TokenClaimTypes.TokenKind, TokenConstants.TokenKindRefresh)
            .RequireClaim(TokenClaimTypes.SessionId);
}
