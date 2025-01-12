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

namespace Emzi0767.NetworkSelfService.Mikrotik.SourceGens;

/// <summary>
/// Represents metadata of an enum.
/// </summary>
public readonly struct EnumMetadata
{
    /// <summary>
    /// Gets whether the metadata is initialized.
    /// </summary>
    public bool IsInitialized { get; }

    /// <summary>
    /// Gets the name of the enum.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the qualified name of the enum type.
    /// </summary>
    public string QualifiedName { get; }

    /// <summary>
    /// Gets the mapping of member to serialized name on the enum.
    /// </summary>
    public IReadOnlyDictionary<string, string> MemberMappings { get; }

    /// <summary>
    /// Creates new enum metadata instance.
    /// </summary>
    /// <param name="name">Name of the enum type.</param>
    /// <param name="qualifiedName">Qualified name of the enum type.</param>
    /// <param name="memberMappings">Mapping of member name to serialized name.</param>
    public EnumMetadata(string name, string qualifiedName, IReadOnlyDictionary<string, string> memberMappings)
    {
        this.IsInitialized = true;
        this.Name = name;
        this.QualifiedName = qualifiedName;
        this.MemberMappings = memberMappings;
    }
}
