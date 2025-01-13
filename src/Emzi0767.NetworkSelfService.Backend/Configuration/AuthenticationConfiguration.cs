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

namespace Emzi0767.NetworkSelfService.Backend.Configuration;

/// <summary>
/// Represents configuration for authentication component of the application.
/// </summary>
public sealed class AuthenticationConfiguration
{
    /// <summary>
    /// <para>Gets or sets the secret key used by the token system, when symmetric algorithms are used.</para>
    /// <para>In the input data, this value has to be a base64 string containing encoded key bytes.</para>
    /// </summary>
    public byte[] SymmetricKey { get; set; }

    /// <summary>
    /// <para>Gets or sets the path to file, which contains the public or private key, used by the token system, when asymmetric algorithms are used.</para>
    /// <para>The file has to be either PEM or DER, which needs to be indicated by the file extension (.pem or .der respectively).</para>
    /// </summary>
    public string AsymmetricKey { get; set; }

    /// <summary>
    /// <para>Gets or sets the password to file, which contains the public or private key.</para>
    /// <para>The input data needs to be a base64-encoded byte[].</para>
    /// </summary>
    public byte[] AsymmetricKeyPassword { get; set; }

    /// <summary>
    /// <para>Gets or sets the algorithm used to validate tokens.</para>
    /// </summary>
    [Required]
    public TokenAlgorithm Algorithm { get; set; }

    /// <summary>
    /// <para>Gets or sets the value to set for issuer field in the token. This value will be used to validate the token as well.</para>
    /// </summary>
    [Required]
    public string Issuer { get; set; }

    /// <summary>
    /// <para>Gets or sets the value to set for audience field in the token. This value will be used to validate the token as well.</para>
    /// </summary>
    [Required]
    public string Audience { get; set; }

    /// <summary>
    /// <para>Gets or sets the number of seconds it takes the primary token to expire.</para>
    /// </summary>
    [Required]
    public int ExpirationSeconds { get; set; }

    /// <summary>
    /// <para>Gets or sets the number of seconds it takes the refresh token to expire.</para>
    /// </summary>
    [Required]
    public int RefreshExpirationSeconds { get; set; }

    /// <summary>
    /// <para>Gets or sets the number of seconds it takes the refresh token to expire, if remember me is enabled.</para>
    /// </summary>
    [Required]
    public int RememberExpirationSeconds { get; set; }

}
