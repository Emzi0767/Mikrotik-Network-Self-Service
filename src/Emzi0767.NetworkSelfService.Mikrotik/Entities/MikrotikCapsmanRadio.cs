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
using Emzi0767.NetworkSelfService.Mikrotik.Attributes;
using Emzi0767.NetworkSelfService.Mikrotik.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

/// <summary>
/// Represents a CAPsMAN radio.
/// </summary>
[GenerateMikrotikEntityMetadata, MikrotikEntity("caps-man", "radio")]
public sealed class MikrotikCapsmanRadio : IMikrotikEntity
{
    /// <inheritdoc />
    public MikrotikClient Client { get; }

    /// <summary>
    /// Gets the unique identifier of this entity.
    /// </summary>
    [DataMember(Name = ".id"), MikrotikReadonlyProperty]
    public string Id { get; internal set; }

    /// <summary>
    /// Gets whether the radio is provisioned.
    /// </summary>
    [DataMember(Name = "provisioned"), MikrotikReadonlyProperty]
    public bool IsProvisioned { get; internal set; }

    /// <summary>
    /// Gets the MAC address of the radio.
    /// </summary>
    [DataMember(Name = "radio-mac"), MikrotikReadonlyProperty]
    public MacAddress MacAddress { get; internal set; }

    /// <summary>
    /// Gets the authenticated name of the CAP.
    /// </summary>
    [DataMember(Name = "remote-cap-name"), MikrotikReadonlyProperty]
    public string Name { get; internal set; }

    /// <summary>
    /// Gets the identity of the CAP.
    /// </summary>
    [DataMember(Name = "remote-cap-identity"), MikrotikReadonlyProperty]
    public string Identity { get; internal set; }

    /// <summary>
    /// Gets the master CAP interface corresponding to this radio.
    /// </summary>
    [DataMember(Name = "interface"), MikrotikReadonlyProperty]
    public string InterfaceName { get; internal set; }

    internal MikrotikCapsmanRadio(MikrotikClient client)
    {
        this.Client = client;
    }
}
