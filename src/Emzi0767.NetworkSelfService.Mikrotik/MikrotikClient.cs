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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Emzi0767.NetworkSelfService.Mikrotik.Entities;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Allows for interfacing with Mikrotik RouterOS devices via the API.
/// </summary>
public sealed class MikrotikClient : IDisposable
{
    private readonly MikrotikApiClient _api;
    private readonly MikrotikClientConfiguration _config;
    private volatile int _requestCounter = 0;
    private IDictionary<string, MikrotikRequest> _outstandingRequests;

    /// <summary>
    /// Creates a new Mikrotik API client.
    /// </summary>
    /// <param name="config">Configuration for this client.</param>
    public MikrotikClient(MikrotikClientConfiguration config)
    {
        this._config = config;
        this._outstandingRequests = new ConcurrentDictionary<string, MikrotikRequest>();

        this._api = new(this._config.TlsOptions);
        this._api.SentenceReceived += this.SentenceReceivedAsync;
    }

    /// <summary>
    /// Connects to the API and begins processing events.
    /// </summary>
    /// <param name="endpoint">Endpoint to connect to.</param>
    /// <param name="addressFamilies">Address families to resolve the hostname to.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task ConnectAsync(DnsEndPoint endpoint, IEnumerable<AddressFamily> addressFamilies = null, CancellationToken cancellationToken = default)
    {
        await this._api.ConnectAsync(endpoint, addressFamilies, cancellationToken);
        await this.LoginAsync(cancellationToken);
    }

    /// <summary>
    /// Connects to the API and begins processing events.
    /// </summary>
    /// <param name="endpoint">Endpoint to connect to.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task ConnectAsync(IPEndPoint endpoint, CancellationToken cancellationToken = default)
    {
        await this._api.ConnectAsync(endpoint, cancellationToken);
        await this.LoginAsync(cancellationToken);
    }

    private async Task LoginAsync(CancellationToken cancellationToken)
    {
        var login = new MikrotikSentence([
            new MikrotikCommandWord(["login"]),
            new MikrotikAttributeWord("name", this._config.Username),
            new MikrotikAttributeWord("password", this._config.Password),
            MikrotikStopWord.Instance,
        ]);
        var req = new MikrotikRequest(null, login);
        this._outstandingRequests[""] = req;

        await this._api.SendAsync(login, cancellationToken);
        await req.AwaitCompletionAsync(cancellationToken);
        this._outstandingRequests.Remove("");
    }

    internal async Task SendAsync(MikrotikRequest request, CancellationToken cancellationToken = default)
    {
        this._outstandingRequests[request.Tag] = request;
        await this._api.SendAsync(request.Request, cancellationToken);
    }

    internal void EndRequest(MikrotikRequest request)
        => this._outstandingRequests.Remove(request.Tag);

    /// <summary>
    /// Disconnects from the API and stops processing events.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        => await this._api.DisconnectAsync(cancellationToken);

    /// <summary>
    /// Retrieves specified entities from the API.
    /// </summary>
    /// <typeparam name="T">Type of entity to retrieve.</typeparam>
    /// <returns>A queryable which allows for specifying retrieval parameters.</returns>
    public IAsyncQueryable<T> Get<T>()
        where T : class, IMikrotikEntity
        => new MikrotikQueryable<T>(this, this.AssignRequestId(), typeof(T));

    /// <summary>
    /// Creates a new entity using the API.
    /// </summary>
    /// <typeparam name="T">Type of entity to create.</typeparam>
    /// <returns>An interactive builder instance, which allows for building the entity.</returns>
    public IMikrotikEntityModifier<T> Create<T>()
        where T : class, IMikrotikEntity
        => throw new NotImplementedException();

    internal MikrotikRequest CreateRequest(IEnumerable<IMikrotikWord> words)
    {
        var reqId = this.AssignRequestId();
        var reqWord = new MikrotikSentenceAttributeWord(MikrotikSentenceAttributeWord.Tag, reqId);
        var reqWords = words.Take(1)
            .Append(reqWord)
            .Concat(words.Skip(1))
            .ToArray();

        var sentence = new MikrotikSentence(reqWords);
        return new(reqId, sentence);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var request in this._outstandingRequests.Values)
            request.Dispose();

        this._outstandingRequests.Clear();
        this._api.Dispose();
    }

    private Task SentenceReceivedAsync(MikrotikApiClient client, MikrotikSentenceReceivedEventArgs ea)
    {
        var sentence = ea.Sentence;
        var tagAttribute = sentence.Words.OfType<MikrotikSentenceAttributeWord>()
            .FirstOrDefault(x => x.Name == MikrotikSentenceAttributeWord.Tag);

        var tag = tagAttribute?.Value ?? "";
        if (!this._outstandingRequests.TryGetValue(tag, out var req))
            return Task.CompletedTask;

        req.Feed(sentence);
        if (req.IsCompleted)
            this._outstandingRequests.Remove(tag);

        return Task.CompletedTask;
    }

    private string AssignRequestId()
    {
        var id = Interlocked.Increment(ref this._requestCounter);
        return $"emzi-mikrotik-request-{id:x}";
    }
}
