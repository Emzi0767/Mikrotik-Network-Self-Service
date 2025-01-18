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
using System.Runtime.Serialization;
using Emzi0767.NetworkSelfService.Mikrotik.Attributes;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

/// <summary>
/// Represents a log entry.
/// </summary>
[GenerateMikrotikEntityMetadata, MikrotikEntity("log")]
public sealed class MikrotikLogEntry : IMikrotikEntity
{
    /// <inheritdoc />
    public MikrotikClient Client { get; }

    /// <summary>
    /// Gets the unique identifier of this entity.
    /// </summary>
    [DataMember(Name = ".id"), MikrotikReadonlyProperty]
    public string Id { get; internal set; }

    /// <summary>
    /// Gets the timestamp of the log entry.
    /// </summary>
    [DataMember(Name = "time"), MikrotikReadonlyProperty]
    public DateTimeOffset Timestamp { get; internal set; }

    /// <summary>
    /// Gets the log topics for this message.
    /// </summary>
    [DataMember(Name = "topics"), MikrotikReadonlyProperty]
    public IEnumerable<string> Topics { get; internal set; }

    /// <summary>
    /// Gets the contents of the log message.
    /// </summary>
    [DataMember(Name = "message"), MikrotikReadonlyProperty]
    public string Message { get; internal set; }

    internal MikrotikLogEntry(MikrotikClient client)
    {
        this.Client = client;
    }
}
