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
using System.IO;
using System.Security.Cryptography;
using Emzi0767.NetworkSelfService.Backend.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Emzi0767.NetworkSelfService.Backend.Services;

/// <summary>
/// Handles loading and providing keys to security token subsystems.
/// </summary>
public sealed class TokenKeyProvider
{
    /// <summary>
    /// Gets the token loaded and provided by this instance.
    /// </summary>
    public SecurityKey Key { get; }

    /// <summary>
    /// Creates a new token provider instance.
    /// </summary>
    /// <param name="opts">Options to configure with.</param>
    public TokenKeyProvider(IOptions<AuthenticationConfiguration> opts)
        : this(opts.Value)
    { }

    internal TokenKeyProvider(AuthenticationConfiguration cfg)
    {
        this.Key = LoadKey(cfg);
    }

    private static SecurityKey LoadKey(AuthenticationConfiguration config)
    {
        return config.Algorithm switch
        {
            // HMAC-SHA
            TokenAlgorithm.HS256 or TokenAlgorithm.HS384 or TokenAlgorithm.HS512 => new SymmetricSecurityKey(config.SymmetricKey),

            // ECDSA
            TokenAlgorithm.ES256 or TokenAlgorithm.ES384 or TokenAlgorithm.ES512 => LoadKey<ECDsa, ECDsaSecurityKey>(config.AsymmetricKey, config.AsymmetricKeyPassword, static k => new(k)),

            // RSA
            TokenAlgorithm.RS256 or TokenAlgorithm.RS384 or TokenAlgorithm.RS512 or
            TokenAlgorithm.PS256 or TokenAlgorithm.PS384 or TokenAlgorithm.PS512 => LoadKey<RSA, RsaSecurityKey>(config.AsymmetricKey, config.AsymmetricKeyPassword, static k => new(k)),

            // Unknown
            _ => null,
        };
    }

    // AsymmetricAlgorithm.Create is haram, should use TAlgorithm.Create, but no static iface for it
#pragma warning disable SYSLIB0045
    private static AsymmetricSecurityKey LoadKey<TAlgorithm, TKey>(string filePath, byte[] filePassword, Func<TAlgorithm, TKey> keyFactory)
        where TAlgorithm : AsymmetricAlgorithm
        where TKey : AsymmetricSecurityKey
    {
        var algo = AsymmetricAlgorithm.Create(typeof(TAlgorithm).Name) as TAlgorithm;
        var keyData = File.ReadAllText(filePath);
        if (filePassword is null)
            algo.ImportFromPem(keyData);
        else
            algo.ImportFromEncryptedPem(keyData, filePassword);

        return keyFactory(algo);
    }
#pragma warning restore SYSLIB0045
}
