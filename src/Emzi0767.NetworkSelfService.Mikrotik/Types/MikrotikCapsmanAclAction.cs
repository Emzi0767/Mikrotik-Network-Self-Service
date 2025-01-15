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
/// Specifies the type of action to take for a CAPsMAN ACL.
/// </summary>
[GenerateEnumMetadata]
public enum MikrotikCapsmanAclAction
{
    /// <summary>
    /// Specifies that a client should be accepted.
    /// </summary>
    [DataMember(Name = "accept")]
    Accept,

    /// <summary>
    /// Specifies that a client should be rejected.
    /// </summary>
    [DataMember(Name = "reject")]
    Reject,

    /// <summary>
    /// Specifies that an external RADIUS server should be queried.
    /// </summary>
    [DataMember(Name = "query-radius")]
    QueryRadius,
}
