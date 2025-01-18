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
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Emzi0767.NetworkSelfService.Mikrotik.Attributes;
using Emzi0767.NetworkSelfService.Mikrotik.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

/// <summary>
/// Represents a DHCP lease on a Mikrotik DHCP server.
/// </summary>
[GenerateMikrotikEntityMetadata, MikrotikEntity("ip", "dhcp-server", "lease")]
public class MikrotikDhcpLease : IMikrotikEntity, IMikrotikDeletableEntity
{
    /// <inheritdoc />
    public MikrotikClient Client { get; }

    /// <summary>
    /// Gets the unique identifier of this entity.
    /// </summary>
    [DataMember(Name = ".id"), MikrotikReadonlyProperty]
    public string Id { get; internal set; }

    /// <summary>
    /// Gets the IP address associated with this lease.
    /// </summary>
    [DataMember(Name = "address")]
    public IPAddress Address { get; internal set; }

    /// <summary>
    /// Gets the address lists the address gets added to if this entry gets bound.
    /// </summary>
    [DataMember(Name = "address-lists")]
    public IEnumerable<string> AddressLists { get; internal set; }

    /// <summary>
    /// Gets whether this entry creates a simple queue entry for IPv4 and IPv6, using MAC address and DUID for
    /// identification.
    /// </summary>
    [DataMember(Name = "allow-dual-stack-queue")]
    public bool AllowsDualStackQueue { get; internal set; }

    /// <summary>
    /// Gets whether to always send DHCP replies as broadcast. If disabled, first 3 are sent as unicast packets.
    /// </summary>
    [DataMember(Name = "always-broadcast")]
    public bool AlwaysSendAsBroadcast { get; internal set; }

    /// <summary>
    /// Gets whether to block access for the client.
    /// </summary>
    [DataMember(Name = "block-access")]
    public bool AccessBlocked { get; internal set; }

    /// <summary>
    /// Gets the identifier of the client to match (if applicable).
    /// </summary>
    [DataMember(Name = "client-id")]
    public string ClientId { get; internal set; }

    /// <summary>
    /// Gets the name of configured DHCP options to send in the responses.
    /// </summary>
    [DataMember(Name = "dhcp-option")]
    public IEnumerable<string> DhcpOptions { get; internal set; }

    /// <summary>
    /// Gets the name of configured DHCP option sets to send in the responses.
    /// </summary>
    [DataMember(Name = "dhcp-option-set")]
    public IEnumerable<string> DhcpOptionSets { get; internal set; }

    /// <summary>
    /// Gets where to place simple queue entries for leases with ratelimits enabled.
    /// </summary>
    [DataMember(Name = "insert-queue-before")]
    public string InsertQueueBefore { get; internal set; }

    /// <summary>
    /// Gets the amount of time the address is assigned to a client for. Setting this to <see cref="TimeSpan.Zero"/>
    /// will make this lease never expire.
    /// </summary>
    [DataMember(Name = "lease-time")]
    public TimeSpan LeaseTime { get; internal set; }

    /// <summary>
    /// Gets the MAC address of the client to match for this lease.
    /// </summary>
    [DataMember(Name = "mac-address")]
    public MacAddress MacAddress { get; internal set; }

    /// <summary>
    /// Gets the name of the parent queue the dynamic queue for this entry will be created under.
    /// </summary>
    [DataMember(Name = "parent-queue")]
    public string ParentQueue { get; internal set; }

    /// <summary>
    /// Gets the type of dynamic queue to assign to this lease.
    /// </summary>
    [DataMember(Name = "queue-type")]
    public MikrotikDhcpQueueType QueueType { get; internal set; }

    /// <summary>
    /// Gets the dynamic rate limit to apply to the client associated with this lease.
    /// </summary>
    [DataMember(Name = "rate-limit")]
    public MikrotikDhcpRateLimit RateLimit { get; internal set; }

    /// <summary>
    /// Gets the routes configured for this lease.
    /// </summary>
    [DataMember(Name = "routes")]
    public IEnumerable<MikrotikDhcpRoute> Routes { get; internal set; }

    /// <summary>
    /// Gets the name of the server which serves this client.
    /// </summary>
    [DataMember(Name = "server")]
    public string Server { get; internal set; }

    /// <summary>
    /// Gets whether the server should use client's source MAC address instead of DHCP CHADDR option to assign the
    /// lease.
    /// </summary>
    [DataMember(Name = "use-src-mac")]
    public bool UseSourceMac { get; internal set; }

    /// <summary>
    /// Gets the hostname associated with the lease.
    /// </summary>
    [DataMember(Name = "host-name"), MikrotikReadonlyProperty]
    public string Hostname { get; internal set; }

    /// <summary>
    /// Gets whether the lease is dynamic.
    /// </summary>
    [DataMember(Name = "dynamic"), MikrotikReadonlyProperty]
    public bool IsDynamic { get; internal set; }

    internal MikrotikDhcpLease(MikrotikClient client)
    {
        this.Client = client;
    }

    /// <inheritdoc />
    public Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}
