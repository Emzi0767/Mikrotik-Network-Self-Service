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

using System.Runtime.Serialization;

namespace Emzi0767.NetworkSelfService.Mikrotik.Types;

/// <summary>
/// Specifies TLS mode for certificate authentication.
/// </summary>
[GenerateEnumMetadata]
public enum MikrotikTlsMode
{
    /// <summary>
    /// Specifies that no certificates are to be used.
    /// </summary>
    [DataMember(Name = "no-certificates")]
    NoCertificates,

    /// <summary>
    /// Specifies that certificates should be verified against the CA chain.
    /// </summary>
    [DataMember(Name = "verify-certificate")]
    VerifyCertificates,

    /// <summary>
    /// Specifies that certificates should be verified against the CA chain and CRLs.
    /// </summary>
    [DataMember(Name = "verify-certificate-with-crl")]
    VerifyCertificatesWithCrl,

    /// <summary>
    /// Specifies that certificates should be used, but not verified.
    /// </summary>
    [DataMember(Name = "dont-verify-certificate")]
    DontVerifyCertificates,
}
