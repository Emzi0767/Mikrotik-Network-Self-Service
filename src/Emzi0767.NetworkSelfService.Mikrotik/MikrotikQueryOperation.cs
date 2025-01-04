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
/// Specifies the type of query operation to perform on the filter stack.
/// </summary>
public enum MikrotikQueryOperation
{
    /// <summary>
    /// Specifies unknown operation.
    /// </summary>
    Unknown,
    
    /// <summary>
    /// Specifies an operation that copies the value at a specified index. 
    /// </summary>
    [EnumMember(Value = $"{MikrotikQueryWord.ValuePlaceholder}+")]
    Copy,
    
    /// <summary>
    /// Specifies an operation that replaces all values with the value at a specified index.
    /// </summary>
    [EnumMember(Value = MikrotikQueryWord.ValuePlaceholder)]
    Replace,
    
    /// <summary>
    /// Specifies an operation that negates the top value.
    /// </summary>
    [EnumMember(Value = "!")]
    Not,
    
    /// <summary>
    /// Specifies an operation that pops top 2 values, performs logical AND, and pushes the result.
    /// </summary>
    [EnumMember(Value = "&")]
    And,
    
    /// <summary>
    /// Specifies an operation that pops top 2 values, performs logical OR, and pushes the result.
    /// </summary>
    [EnumMember(Value = "|")]
    Or,
    
    /// <summary>
    /// Specifies an operation that pushes a copy of the top value.
    /// </summary>
    [EnumMember(Value = ".")]
    CopyTop,
}