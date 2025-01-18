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
/// Non-generic base for extra properties.
/// </summary>
public interface IMikrotikExtraProperties
{ }

/// <summary>
/// Represents extra properties for a given entity create/modify action.
/// </summary>
/// <typeparam name="T">Type of entity.</typeparam>
public interface IMikrotikExtraProperties<T> : IMikrotikExtraProperties
    where T : class, IMikrotikEntity
{
    /// <summary>
    /// Gets or sets the entity to place the created/modified entity before.
    /// </summary>
    T PlaceBefore { get; set; }

    /// <summary>
    /// Gets or sets the entity to place the created/modified entity after.
    /// </summary>
    T PlaceAfter { get; set; }
}
