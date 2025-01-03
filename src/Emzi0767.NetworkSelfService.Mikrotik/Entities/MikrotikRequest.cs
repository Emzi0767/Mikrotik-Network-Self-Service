using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

internal sealed class MikrotikRequest : IDisposable
{
    public string Tag { get; }
    public MikrotikSentence Request { get; }

    public bool IsCompleted
        => this._responses.IsCompleted;

    public IAsyncEnumerable<MikrotikSentence> Responses
        => this._responses;

    private readonly MikrotikResponseEnumerable _responses;

    public MikrotikRequest(string tag, MikrotikSentence request)
    {
        this.Tag = tag;
        this.Request = request;
        this._responses = new();
    }

    public async Task AwaitCompletionAsync(CancellationToken cancellationToken = default)
    {
        await foreach (var response in this.Responses.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            var firstWord = response.Words.First();
            if (firstWord is MikrotikReplyWord { IsFinal: true })
                return;
        }
        
        MikrotikThrowHelper.Throw_RequestTerminatedEarly();
    }

    public void Feed(MikrotikSentence sentence)
        => this._responses.Feed(sentence);

    public void TerminateEarly()
        => this._responses.Terminate();

    public void Dispose()
        => this._responses.Dispose();
}