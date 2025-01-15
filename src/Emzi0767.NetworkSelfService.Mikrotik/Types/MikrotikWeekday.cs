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

namespace Emzi0767.NetworkSelfService.Mikrotik.Types;

/// <summary>
/// Specifies weekdays.
/// </summary>
[Flags, GenerateEnumMetadata]
public enum MikrotikWeekday
{
    /// <summary>
    /// Specifies no weekdays.
    /// </summary>
    None = 0,

    /// <summary>
    /// Specifies monday.
    /// </summary>
    [DataMember(Name = "mon")]
    Monday = 1 << 0,

    /// <summary>
    /// Specifies monday.
    /// </summary>
    [DataMember(Name = "tue")]
    Tuesday = 1 << 1,

    /// <summary>
    /// Specifies monday.
    /// </summary>
    [DataMember(Name = "wed")]
    Wednesday = 1 << 2,

    /// <summary>
    /// Specifies monday.
    /// </summary>
    [DataMember(Name = "thu")]
    Thursday = 1 << 3,

    /// <summary>
    /// Specifies monday.
    /// </summary>
    [DataMember(Name = "fri")]
    Friday = 1 << 4,

    /// <summary>
    /// Specifies monday.
    /// </summary>
    [DataMember(Name = "sat")]
    Saturday = 1 << 5,

    /// <summary>
    /// Specifies monday.
    /// </summary>
    [DataMember(Name = "sun")]
    Sunday = 1 << 6,
}
