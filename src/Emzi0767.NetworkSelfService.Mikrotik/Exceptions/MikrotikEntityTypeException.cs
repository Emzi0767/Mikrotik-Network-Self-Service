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

namespace Emzi0767.NetworkSelfService.Mikrotik.Exceptions;

/// <summary>
/// Indicates that an invalid, unsupported, or unexpected type was used where a Mikrotik entity type was expected.
/// </summary>
public sealed class MikrotikEntityTypeException : Exception
{
    /// <summary>
    /// Gets the entity type that was attempted to be used.
    /// </summary>
    public Type EntityType { get; }

    /// <summary>
    /// Creates a new exception indicating that an invalid type was used in place of a Mikrotik entity type.
    /// </summary>
    /// <param name="entityType">Type that was used.</param>
    public MikrotikEntityTypeException(Type entityType)
        : this(entityType, "Invalid type was used as Mikrotik entity type.")
    { }

    /// <summary>
    /// Creates a new exception indicating that an invalid type was used in place of a Mikrotik entity type.
    /// </summary>
    /// <param name="entityType">Type that was used.</param>
    /// <param name="message">Exception message.</param>
    public MikrotikEntityTypeException(Type entityType, string message)
        : base(message)
    {
        this.EntityType = entityType;
    }
}
