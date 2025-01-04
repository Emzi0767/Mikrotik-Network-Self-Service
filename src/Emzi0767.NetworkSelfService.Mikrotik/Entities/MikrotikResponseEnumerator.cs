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
using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

internal sealed class MikrotikResponseEnumerator : IAsyncEnumerator<MikrotikSentence>
{
    public MikrotikSentence Current
        => this._source[this._index];

    private int _index = -1;
    private readonly MikrotikResponseEnumerable _source;
    private readonly CancellationToken _cancellationToken;

    public MikrotikResponseEnumerator(MikrotikResponseEnumerable source, CancellationToken cancellationToken)
    {
        this._source = source;
        this._cancellationToken = cancellationToken;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        var result = this._source.WaitForNextItemAsync(this._index + 1, this._cancellationToken);
        if (result.IsCompleted)
        {
            if (result.Result) ++this._index;
            
            return result;
        }

        var t = _moveNextAsync();
        return new ValueTask<bool>(t);

        async Task<bool> _moveNextAsync()
        {
            var r = await result;
            if (r) ++this._index;

            return r;
        }
    }
    
    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}