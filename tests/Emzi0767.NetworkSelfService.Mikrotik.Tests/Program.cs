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

using System.Net;
using System.Text.Json;
using Emzi0767.NetworkSelfService.Mikrotik.Entities;

namespace Emzi0767.NetworkSelfService.Mikrotik.Tests;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var configJson = args.Length > 0 ? args[0] : null;
        using var json = File.OpenRead(configJson);
        var config = await JsonSerializer.DeserializeAsync<Config>(json).ConfigureAwait(false);

        using var mikrotik = new MikrotikClient(
            new()
            {
                TlsOptions = new()
                {
                    UseTls = config.UseTls,
                    AllowObsoleteTlsVersions = config.AllowObsoleteTlsVersions,
                },
                Username = config.Username,
                Password = config.Password,
            }
        );

        await mikrotik.ConnectAsync(new DnsEndPoint(config.Hostname, config.Port));
        Console.WriteLine("! CONNECTED");

        await Task.Delay(TimeSpan.FromSeconds(1));
        Console.WriteLine("! SETTLED");

        var countQuery = await mikrotik.Get<MikrotikDhcpLease>()
            //.OfType<MikrotikDhcpLease>()
            .Where(x => x.Server != null)
            .Select(x => new { s = x.Server, b = x.AccessBlocked, d = x.AddressLists, })
            .Select(x => new { srv = x.s, x.b, x.d, })
            .Where(x => x.b)
            .SelectMany(x => x.d)
            .AsAsyncQueryable()
            .CountAsync();
        Console.WriteLine("! SENT INTERFACE LIST QUERY");

        await Task.Delay(TimeSpan.FromSeconds(3));
        Console.WriteLine("! SPENT TIME");

        await mikrotik.DisconnectAsync();
        Console.WriteLine("! DISCONNECTED");
    }
}
