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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Emzi0767.NetworkSelfService.Backend.Configuration;
using Emzi0767.NetworkSelfService.Mikrotik;
using Emzi0767.NetworkSelfService.Mikrotik.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Emzi0767.NetworkSelfService.Backend.Services;

public sealed class MikrotikProvider : IHostedService
{
    private readonly MikrotikClient _client;
    private readonly MikrotikConfiguration _config;
    private readonly IEnumerable<AddressFamily> _addressFamilies;

    public MikrotikProvider(IOptions<MikrotikConfiguration> opts)
    {
        this._config = opts.Value;
        this._client = new(new()
        {
            Username = this._config.Username,
            Password = this._config.Password,
            TlsOptions = new()
            {
                UseTls = this._config.EnableTls,
                AllowObsoleteTlsVersions = this._config.AllowObsoleteTlsVersions
            },
        });

        this._addressFamilies = (this._config.EnableIpv4, this._config.EnableIpv6) switch
        {
            (true, false) => [ AddressFamily.InterNetwork, ],
            (false, true) => [ AddressFamily.InterNetworkV6, ],
            (true, true) => [ AddressFamily.InterNetwork, AddressFamily.InterNetworkV6, ],
            _ => throw new ArgumentException("Must enable at least IPv4 or IPv6."),
        };
    }

    public IAsyncQueryable<T> Get<T>()
        where T : class, IMikrotikEntity
        => this._client.Get<T>();

    public async Task StartAsync(CancellationToken cancellationToken)
        => await this._client.ConnectAsync(new DnsEndPoint(this._config.Hostname, this._config.Port), this._addressFamilies, cancellationToken);

    public async Task StopAsync(CancellationToken cancellationToken)
        => await this._client.DisconnectAsync(cancellationToken);
}
