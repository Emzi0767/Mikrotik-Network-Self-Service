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

using System.Collections.Immutable;

namespace Emzi0767.NetworkSelfService.Mikrotik.SourceGens;

/// <summary>
/// Represents metadata of a Mikrotik entity.
/// </summary>
public readonly struct ObjectMetadata
{
    /// <summary>
    /// Gets whether the metadata object is initialized.
    /// </summary>
    public bool IsInitialized { get; }

    /// <summary>
    /// Gets the name of the object.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the qualified name of the object.
    /// </summary>
    public string QualifiedName { get; }

    /// <summary>
    /// Gets the members of this object.
    /// </summary>
    public ImmutableArray<ObjectMemberMetadata> Members { get; }

    /// <summary>
    /// Creates new object metadata.
    /// </summary>
    /// <param name="name">Name of the object.</param>
    /// <param name="qualifiedName">Qualified name of the object.</param>
    /// <param name="members">Information about the object's members.</param>
    public ObjectMetadata(string name, string qualifiedName, ImmutableArray<ObjectMemberMetadata> members)
    {
        this.IsInitialized = true;
        this.Name = name;
        this.QualifiedName = qualifiedName;
        this.Members = members;
    }
}
