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
/// Specifies how a DHCP server should respond to out-of-range requests.
/// </summary>
[GenerateEnumMetadata]
public enum MikrotikDhcpAuthoritative
{
    /// <summary>
    /// Specifies that requests with less than 10 seconds will be processed as no, and more will be processed as yes.
    /// </summary>
    [DataMember(Name = "after-10sec-delay")]
    After10SecondDelay,

    /// <summary>
    /// Specifies that requests with less than 2 seconds will be processed as no, and more will be processed as yes.
    /// </summary>
    [DataMember(Name = "after-2sec-delay")]
    After2SecondDelay,

    /// <summary>
    /// Sends a negative response to requests for unavailable addresses.
    /// </summary>
    [DataMember(Name = "yes")]
    Yes,

    /// <summary>
    /// Ignores requests for unavailable addresses.
    /// </summary>
    [DataMember(Name = "no")]
    No
}
