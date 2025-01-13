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

using Emzi0767.NetworkSelfService.Backend.Configuration;
using Microsoft.Extensions.Options;

namespace Emzi0767.NetworkSelfService.Backend.Services;

/// <summary>
/// Provides functionality of hashing and verifying passwords.
/// </summary>
public sealed class PasswordHashProvider
{
    private readonly int _factor;

    /// <summary>
    /// Creates a new password hasher with specified config.
    /// </summary>
    /// <param name="config">Configuration for the hasher.</param>
    public PasswordHashProvider(IOptions<ApplicationConfiguration> config)
    {
        var cfg = config.Value;
        this._factor = cfg.BcryptFactor;
    }

    /// <summary>
    /// Hashes a password, returning the derived bytes. Uses given salt.
    /// </summary>
    /// <param name="input">Input password to hash.</param>
    /// <returns>Derived bytes.</returns>
    public string Hash(string input)
        => BCrypt.Net.BCrypt.HashPassword(input, this._factor);

    /// <summary>
    /// Verifies a password against a given hash.
    /// </summary>
    /// <param name="input">Input password to verify.</param>
    /// <param name="hash">Hash to verify against.</param>
    /// <returns>Whether the password matches.</returns>
    public bool Verify(string input, string hash)
        => BCrypt.Net.BCrypt.Verify(input, hash);
}
