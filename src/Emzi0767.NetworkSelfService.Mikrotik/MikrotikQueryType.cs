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

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Specifies the type of query to Mikrotik API.
/// </summary>
public enum MikrotikQueryType
{
    /// <summary>
    /// Specifies an unknown query.
    /// </summary>
    Unknown,
    
    /// <summary>
    /// Specifies that the query filters for objects with a given property.
    /// </summary>
    [EnumMember(Value = "")]
    HasProperty,
    
    /// <summary>
    /// Specifies that the query filters for objects which have no property of a given name.
    /// </summary>
    [EnumMember(Value = "-")]
    LacksProperty,
    
    /// <summary>
    /// Specifies that the query filters for objects with a given property's value equal to a given value.
    /// </summary>
    [EnumMember(Value = "")]
    Equals,
    
    /// <summary>
    /// Specifies that the query filters for objects with a given property's value being less than a given value.
    /// </summary>
    [EnumMember(Value = "<")]
    LessThan,
    
    /// <summary>
    /// Specifies that the query filters for objects with a given property's value being greater than a given value.
    /// </summary>
    [EnumMember(Value = ">")]
    GreaterThan,
    
    /// <summary>
    /// Specifies an operation on the filter result stack.
    /// </summary>
    [EnumMember(Value = "#")]
    Operation,
}