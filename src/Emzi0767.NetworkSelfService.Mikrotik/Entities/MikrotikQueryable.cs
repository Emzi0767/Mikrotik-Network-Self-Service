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
using Emzi0767.NetworkSelfService.Mikrotik.Exceptions;
using Emzi0767.NetworkSelfService.Mikrotik.Expressions;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

internal abstract class MikrotikQueryable
{
    protected readonly MikrotikClient _client;
    protected readonly string _requestId;
    protected readonly Type _rootType;

    public MikrotikQueryable(MikrotikClient client, string requestId, Type rootType)
    {
        this._client = client;
        this._requestId = requestId;
        this._rootType = rootType;
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new MikrotikQueryable<TElement>(this._client, this._requestId, this._rootType, expression);
}

internal sealed class MikrotikQueryable<T> : MikrotikQueryable, IAsyncQueryable<T>, IAsyncQueryProvider
{
    public Type ElementType
        => typeof(T);

    public IQueryProvider Provider
        => this;

    public Expression Expression { get; }

    private Lazy<MikrotikExpression> QueryExpression { get; }

    public MikrotikQueryable(MikrotikClient client, string requestId, Type rootType)
        : base(client, requestId, rootType)
    {
        this.Expression = Expression.Constant(this);
        this.QueryExpression = new(this.BuildQueryExpression);
    }

    public MikrotikQueryable(MikrotikClient client, string requestId, Type rootType, Expression expression)
        : base(client, requestId, rootType)
    {
        this.Expression = expression;
        this.QueryExpression = new(this.BuildQueryExpression);
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
        var expr = this.QueryExpression.Value;
        return this.ExecuteExpressionAsync(expr, cancellationToken).GetAsyncEnumerator(cancellationToken);
    }

    private async IAsyncEnumerable<T> ExecuteExpressionAsync(MikrotikExpression expression, CancellationToken cancellationToken = default)
    {
        var req = this._client.CreateRequest(expression.Words);
        await foreach (var response in req.Responses)
        {
            var first = response.Words.First();
            if (first is MikrotikReplyWord { IsError: true })
            {
                this._client.EndRequest(req);
                this.ParseAndThrow(response);
                break;
            }

            if (first is MikrotikReplyWord { IsFinal: true })
                break;

            if (first is not MikrotikReplyWord { Type: MikrotikReplyWordType.Data })
                continue;

            var data = new Dictionary<string, object>();
            foreach (var attr in response.Words.OfType<MikrotikAttributeWord>())
            {
                // TODO: parse
            }

            var instance = expression.Inflater.Inflate(this._client, data);
            if (expression.UnpackEnumerable)
                foreach (var item in instance as IEnumerable<T>)
                    yield return item;
            else
                yield return (T)instance;
        }

        this._client.EndRequest(req);
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
        var expr = this.QueryExpression.Value;
        throw new NotImplementedException();
    }

    private MikrotikExpression BuildQueryExpression()
    {
        var parser = MikrotikExpressionParser.Instance;
        return parser.Parse(this.Expression, this._rootType);
    }

    private void ParseAndThrow(in MikrotikSentence sentence)
    {
        var category = MikrotikApiErrorCategory.Unknown;
        var message = "";
        foreach (var word in sentence.Words)
        {
            if (word is not MikrotikAttributeWord attr)
                continue;

            switch (attr.Name)
            {
                case "category":
                    category = (MikrotikApiErrorCategory)int.Parse(attr.Value);
                    break;

                case "message":
                    message = attr.Value;
                    break;
            }
        }

        MikrotikThrowHelper.Throw_MikrotikApi(message, category);
    }
}
