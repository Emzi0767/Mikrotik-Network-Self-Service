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
/// Represents a CAP interface with its actual/final configuration.
/// </summary>
[GenerateMikrotikEntityMetadata, MikrotikEntity("caps-man", "actual-interface-configuration")]
public sealed class MikrotikCapsmanInterfaceConfiguration : IMikrotikEntity
{
    /// <inheritdoc />
    public MikrotikClient Client { get; }

    /// <summary>
    /// Gets the unique identifier of this entity.
    /// </summary>
    [DataMember(Name = ".id")]
    public string Id { get; internal set; }

    /// <summary>
    /// Gets the name of this interface.
    /// </summary>
    [DataMember(Name = "name")]
    public string Name { get; internal set; }

    /// <summary>
    /// Gets the name of the master interface.
    /// </summary>
    [DataMember(Name = "master-interface")]
    public string MasterInterfaceName { get; internal set; }

    /// <summary>
    /// Gets the MAC address of this interface.
    /// </summary>
    [DataMember(Name = "mac-address")]
    public MacAddress MacAddress { get; internal set; }

    /// <summary>
    /// Gets the MAC address of the radio. This is non-zero for slave interfaces.
    /// </summary>
    [DataMember(Name = "radio-mac")]
    public MacAddress RadioMacAddress { get; internal set; }

    /// <summary>
    /// Gets the layer 2 MTU of this interface.
    /// </summary>
    [DataMember(Name = "l2mtu")]
    public int Layer2Mtu { get; internal set; }

    /// <summary>
    /// Gets the wireless band of this interface.
    /// </summary>
    [DataMember(Name = "channel.band")]
    public MikrotikWirelessBand WirelessBand { get; internal set; }

    /// <summary>
    /// Gets the name of the interface list this interface is attached to.
    /// </summary>
    [DataMember(Name = "datapath.interface-list")]
    public string InterfaceListName { get; internal set; }

    /// <summary>
    /// Gets the SSID of the broadcasted wireless network.
    /// </summary>
    [DataMember(Name = "configuration.ssid")]
    public string Ssid { get; internal set; }

    internal MikrotikCapsmanInterfaceConfiguration(MikrotikClient client)
    {
        this.Client = client;
    }
}
