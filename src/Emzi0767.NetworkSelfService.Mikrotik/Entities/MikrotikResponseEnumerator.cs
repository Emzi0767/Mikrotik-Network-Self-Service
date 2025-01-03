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