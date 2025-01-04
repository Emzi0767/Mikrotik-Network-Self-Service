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

using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Represents a <see cref="IQueryProvider"/> that works asynchronously.
/// </summary>
public interface IAsyncQueryProvider : IQueryProvider
{
    /// <summary>
    /// Asynchronously executes this query against the data source.
    /// </summary>
    /// <param name="expression">Expression to evaluate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <typeparam name="TResult">Type of result to return.</typeparam>
    /// <returns>The evaluated result.</returns>
    ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default);
}