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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

internal abstract class MikrotikQueryable
{
    protected readonly MikrotikClient _client;
    protected readonly string _requestId;

    public MikrotikQueryable(MikrotikClient client, string requestId)
    {
        this._client = client;
        this._requestId = requestId;
    }
    
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new MikrotikQueryable<TElement>(this._client, this._requestId, expression);
}

internal sealed class MikrotikQueryable<T> : MikrotikQueryable, IAsyncQueryable<T>, IAsyncQueryProvider
{
    public Type ElementType
        => typeof(T);

    public IQueryProvider Provider
        => this;
    
    public Expression Expression { get; }

    public MikrotikQueryable(MikrotikClient client, string requestId)
        : base(client, requestId)
    {
        this.Expression = Expression.Constant(this);
    }

    public MikrotikQueryable(MikrotikClient client, string requestId, Expression expression)
        : base(client, requestId)
    {
        this.Expression = expression;
    }
    
    public IQueryable CreateQuery(Expression expression)
    {
        var createQueryGeneric = expression.CreateQueryGeneric();
        return createQueryGeneric.Invoke(this, [ expression ]) as IQueryable;
    }

    public IEnumerator<T> GetEnumerator()
    {
        MikrotikThrowHelper.Throw_SyncNotSupported();
        return null;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => this.GetEnumerator();

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public TResult Execute<TResult>(Expression expression)
    {
        MikrotikThrowHelper.Throw_SyncNotSupported();
        return default;
    }

    public object Execute(Expression expression)
        => this.Execute<object>(expression);

    public ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}