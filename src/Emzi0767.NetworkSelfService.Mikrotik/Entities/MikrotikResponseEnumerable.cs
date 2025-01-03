using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

internal sealed class MikrotikResponseEnumerable : IAsyncEnumerable<MikrotikSentence>, IDisposable, IAsyncDisposable
{
    private ImmutableArray<MikrotikSentence> _buffer = ImmutableArray<MikrotikSentence>.Empty;

    private readonly object _syncRoot = new();
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;
    private readonly ManualResetEventSlim _feedEvent = new(false);
    private Lazy<Task> _feedEventTask;
    
    public bool IsCompleted { get; private set; }
    
    public MikrotikSentence this[int index]
        => this._buffer[index];

    public MikrotikResponseEnumerable()
    {
        this._cancellationTokenSource = new();
        this._cancellationToken = this._cancellationTokenSource.Token;
        this._feedEventTask = new(this.CreateFeedEventTask);
    }
    
    public void Feed(MikrotikSentence sentence)
    {
        lock (this._syncRoot)
        {
            this._buffer = this._buffer.Add(sentence);
            var first = sentence.Words.First();
            if (first is MikrotikReplyWord { IsFinal: true })
                this.IsCompleted = true;

            if (this._feedEventTask.IsValueCreated && !this.IsCompleted)
                this._feedEventTask = new(this.CreateFeedEventTask);

            this._feedEvent.Set();
        }
    }

    public void Terminate()
    {
        try
        {
            lock (this._syncRoot)
            {
                this._cancellationTokenSource.Cancel();
                this._feedEvent.Set();
            }
        }
        catch { }
    }

    public ValueTask<bool> WaitForNextItemAsync(int currentIndex, CancellationToken cancellationToken = default)
    {
        if (currentIndex < this._buffer.Length)
            return ValueTask.FromResult(true);

        var t = _waitForItem(this._feedEventTask.Value);
        return new ValueTask<bool>(t);

        static async Task<bool> _waitForItem(Task _t)
        {
            try
            {
                await _t;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    
    public IAsyncEnumerator<MikrotikSentence> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new MikrotikResponseEnumerator(this, cancellationToken);

    public void Dispose()
    {
        this._feedEvent.Dispose();
        this._cancellationTokenSource.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        this.Dispose();
        return ValueTask.CompletedTask;
    }

    private Task CreateFeedEventTask()
        => this._feedEvent.WaitAsync(this._cancellationToken)
            .ContinueWith(t => this._feedEvent.Reset(), TaskContinuationOptions.OnlyOnRanToCompletion);
}