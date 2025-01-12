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
/// Specifies the type of VLAN tagging to apply for a CAPsMAN client.
/// </summary>
[GenerateEnumMetadata]
public enum MikrotikCapsmanVlanMode
{
    /// <summary>
    /// Do not tag wireless traffic inside the network.
    /// </summary>
    [DataMember(Name = "no-tag")]
    DontTag,

    /// <summary>
    /// Tag wireless traffic inside the network.
    /// </summary>
    [DataMember(Name = "use-tag")]
    UseTag,

    /// <summary>
    /// Tag wireless traffic inside the network using a service VLAN tag.
    /// </summary>
    [DataMember(Name = "use-service-tag")]
    UseServiceTag,
}
