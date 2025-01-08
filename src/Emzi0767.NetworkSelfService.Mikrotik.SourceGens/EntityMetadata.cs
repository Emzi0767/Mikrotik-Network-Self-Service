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
public readonly struct EntityMetadata
{
    /// <summary>
    /// Gets whether the metadata object is initialized.
    /// </summary>
    public bool IsInitialized { get; }

    /// <summary>
    /// Gets the name of the entity.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the qualified name of the entity.
    /// </summary>
    public string QualifiedName { get; }

    /// <summary>
    /// Gets the members of this entity.
    /// </summary>
    public ImmutableArray<EntityMemberMetadata> Members { get; }

    /// <summary>
    /// Gets the API resource path.
    /// </summary>
    public ImmutableArray<string> Path { get; }

    /// <summary>
    /// Creates new entity metadata.
    /// </summary>
    /// <param name="name">Name of the entity.</param>
    /// <param name="qualifiedName">Qualified name of the entity.</param>
    /// <param name="members">Information about the entity's members.</param>
    /// <param name="path">Path to the API resource.</param>
    public EntityMetadata(string name, string qualifiedName, ImmutableArray<EntityMemberMetadata> members, ImmutableArray<string> path)
    {
        this.IsInitialized = true;
        this.Name = name;
        this.QualifiedName = qualifiedName;
        this.Members = members;
        this.Path = path;
    }
}
