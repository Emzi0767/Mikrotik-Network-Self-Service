using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emzi0767.Types;
using Emzi0767.Utilities;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Provides low-level functionality for interacting with Mikrotik's API.
/// </summary>
public sealed class MikrotikApiClient : IDisposable, IAsyncDisposable
{
    private static AddressFamily[] _supportedAddressFamilies = [ AddressFamily.InterNetwork, AddressFamily.InterNetworkV6 ];
    
    private Stream _stream;
    private TcpClient _client;
    private CancellationTokenSource _cts;

    private readonly AsyncEvent<MikrotikApiClient, MikrotikSentenceReceivedEventArgs> _sentenceReceived;
    private readonly SemaphoreSlim _semaphore = new(1);
    private readonly IMemoryBuffer<byte> _buffer = new ContinuousMemoryBuffer<byte>();
    
    /// <summary>
    /// Gets the TLS options for this client instance.
    /// </summary>
    public MikrotikTlsOptions TlsOptions { get; }
    
    /// <summary>
    /// Creates a new Mikrotik API client that uses plaintext connections.
    /// </summary>
    public MikrotikApiClient()
        : this(default)
    { }

    /// <summary>
    /// Creates a new Mikrotik API client with specified TLS options.
    /// </summary>
    /// <param name="tlsOptions">Options for TLS connections.</param>
    public MikrotikApiClient(MikrotikTlsOptions tlsOptions)
    {
        this.TlsOptions = tlsOptions;
        this._sentenceReceived = new("SENTENCE_RECEIVED", TimeSpan.FromSeconds(1), this.AsyncEventException);
    }

    /// <summary>
    /// Attempts to connect to the API.
    /// </summary>
    /// <param name="endpoint">Endpoint the API is located at.</param>
    /// <param name="addressFamilies">Address families to try to resolve the hostname to.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task ConnectAsync(DnsEndPoint endpoint, IEnumerable<AddressFamily> addressFamilies = default, CancellationToken cancellationToken = default)
    {
        addressFamilies ??= _supportedAddressFamilies;
        if (addressFamilies != _supportedAddressFamilies)
        {
            foreach (var addressFamily in addressFamilies)
            {
                if (!_supportedAddressFamilies.Contains(addressFamily))
                    MikrotikThrowHelper.Throw_Argument(nameof(addressFamilies), "Unsupported address family specified.");
            }
        }

        var addrs = new List<IPAddress>();
        foreach (var addressFamily in addressFamilies)
        {
            try
            {
                var entry = await Dns.GetHostEntryAsync(endpoint.Host, addressFamily, cancellationToken).ConfigureAwait(false);
                addrs.AddRange(entry.AddressList);
            }
            catch { }
        }

        var endpoints = addrs.Select(x => new IPEndPoint(x, endpoint.Port));
        await this.ConnectAsync(endpoints, endpoint.Host, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Attempts to connect to the API.
    /// </summary>
    /// <param name="endpoint">Endpoint the API is located at.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task ConnectAsync(IPEndPoint endpoint, CancellationToken cancellationToken = default)
        => await this.ConnectAsync([ endpoint ], endpoint.Address.ToString(), cancellationToken).ConfigureAwait(false);

    private async Task ConnectAsync(IEnumerable<IPEndPoint> endpoints, string hostname, CancellationToken cancellationToken = default)
    {
        if (this._stream is not null)
            MikrotikThrowHelper.Throw_InvalidOperation("Cannot connect when already connected.");
        
        await this._semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            foreach (var endpoint in endpoints)
            {
                try
                {
                    await this.ConnectAsync(endpoint, hostname, cancellationToken).ConfigureAwait(false);
                    break;
                }
                catch
                {
                    this._client?.Dispose();
                    if (this._stream is not null) await this._stream.DisposeAsync().ConfigureAwait(false);
                    this._client = null;
                    this._stream = null;
                    continue;
                }
            }
            
            if (this._client is null || this._stream is null)
                MikrotikThrowHelper.Throw_Connection("Could not connect to the router using specified parameters.");
            
            this._cts = new CancellationTokenSource();
            var token = this._cts.Token;
            _ = this.ReadLoop(this._stream, token);
        }
        finally
        {
            this._semaphore.Release();
        }
    }
    
    private async Task ConnectAsync(IPEndPoint endpoint, string hostname, CancellationToken cancellationToken = default)
    {
        this._client = new TcpClient();
        await this._client.ConnectAsync(endpoint.Address, endpoint.Port, cancellationToken).ConfigureAwait(false);

        this._stream = this._client.GetStream();
        if (this.TlsOptions.UseTls)
        {
            var tls = this.TlsOptions.CertificateValidationCallback is null
                ? new SslStream(this._stream, true)
                : new SslStream(this._stream, true, this.TlsOptions.CertificateValidationCallback);
            
            await tls.AuthenticateAsClientAsync(new()
            {
                TargetHost = hostname,
                EnabledSslProtocols = this.TlsOptions.AllowObsoleteTlsVersions
                    ? SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13
                    : SslProtocols.Tls12 | SslProtocols.Tls13,
                EncryptionPolicy = EncryptionPolicy.RequireEncryption,
            }, cancellationToken).ConfigureAwait(false);

            this._stream = tls;
        }
    }

    /// <summary>
    /// Attempts to disconnect from the API.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (this._stream is null)
            MikrotikThrowHelper.Throw_InvalidOperation("Cannot disconnect when not connected.");
        
        await this._semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            this._cts.Cancel();
            this._cts.Dispose();
            await this._stream.DisposeAsync().ConfigureAwait(false);
            this._client.Close();
            this._client.Dispose();

            this._stream = null;
            this._client = null;
        }
        finally
        {
            this._semaphore.Release();
        }
    }

    /// <summary>
    /// Sends a sentence to the API. A response, if any, will be propagated via <see cref="SentenceReceived"/> event.
    /// </summary>
    /// <param name="sentence">Sentence to send to the API.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task SendAsync(MikrotikSentence sentence, CancellationToken cancellationToken = default)
    {
        if (this._stream is null)
            MikrotikThrowHelper.Throw_InvalidOperation("Cannot send data when not connected.");
        
        await this._semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            this._buffer.Clear();
            if (!sentence.TryEncode(this._buffer))
                MikrotikThrowHelper.Throw_InvalidData("Invalid sentence.");
            
            await this._buffer.CopyToAsync(this._stream, cancellationToken).ConfigureAwait(false);
            await this._stream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            this._semaphore.Release();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        try
        {
            this._cts.Cancel();
            this._cts.Dispose();
        }
        catch { }

        this._buffer.Dispose();
        this._semaphore.Dispose();
        this._stream?.Dispose();
        this._client?.Dispose();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        try
        {
            this._cts.Cancel();
            this._cts.Dispose();
        }
        catch { }

        this._buffer.Dispose();
        this._semaphore.Dispose();

        if (this._stream is not null)
            await this._stream.DisposeAsync().ConfigureAwait(false);

        this._client?.Dispose();
    }
    
    /// <summary>
    /// Fired whenever a sentence is received from the API.
    /// </summary>
    public event AsyncEventHandler<MikrotikApiClient, MikrotikSentenceReceivedEventArgs> SentenceReceived
    {
        add => this._sentenceReceived.Register(value);
        remove => this._sentenceReceived.Unregister(value);
    }
    
    /// <summary>
    /// Fired whenever an exception is thrown in an event handler.
    /// </summary>
    public event TypedEventHandler<MikrotikApiClient, MikrotikExceptionEventArgs> Exception;

    private async Task ReadLoop(Stream stream, CancellationToken cancellationToken)
    {
        var words = new List<IMikrotikWord>();
        using var chars = new ContinuousMemoryBuffer<char>();
        using var buff = new ContinuousMemoryBuffer<byte>();
        using var memown = MemoryPool<byte>.Shared.Rent(5);
        var mem = memown.Memory[..5];
        var read = 0;
        var br = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            var len = 0L;
            do br += await stream.ReadAsync(mem[br..], cancellationToken).ConfigureAwait(false);
            while (!mem.Span.TryDecodeLength(out len, out read));

            var m = mem[read..br];
            if (m.Length >= len)
            {
                buff.Write(m[..(int)len].Span);
                m = m[(int)len..];
                m.CopyTo(mem);
                br = m.Length;
            }
            else
            {
                br = 0;
                buff.Write(m.Span);
                var remaining = (ulong)(len - m.Length);
                while (remaining > 0)
                {
                    await buff.WriteAsync(stream, remaining, cancellationToken).ConfigureAwait(false);
                    remaining = (ulong)len - buff.Count;
                }
            }

            MikrotikHelpers.DefaultEncoding.GetChars(buff.AsSequence(), chars);
            var decoded = new string(chars.Span);
            buff.Clear();
            chars.Clear();
            
            if (!decoded.TryParseWord(out var word))
                continue;
            
            words.Add(word);
            if (word is MikrotikStopWord)
            {
                var sentence = new MikrotikSentence(words);
                var ea = new MikrotikSentenceReceivedEventArgs()
                {
                    Sentence = sentence,
                };
                
                await this._sentenceReceived.InvokeAsync(this, ea).ConfigureAwait(false);
                words = new();

                if (sentence.Words.First() is MikrotikReplyWord { Type: MikrotikReplyWordType.ConnectionTermination })
                {
                    await this.DisconnectAsync(cancellationToken).ConfigureAwait(false);
                    break;
                }
            }
        }
    }

    private void AsyncEventException<TArgs>(AsyncEvent<MikrotikApiClient, TArgs> asyncEvent, Exception exception, AsyncEventHandler<MikrotikApiClient, TArgs> handler, MikrotikApiClient sender, TArgs args)
        where TArgs : AsyncEventArgs
    {
        if (this.Exception is not null)
            this.Exception(this, new()
            {
                Exception = exception,
                Event = asyncEvent,
                EventArgs = args,
                Handler = handler as AsyncEventHandler<MikrotikApiClient, AsyncEventArgs>,
            });
    }
}