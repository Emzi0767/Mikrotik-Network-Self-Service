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

using System.Collections.Generic;
using System.Runtime.Serialization;
using Emzi0767.NetworkSelfService.Mikrotik.Attributes;
using Emzi0767.NetworkSelfService.Mikrotik.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

/// <summary>
/// Represents a configured IPv4 pool.
/// </summary>
[GenerateMikrotikEntityMetadata, MikrotikEntity("ip", "pool")]
public sealed class MikrotikIPv4Pool : IMikrotikEntity
{
    /// <inheritdoc />
    public MikrotikClient Client { get; }

    /// <summary>
    /// Gets the unique identifier of this entity.
    /// </summary>
    [DataMember(Name = ".id"), MikrotikReadonlyProperty]
    public string Id { get; internal set; }

    /// <summary>
    /// Gets the comment associated with the pool.
    /// </summary>
    [DataMember(Name = "comment")]
    public string Comment { get; internal set; }

    /// <summary>
    /// Gets the name of this pool.
    /// </summary>
    [DataMember(Name = "name")]
    public string Name { get; internal set; }

    /// <summary>
    /// Gets the name of the fall-through pool.
    /// </summary>
    [DataMember(Name = "next-pool")]
    public string NextPoolName { get; internal set; }

    /// <summary>
    /// Gets the IPv4 ranges of this pool.
    /// </summary>
    [DataMember(Name = "ranges")]
    public IEnumerable<IPRange> Ranges { get;internal set; }

    internal MikrotikIPv4Pool(MikrotikClient client)
    {
        this.Client = client;
    }
}
