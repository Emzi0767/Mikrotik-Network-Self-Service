using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

internal sealed class MikrotikQueryable<T> : IAsyncQueryable<T>, IAsyncQueryProvider
{
    public Type ElementType
        => typeof(T);

    public IQueryProvider Provider
        => this;
    
    public Expression Expression { get; }

    private readonly MikrotikClient _client;
    private readonly string _requestId;

    public MikrotikQueryable(MikrotikClient client, string requestId)
    {
        this._client = client;
        this._requestId = requestId;
        this.Expression = Expression.Constant(client);
    }

    private MikrotikQueryable(MikrotikClient client, string requestId, Expression expression)
    {
        this._client = client;
        this._requestId = requestId;
        this.Expression = expression;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        throw new NotImplementedException();
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new MikrotikQueryable<TElement>(this._client, this._requestId, expression);

    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
        => this.GetEnumerator();

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public TResult Execute<TResult>(Expression expression)
    {
        throw new NotImplementedException();
    }

    public object Execute(Expression expression)
        => this.Execute<object>(expression);

    public ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}