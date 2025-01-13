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
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Emzi0767.NetworkSelfService.Backend.Configuration;
using Emzi0767.NetworkSelfService.Backend.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Emzi0767.NetworkSelfService.Backend.Services;

/// <summary>
/// Generates security tokens.
/// </summary>
public sealed class TokenGenerator
{
    private readonly AuthenticationConfiguration _configuration;
    private readonly SecurityKey _securityKey;
    private readonly string _algorithm;
    private readonly JwtSecurityTokenHandler _jwtHandler;

    /// <summary>
    /// Creates a new token generator.
    /// </summary>
    /// <param name="opts">Options to configure the generator with.</param>
    /// <param name="keyProvider">Key provider for generating keys to sign tokens with.</param>
    public TokenGenerator(
        IOptions<AuthenticationConfiguration> opts,
        TokenKeyProvider keyProvider)
    {
        this._configuration = opts.Value;
        this._securityKey = keyProvider.Key;
        this._algorithm = this._configuration.Algorithm.ToString();
        this._jwtHandler = new();
    }

    /// <summary>
    /// Generates a pair of tokens for authentication.
    /// </summary>
    /// <param name="user">User to generate the tokens for.</param>
    /// <param name="session">Session to generate tokens for.</param>
    /// <param name="issueRefresh">Whether to issue a new refresh token.</param>
    /// <returns>Generated token pair.</returns>
    public SecurityTokenPair GenerateTokenPair(DbUser user, DbSession session, bool issueRefresh = true)
    {
        var credentials = new SigningCredentials(this._securityKey, this._algorithm);
        var authenticationClaims = new List<Claim>()
        {
            new(TokenClaimTypes.UserId, user.Username),
            new(TokenClaimTypes.SessionId, session.Id.ToString()),
            new(TokenClaimTypes.TokenKind, TokenConstants.TokenKindAuthentication),
        };

        var refreshClaims = new List<Claim>()
        {
            new(TokenClaimTypes.SessionId, session.Id.ToString()),
            new(TokenClaimTypes.IsRemembered, session.IsRemembered ? "true" : "false"),
            new(TokenClaimTypes.TokenKind, TokenConstants.TokenKindRefresh),
        };

        var now = DateTime.UtcNow;
        var authenticationToken = new JwtSecurityToken(
            this._configuration.Issuer,
            this._configuration.Audience,
            authenticationClaims,
            now,
            now.AddSeconds(this._configuration.ExpirationSeconds),
            credentials);

        var refreshToken = issueRefresh
            ? new JwtSecurityToken(
                this._configuration.Issuer,
                this._configuration.Audience,
                refreshClaims,
                session.CreatedAt.DateTime,
                session.ExpiresAt.DateTime,
                credentials)
            : null;

        return new(
            this._jwtHandler.WriteToken(authenticationToken),
            issueRefresh ? this._jwtHandler.WriteToken(refreshToken) : null,
            authenticationToken.ValidTo
        );
    }

    /// <summary>
    /// Computes expiration date for a session.
    /// </summary>
    /// <param name="remember">Whether to remember the user (extend session validity).</param>
    /// <returns>Computed timestamp of expiration.</returns>
    public DateTimeOffset GetExpirationDate(bool remember)
        => DateTimeOffset.UtcNow.AddSeconds(remember
            ? this._configuration.RememberExpirationSeconds
            : this._configuration.ExpirationSeconds);
}

