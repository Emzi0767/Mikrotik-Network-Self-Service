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

using System;
using System.Runtime.Serialization;
using Emzi0767.NetworkSelfService.Mikrotik.Attributes;
using Emzi0767.NetworkSelfService.Mikrotik.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

/// <summary>
/// Represents a Mikrotik CAPsMAN ACL entry.
/// </summary>
[GenerateMikrotikEntityMetadata, MikrotikEntity("caps-man", "access-list")]
public sealed class MikrotikCapsmanAcl : IMikrotikEntity
{
    /// <inheritdoc />
    public MikrotikClient Client { get; }

    /// <summary>
    /// Gets the unique identifier of this entity.
    /// </summary>
    [DataMember(Name = ".id")]
    public string Id { get; internal set; }

    /// <summary>
    /// Gets the MAC address to match.
    /// </summary>
    [DataMember(Name = "mac-address")]
    public MacAddress? Address { get; internal set; }

    /// <summary>
    /// Gets the mask of the address to match.
    /// </summary>
    [DataMember(Name = "mac-address-mask")]
    public MacAddress? Mask { get; internal set; }

    /// <summary>
    /// Gets the name of the interface list the CAP interface is attached to.
    /// </summary>
    [DataMember(Name = "interface")]
    public string InterfaceList { get; internal set; }

    /// <summary>
    /// Gets the times during which the clients are allowed to connect.
    /// </summary>
    [DataMember(Name = "time")]
    public MikrotikTimeRange? Time { get; internal set; }

    /// <summary>
    /// Gets the signal range in which the clients are allowed to connect.
    /// </summary>
    [DataMember(Name = "signal-range")]
    public MikrotikRange? SignalRange { get; internal set; }

    /// <summary>
    /// Gets the maximum amount of time the clients are allowed to remain out of range before they get booted.
    /// </summary>
    [DataMember(Name = "allow-signal-out-of-range")]
    public TimeSpan? AllowSignalOutOfRange { get; internal set; }

    /// <summary>
    /// Gets the regular expression to match the SSID.
    /// </summary>
    [DataMember(Name = "ssid-regexp")]
    public string SsidRegex { get; internal set; }

    /// <summary>
    /// Gets the action to execute for this ACL.
    /// </summary>
    [DataMember(Name = "action")]
    public MikrotikCapsmanAclAction Action { get; internal set; }

    /// <summary>
    /// Gets the maximum receive bandwidth allowed for clients matched by this ACL.
    /// </summary>
    [DataMember(Name = "ap-tx-limit")]
    public int? TransmitBandwidthLimit { get; internal set; }

    /// <summary>
    /// Gets the maximum transmit bandwidth allowed for clients matched by this ACL.
    /// </summary>
    [DataMember(Name = "client-tx-limit")]
    public int? ReceiveBandwidthLimit { get; internal set; }

    /// <summary>
    /// Gets whether to enable client-to-client forwarding for clients matched by this ACL.
    /// </summary>
    [DataMember(Name = "client-to-client-forwarding")]
    public bool? EnableClientToClientForwarding { get; internal set; }

    /// <summary>
    /// Gets the private password to use for clients matched by this ACL.
    /// </summary>
    [DataMember(Name = "private-passphrase")]
    public string PrivatePassword { get; internal set; }

    /// <summary>
    /// Gets whether to enable RADIUS traffic accounting for this ACL.
    /// </summary>
    [DataMember(Name = "radius-accounting")]
    public bool? EnableRadiusAccounting { get; internal set; }

    /// <summary>
    /// Gets the VLAN tagging mode to use for clients matched by this ACL.
    /// </summary>
    [DataMember(Name = "vlan-mode")]
    public MikrotikCapsmanVlanMode? VlanMode { get; internal set; }

    /// <summary>
    /// Gets the VLAN ID to assign to the clients matched by this ACL.
    /// </summary>
    [DataMember(Name = "vlan-id")]
    public int? VlanId { get; internal set; }

    /// <summary>
    /// Gets the comment for this entry.
    /// </summary>
    [DataMember(Name = "comment")]
    public string Comment { get; internal set; }

    /// <summary>
    /// Gets whether the entry is disabled.
    /// </summary>
    [DataMember(Name = "disabled")]
    public bool Disabled { get; internal set; }

    internal MikrotikCapsmanAcl(MikrotikClient client)
    {
        this.Client = client;
    }
}
