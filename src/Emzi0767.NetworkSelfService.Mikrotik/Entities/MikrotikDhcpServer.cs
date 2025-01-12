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
using System.Net;
using System.Runtime.Serialization;
using Emzi0767.NetworkSelfService.Mikrotik.Attributes;
using Emzi0767.NetworkSelfService.Mikrotik.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

/// <summary>
/// Represents a DHCP server.
/// </summary>
[GenerateMikrotikEntityMetadata, MikrotikEntity("ip", "dhcp-server")]
public sealed class MikrotikDhcpServer : IMikrotikEntity
{
    /// <inheritdoc />
    public MikrotikClient Client { get; }

    /// <summary>
    /// Gets whether to add ARP entry for every lease.
    /// </summary>
    [DataMember(Name = "add-arp")]
    public bool AddArpEntry { get; internal set; }

    /// <summary>
    /// Gets the name of the address pool to assign addresses from.
    /// </summary>
    [DataMember(Name = "address-pool")]
    public string AddressPoolName { get; internal set; }

    /// <summary>
    /// Gets whether this entry creates a simple queue entry for IPv4 and IPv6, using MAC address and DUID for
    /// identification.
    /// </summary>
    [DataMember(Name = "allow-dual-stack-queue")]
    public bool AllowDualStackQueue { get; internal set; }

    /// <summary>
    /// Gets whether to always send DHCP replies as broadcast. If disabled, first 3 are sent as unicast packets.
    /// </summary>
    [DataMember(Name = "always-broadcast")]
    public bool AlwaysSendAsBroadcast { get; internal set; }

    /// <summary>
    /// Gets whether the server is authoritative.
    /// </summary>
    [DataMember(Name = "authoritative")]
    public MikrotikDhcpAuthoritative Authoritative { get; internal set; }

    /// <summary>
    /// Gets the lease time for BOOTP.
    /// </summary>
    [DataMember(Name = "bootp-lease-time")]
    public string BootpLeaseTime { get; internal set; }

    /// <summary>
    /// Gets whether to support BOOTP.
    /// </summary>
    [DataMember(Name = "bootp-support")]
    public string BootpSupport { get; internal set; }

    /// <summary>
    /// Gets the limit of clients per single MAC address.
    /// </summary>
    [DataMember(Name = "client-mac-limit")]
    public string ClientMacLimit { get; internal set; }

    /// <summary>
    /// Gets whether to detect conflicts before assigning an address.
    /// </summary>
    [DataMember(Name = "conflict-detection")]
    public bool DetectConflicts { get; internal set; }

    /// <summary>
    /// Gets the number of seconds in the request below which requests are processed.
    /// </summary>
    [DataMember(Name = "delay-threshold")]
    public string DelayThreshold { get; internal set; }

    /// <summary>
    /// Gets the name of configured DHCP option set to send in the response.
    /// </summary>
    [DataMember(Name = "dhcp-option-set")]
    public string DhcpOptionSetName { get; internal set; }

    /// <summary>
    /// Gets where to place simple queue entries for leases with ratelimits enabled.
    /// </summary>
    [DataMember(Name = "insert-queue-before")]
    public string InsertQueueBefore { get; internal set; }

    /// <summary>
    /// Gets the name of the interface the server runs on.
    /// </summary>
    [DataMember(Name = "interface")]
    public string Interface { get; internal set; }

    /// <summary>
    /// Gets the script to execute every time a lease is assigned or unassigned.
    /// </summary>
    [DataMember(Name = "lease-script")]
    public string LeaseScript { get; internal set; }

    /// <summary>
    /// Gets the amount of time to assign the leases for.
    /// </summary>
    [DataMember(Name = "lease-time")]
    public TimeSpan LeaseTime { get; internal set; }

    /// <summary>
    /// Gets the name of the server.
    /// </summary>
    [DataMember(Name = "name")]
    public string Name { get; internal set; }

    /// <summary>
    /// Gets the name of the parent queue the dynamic queue for this entry will be created under.
    /// </summary>
    [DataMember(Name = "parent-queue")]
    public string ParentQueue { get; internal set; }

    /// <summary>
    /// Gets the IP address of the allowed DHCP relay.
    /// </summary>
    [DataMember(Name = "relay")]
    public IPAddress RelayAddress { get; internal set; }

    /// <summary>
    /// Gets the IP address of the server when sending out responses to clients.
    /// </summary>
    [DataMember(Name = "server-address")]
    public IPAddress ServerAddress { get; internal set; }

    /// <summary>
    /// Gets whether to send framed RADIUS routes as classess DHCP routes.
    /// </summary>
    [DataMember(Name = "use-framed-as-classess")]
    public bool ForwardFramedAsClassless { get; internal set; }

    /// <summary>
    /// Gets whether to use RADIUS.
    /// </summary>
    [DataMember(Name = "use-radius")]
    public bool UseRadius { get; internal set; }

    internal MikrotikDhcpServer(MikrotikClient client)
    {
        this.Client = client;
    }
}
