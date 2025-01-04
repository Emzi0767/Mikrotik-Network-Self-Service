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

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

/// <summary>
/// Represents a Mikrotik API entity that can be modified.
/// </summary>
public interface IMikrotikModifiableEntity<T>
    where T : class, IMikrotikEntity
{
    /// <summary>
    /// Begins modifying the entity.
    /// </summary>
    /// <returns>A modifier which allows for further changes to be made, or queued changes to be executed.</returns>
    IMikrotikEntityModifier<T> Modify();
}