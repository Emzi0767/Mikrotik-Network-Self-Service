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

using System.Net;
using System.Runtime.Serialization;
using Emzi0767.NetworkSelfService.Mikrotik.Attributes;
using Emzi0767.NetworkSelfService.Mikrotik.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

/// <summary>
/// Represents an IPv4 address associated to an interface.
/// </summary>
[GenerateMikrotikEntityMetadata, MikrotikEntity("ip", "address")]
public sealed class MikrotikIPv4Address : IMikrotikEntity
{
    /// <inheritdoc />
    public MikrotikClient Client { get; }

    /// <summary>
    /// Gets the unique identifier of this entity.
    /// </summary>
    [DataMember(Name = ".id")]
    public string Id { get; internal set; }

    /// <summary>
    /// Gets the address assigned via this association.
    /// </summary>
    [DataMember(Name = "address")]
    public IPAddressWithSubnet Address { get; internal set; }

    /// <summary>
    /// Gets the base network of the address.
    /// </summary>
    [DataMember(Name = "network")]
    public IPAddress Network { get; internal set; }

    /// <summary>
    /// Gets the interface the address is assigned to.
    /// </summary>
    [DataMember(Name = "interface")]
    public string Interface { get; internal set; }

    /// <summary>
    /// Gets the actual interface the address is on.
    /// </summary>
    [DataMember(Name = "actual-interface")]
    public string ActualInterface { get; internal set; }

    internal MikrotikIPv4Address(MikrotikClient client)
    {
        this.Client = client;
    }
}
