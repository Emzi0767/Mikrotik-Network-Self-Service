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
/// Represents a CAPsMAN datapath.
/// </summary>
[GenerateMikrotikEntityMetadata, MikrotikEntity("caps-man", "datapath")]
public sealed class MikrotikCapsmanDatapath : IMikrotikEntity
{
    /// <inheritdoc />
    public MikrotikClient Client { get; }

    /// <summary>
    /// Gets the unique identifier of this entity.
    /// </summary>
    [DataMember(Name = ".id"), MikrotikReadonlyProperty]
    public string Id { get; internal set; }

    /// <summary>
    /// Gets the name of the datapath.
    /// </summary>
    [DataMember(Name = "name")]
    public string Name { get; internal set; }

    /// <summary>
    /// Gets whether to enable client-to-client forwarding.
    /// </summary>
    [DataMember(Name = "client-to-client-forwarding")]
    public bool EnableClientToClientForwarding { get; internal set; }

    /// <summary>
    /// Gets the name of the bridge this datapath uses.
    /// </summary>
    [DataMember(Name = "bridge")]
    public string Bridge { get; internal set; }

    /// <summary>
    /// Gets whether to enable forwarding at AP. If disabled, traffic is forwarded at the CAPsMAN controller.
    /// </summary>
    [DataMember(Name = "local-forwarding")]
    public bool EnableLocalForwarding { get; internal set; }

    /// <summary>
    /// Gets the VLAN tagging mode for this datapath.
    /// </summary>
    [DataMember(Name = "vlan-mode")]
    public MikrotikCapsmanVlanMode VlanTaggingMode { get; internal set; }

    /// <summary>
    /// Gets the VLAN ID for the traffic.
    /// </summary>
    [DataMember(Name = "vlan-id")]
    public int? VlanId { get; internal set; }

    /// <summary>
    /// Gets the name of the interface list to attach the CAP interfaces to.
    /// </summary>
    [DataMember(Name = "interface-list")]
    public string InterfaceList { get; internal set; }

    internal MikrotikCapsmanDatapath(MikrotikClient client)
    {
        this.Client = client;
    }
}
