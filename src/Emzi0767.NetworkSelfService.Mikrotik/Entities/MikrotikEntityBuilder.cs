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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

internal abstract class MikrotikEntityBuilder<T> : IMikrotikEntityModifier<T>
    where T : class, IMikrotikEntity
{
    private IDictionary<string, object> _setValues = new Dictionary<string, object>();
    private IList<IMikrotikWord> _extraWords = new List<IMikrotikWord>();
    private readonly MikrotikClient _client;

    protected abstract string Command { get; }

    public MikrotikEntityBuilder(MikrotikClient client)
    {
        if (client is null)
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(client), "Client cannot be null.");

        this._client = client;
    }

    /// <inheritdoc />
    public IMikrotikExtraProperties<T> Extras { get; } = new MikrotikExtraProperties<T>();

    /// <inheritdoc />
    public IMikrotikEntityModifier<T> Set<TProp>(Expression<Func<T, TProp>> propertySelector, TProp newValue)
    {
        if (EntityProxies.IsReadOnly(propertySelector))
            MikrotikThrowHelper.Throw_InvalidOperation("Cannot set the value of a readonly property.");

        var mapped = EntityProxies.MapToSerialized(propertySelector);
        this._setValues[mapped] = newValue;
        return this;
    }

    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        var words = new List<IMikrotikWord>(this._setValues.Count + this._extraWords.Count + 2);
        var cmd = new MikrotikCommandWord([ ..EntityProxies.GetPath<T>(), this.Command ]);
        words.Add(cmd);
        words.AddRange(this._extraWords);

        foreach (var (k, v) in this._setValues)
            words.Add(new MikrotikAttributeWord(k, v.ToMikrotikString()));

        words.Add(MikrotikStopWord.Instance);

        var req = this._client.CreateRequest(words);
        await this._client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        await req.AwaitCompletionAsync(cancellationToken).ConfigureAwait(false);
    }

    protected void AddExtraWord(IMikrotikWord word)
        => this._extraWords.Add(word);
}
