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

namespace Emzi0767.NetworkSelfService.Mikrotik.SourceGens;

/// <summary>
/// Represents metadata of a Mikrotik entity's member.
/// </summary>
public readonly struct EntityMemberMetadata
{
    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the fully-qualified name of the property type.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the serialized property name.
    /// </summary>
    public string SerializedName { get; }

    /// <summary>
    /// Gets whether a member is read-only.
    /// </summary>
    public bool IsReadOnly { get; }

    /// <summary>
    /// Creates new entity member metadata.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="type">Fully qualified name of the property's type.</param>
    /// <param name="serializedName">Name of the property in serialized data.</param>
    /// <param name="isReadOnly">Whether the member is read-only.</param>
    public EntityMemberMetadata(string name, string type, string serializedName, bool isReadOnly)
    {
        this.Name = name;
        this.Type = type;
        this.SerializedName = serializedName;
        this.IsReadOnly = isReadOnly;
    }
}
