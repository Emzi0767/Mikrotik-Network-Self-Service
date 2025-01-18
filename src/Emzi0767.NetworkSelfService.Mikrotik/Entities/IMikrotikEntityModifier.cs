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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

/// <summary>
/// Represents a set of modifications to make to an entity.
/// </summary>
public interface IMikrotikEntityModifier<T>
    where T : class, IMikrotikEntity
{
    /// <summary>
    /// Gets the extra properties for this request.
    /// </summary>
    IMikrotikExtraProperties<T> Extras { get; }

    /// <summary>
    /// Begins modifying the entity.
    /// </summary>
    /// <param name="propertySelector">Selector of the property to modify.</param>
    /// <param name="newValue">New value for the property.</param>
    /// <typeparam name="TProp">Type of the selected property.</typeparam>
    /// <returns>A modifier which allows for further changes to be made, or queued changes to be executed.</returns>
    IMikrotikEntityModifier<T> Set<TProp>(Expression<Func<T, TProp>> propertySelector, TProp newValue);

    /// <summary>
    /// Asynchronously executes scheduled changes to the entity.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the operation.</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);
}
