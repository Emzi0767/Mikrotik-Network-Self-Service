using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Emzi0767.NetworkSelfService.Mikrotik.Entities;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Allows for interfacing with Mikrotik RouterOS devices via the API.
/// </summary>
public sealed class MikrotikClient
{
    private readonly MikrotikApiClient _api;
    private readonly MikrotikClientConfiguration _config;
    private volatile int _requestCounter = 0;

    /// <summary>
    /// Creates a new Mikrotik API client.
    /// </summary>
    /// <param name="config">Configuration for this client.</param>
    public MikrotikClient(MikrotikClientConfiguration config)
    {
        this._config = config;

        this._api = new(this._config.TlsOptions);
        this._api.SentenceReceived += this.SentenceReceivedAsync;
    }

    /// <summary>
    /// Connects to the API and begins processing events.
    /// </summary>
    /// <param name="endpoint">Endpoint to connect to.</param>
    /// <param name="addressFamilies">Address families to resolve the hostname to.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task ConnectAsync(DnsEndPoint endpoint, IEnumerable<AddressFamily> addressFamilies = default, CancellationToken cancellationToken = default)
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
        
        await this._api.SendAsync(login, cancellationToken);
        // TODO: await response
    }

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
        where T : IMikrotikEntity
        => new MikrotikQueryable<T>(this, this.AssignRequestId());

    private async Task SentenceReceivedAsync(MikrotikApiClient client, MikrotikSentenceReceivedEventArgs ea)
    {
        
    }

    private string AssignRequestId()
    {
        var id = Interlocked.Increment(ref this._requestCounter);
        return $"emzi-mikrotik-request-{id:x}";
    }
}