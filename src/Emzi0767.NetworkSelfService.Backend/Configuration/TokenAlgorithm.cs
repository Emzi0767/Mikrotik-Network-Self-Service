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

namespace Emzi0767.NetworkSelfService.Backend.Configuration;

/// <summary>
/// Determines what kind of algorithm is used by the token system.
/// </summary>
public enum TokenAlgorithm : int
{
    /// <summary>
    /// Specifies unknown algorithm. This is usually indicative of an error.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Specifies HMAC-SHA256 algorithm. This is a symmetrical algorithm.
    /// </summary>
    HS256,

    /// <summary>
    /// Specifies HMAC-SHA384 algorithm. This is a symmetrical algorithm.
    /// </summary>
    HS384,

    /// <summary>
    /// Specifies HMAC-SHA512 algorithm. This is a symmetrical algorithm.
    /// </summary>
    HS512,

    /// <summary>
    /// Specifies RSA-SHA256 algorithm. This is an asymmetrical algorithm.
    /// </summary>
    RS256,

    /// <summary>
    /// Specifies RSA-SHA384 algorithm. This is an asymmetrical algorithm.
    /// </summary>
    RS384,

    /// <summary>
    /// Specifies RSA-SHA512 algorithm. This is an asymmetrical algorithm.
    /// </summary>
    RS512,

    /// <summary>
    /// Specifies RSA-PSS-SHA256 algorithm. This is an asymmetrical algorithm.
    /// </summary>
    PS256,

    /// <summary>
    /// Specifies RSA-PSS-SHA384 algorithm. This is an asymmetrical algorithm.
    /// </summary>
    PS384,

    /// <summary>
    /// Specifies RSA-PSS-SHA512 algorithm. This is an asymmetrical algorithm.
    /// </summary>
    PS512,

    /// <summary>
    /// Specifies ECDSA-SHA256 algorithm. This is an asymmetrical algorithm.
    /// </summary>
    ES256,

    /// <summary>
    /// Specifies ECDSA-SHA384 algorithm. This is an asymmetrical algorithm.
    /// </summary>
    ES384,

    /// <summary>
    /// Specifies ECDSA-SHA512 algorithm. This is an asymmetrical algorithm.
    /// </summary>
    ES512,
}
