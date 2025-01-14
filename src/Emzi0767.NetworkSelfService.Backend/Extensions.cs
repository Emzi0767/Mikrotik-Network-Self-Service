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
using System.Linq;
using System.Security.Claims;
using System.Text;
using Emzi0767.NetworkSelfService.gRPC;
using Grpc.Core;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Configuration;

namespace Emzi0767.NetworkSelfService.Backend;

/// <summary>
/// Provides various extensions throughout the project.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Gets preconfigured UTF-8 encoding.
    /// </summary>
    public static Encoding UTF8 { get; } = new UTF8Encoding(false);

    public static IConfiguration SetupConfiguration(this IConfiguration configuration)
    {
        // 1. Load default configuration (appsettings.*.json)
        // 2. Load environment variables (overwrite existing if applicable)
        // 3. Load commandline (ditto)
        var cfgBoot = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .AddEnvironmentVariables("NSS2:")
            .AddCommandLine(Environment.GetCommandLineArgs())
            .Build();

        // 1. Load JSON configuration
        // 3. Load XML configuration (see above)
        var cfgFiles = new ConfigurationBuilder()
            .AddJsonFile(cfgBoot.GetSection("Configuration:Json")?.Value ?? "config.json", optional: true)
            .AddYamlFile(cfgBoot.GetSection("Configuration:Yaml")?.Value ?? "config.yml", optional: true)
            .AddXmlFile(cfgBoot.GetSection("Configuration:Xml")?.Value ?? "config.xml", optional: true)
            .Build();

        // 1. Load file config values
        // 2. Load bootstrap config values (see bootstrap)
        return new ConfigurationBuilder()
            .AddConfiguration(cfgFiles)
            .AddConfiguration(cfgBoot)
            .Build();
    }

    /// <summary>
    /// Retrieves a specific claim for a given claims principal.
    /// </summary>
    /// <param name="user">Claims principal to find a claim for.</param>
    /// <param name="claimType">Type of claim to retrieve.</param>
    /// <returns>Claim or null.</returns>
    public static Claim GetClaim(this ClaimsPrincipal user, string claimType)
        => user.Claims.FirstOrDefault(x => x.Type == claimType);

    /// <summary>
    /// Gets the claim principal's username.
    /// </summary>
    /// <param name="user">Claim principal to get ID for.</param>
    /// <returns>Name or null.</returns>
    public static string GetName(this ClaimsPrincipal user)
    {
        var claim = user.GetClaim(TokenClaimTypes.UserId);
        return claim?.Value;
    }

    /// <summary>
    /// Gets the claim principal's Session ID.
    /// </summary>
    /// <param name="user">Claim principal to get ID for.</param>
    /// <returns>ID or null.</returns>
    public static Guid? GetSessionId(this ClaimsPrincipal user)
    {
        var claim = user.GetClaim(TokenClaimTypes.SessionId);
        if (claim is null)
            return null;

        return Guid.Parse(claim.Value);
    }

    /// <summary>
    /// Gets whether the principal was authenticated using an authentication token.
    /// </summary>
    /// <param name="user">Claims principal to get token type for.</param>
    /// <returns>Whether the token used was of specified kind.</returns>
    public static bool IsAuthenticationToken(this ClaimsPrincipal user)
    {
        var claim = user.GetClaim(TokenClaimTypes.TokenKind);
        return claim?.Value == TokenConstants.TokenKindAuthentication;
    }

    /// <summary>
    /// Gets whether the principal was authenticated using a refresh token.
    /// </summary>
    /// <param name="user">Claims principal to get token type for.</param>
    /// <returns>Whether the token used was of specified kind.</returns>
    public static bool IsRefreshToken(this ClaimsPrincipal user)
    {
        var claim = user.GetClaim(TokenClaimTypes.TokenKind);
        return claim?.Value == TokenConstants.TokenKindRefresh;
    }

    /// <summary>
    /// Fills in a failure result.
    /// </summary>
    /// <param name="result">Result to fill in.</param>
    /// <param name="code">Error code to fill in.</param>
    /// <returns>Filled in result (passthrough).</returns>
    public static Result Failure(this Result result, ErrorCode code)
    {
        result.IsSuccess = false;
        result.Error = new() { Code = code, };

        return result;
    }

    /// <summary>
    /// Adds XSRF data to the response.
    /// </summary>
    /// <param name="result">Result to augment.</param>
    /// <param name="antiforgery">XSRF token generator.</param>
    /// <param name="context">Context in which to add a token.</param>
    /// <returns>Augmented response.</returns>
    public static Result SetXsrf(this Result result, IAntiforgery antiforgery, ServerCallContext context)
    {
        var tokens = antiforgery.GetTokens(context.GetHttpContext());
        var ctx = context.GetHttpContext();
        if (tokens.CookieToken is not null)
            ctx.Response.Cookies.Append("XSRF-TOKEN", tokens.CookieToken, new() { HttpOnly = true, });

        result.XsrfToken = tokens.RequestToken;
        return result;
    }
}
