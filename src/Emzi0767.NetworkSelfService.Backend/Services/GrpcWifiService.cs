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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Emzi0767.NetworkSelfService.Backend.Data;
using Emzi0767.NetworkSelfService.gRPC;
using Emzi0767.NetworkSelfService.Mikrotik;
using Emzi0767.NetworkSelfService.Mikrotik.Entities;
using Emzi0767.NetworkSelfService.Mikrotik.Types;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Emzi0767.NetworkSelfService.Backend.Services;

[Authorize]
public sealed class GrpcWifiService : Wifi.WifiBase
{
    private readonly ILogger<GrpcWifiService> _logger;
    private readonly UserRepository _users;
    private readonly NetworkRepository _networks;
    private readonly ApMappingRepository _apMappings;
    private readonly MikrotikProvider _mikrotikProvider;

    public GrpcWifiService(
        ILogger<GrpcWifiService> logger,
        UserRepository users,
        NetworkRepository networks,
        ApMappingRepository apMappings,
        MikrotikProvider mikrotikProvider)
    {
        this._logger = logger;
        this._users = users;
        this._networks = networks;
        this._apMappings = apMappings;
        this._mikrotikProvider = mikrotikProvider;
    }

    private string GetUsername(ServerCallContext context)
        => context.GetHttpContext().User.GetName();

    private async Task<WifiInfo> GetWifiInfoAsync(ServerCallContext context)
    {
        var user = await this._users.GetWithNetworkAsync(this.GetUsername(context), context.CancellationToken);
        var network = user.Network;
        var apMap = await this._apMappings.GetMappingDictionaryAsync(context.CancellationToken);

        var configurations = await this._mikrotikProvider.Get<MikrotikCapsmanInterfaceConfiguration>()
            .Where(x => x.InterfaceListName == network.WirelessInterfaceList)
            .AsAsyncQueryable()
            .ToListAsync(context.CancellationToken);

        var radios = await this._mikrotikProvider.Get<MikrotikCapsmanRadio>()
            .ToListAsync(context.CancellationToken);

        var ifaces = configurations.Select(x => x.MasterInterfaceName ?? x.Name).ToArray();
        var registrations = await this._mikrotikProvider.Get<MikrotikCapsmanRegistration>()
            .WhereIn(x => x.InterfaceName, ifaces)
            .AsAsyncQueryable()
            .ToListAsync(context.CancellationToken);

        var radioMap = radios.ToDictionary(x => x.InterfaceName, x => x.Identity);
        var ifaceApMap = configurations
            .Select(x => x.MasterInterfaceName ?? x.Name)
            .ToDictionary(x => x, x => radioMap[x]);

        var bandMap = configurations.ToDictionary(x => x.Name, x => x.WirelessBand switch
            {
                MikrotikWirelessBand.B_2_4GHz
                    or MikrotikWirelessBand.BG_2_4GHz
                    or MikrotikWirelessBand.BGN_2_4GHz
                    or MikrotikWirelessBand.GN_2_4GHz
                    or MikrotikWirelessBand.G_2_4GHz
                    or MikrotikWirelessBand.N_2_4GHz
                    => WifiBand.Band24Ghz,

                MikrotikWirelessBand.A_5GHz
                    or MikrotikWirelessBand.AN_5GHz
                    or MikrotikWirelessBand.ANAC_5GHz
                    or MikrotikWirelessBand.NAC_5GHz
                    or MikrotikWirelessBand.AC_5GHz
                    or MikrotikWirelessBand.N_5GHz
                    => WifiBand.Band5Ghz,

                _ => WifiBand.BandUnknown,
            });

        var datapath = await this._mikrotikProvider.Get<MikrotikCapsmanDatapath>()
            .FirstOrDefaultAsync(x => x.InterfaceList == network.WirelessInterfaceList, context.CancellationToken);

        var config = new WifiConfigResponse
        {
            Ssid = configurations.First().Ssid,
            IsolateClients = datapath.EnableLocalForwarding && datapath.EnableClientToClientForwarding,
        };

        var acls = await this._mikrotikProvider.Get<MikrotikCapsmanAcl>()
            .Where(x => x.InterfaceList == network.WirelessInterfaceList)
            .AsAsyncQueryable()
            .ToListAsync(context.CancellationToken);

        var wifiAcls = acls.Select(MakeAcl)
            .ToArray();

        var log = (await this._mikrotikProvider.Get<MikrotikLogEntry>()
            .ToListAsync(context.CancellationToken))
            .Where(x => x.Topics.SequenceEqual([ "caps", "info" ]) && x.Message.EndsWith("rejected, forbidden by access-list"))
            .DistinctBy(x => x.Message)
            .Select(x => WifiLoggedAttempt.Parse(x.Message))
            .Where(x => ifaceApMap.ContainsKey(x.Interface))
            .Select(x => x.Address.ToString());

        var recentAttempts = new WifiRecentAttemptsResponse();
        recentAttempts.MacAddresses.AddRange(log);

        var connected = registrations.Select(x => new WifiConnectedDevice
            {
                MacAddress = x.MacAddress.ToString(),
                Ap = ifaceApMap[x.InterfaceName],
                Comment = x.Comment,
                Band = bandMap[x.InterfaceName],
            })
            .ToArray();

        return new(
            user,
            network,
            apMap,
            config,
            wifiAcls,
            recentAttempts,
            connected);
    }

    public override async Task<Result> GetInfo(Empty request, ServerCallContext context)
    {
        var info = await this.GetWifiInfoAsync(context);
        this._logger.LogInformation("Get DHCP info for '{username}'", info.User.Username);

        var response = new WifiInfoResponse
        {
            Configuration = info.Config,
            ConnectedDevices = new(),
            RecentAttempts = info.RecentAttempts,
            AccessControl = new(),
        };

        response.ConnectedDevices.Devices.AddRange(info.ConnectedDevices);
        response.AccessControl.Acls.AddRange(info.Acls);

        return new() { IsSuccess = true, Result_ = Any.Pack(response), };
    }

    public override async Task<Result> GetConfiguration(Empty request, ServerCallContext context)
    {
        return new() { IsSuccess = false };
    }

    public override async Task<Result> GetAcls(Empty request, ServerCallContext context)
    {
        return new() { IsSuccess = false };
    }

    public override async Task<Result> GetRecentConnectionAttempts(Empty request, ServerCallContext context)
    {
        return new() { IsSuccess = false };
    }

    public override async Task<Result> GetConnectedDevices(Empty request, ServerCallContext context)
    {
        return new() { IsSuccess = false };
    }

    public override async Task<Result> UpdateConfiguration(WifiUpdateRequest request, ServerCallContext context)
    {
        return new() { IsSuccess = false };
    }

    public override async Task<Result> CreateAcl(WifiAclCreateRequest request, ServerCallContext context)
    {
        return new() { IsSuccess = false };
    }

    public override async Task<Result> DeleteAcl(WifiAclCreateRequest request, ServerCallContext context)
    {
        return new() { IsSuccess = false };
    }

    public override async Task<Result> UpdateAcl(WifiAclUpdateRequest request, ServerCallContext context)
    {
        return new() { IsSuccess = false };
    }

    private readonly record struct WifiInfo(
        DbUser User,
        DbNetwork Network,
        IReadOnlyDictionary<string, string> ApMap,
        WifiConfigResponse Config,
        IEnumerable<WifiAcl> Acls,
        WifiRecentAttemptsResponse RecentAttempts,
        IEnumerable<WifiConnectedDevice> ConnectedDevices);

    private readonly record struct WifiLoggedAttempt(
        MacAddress Address,
        string Interface)
    {
        public static WifiLoggedAttempt Parse(string message)
        {
            var msg = new StringSegment(message);
            var macIface = msg.Split([ ' ' ]).First().Split([ '@' ]);
            using var enumerator = macIface.GetEnumerator();

            enumerator.MoveNext();
            var address = MacAddress.Parse(enumerator.Current, CultureInfo.InvariantCulture);

            enumerator.MoveNext();
            var iface = enumerator.Current.ToString();

            return new(address, iface);
        }
    }

    private static WifiTimeRestriction MakeRestriction(MikrotikTimeRange range)
    {
        var restriction = new WifiTimeRestriction
        {
            Start = Duration.FromTimeSpan(range.Start),
            End = Duration.FromTimeSpan(range.End),
        };

        restriction.Days.AddRange(
            System.Enum.GetValues<MikrotikWeekday>()
                .Where(x => x != MikrotikWeekday.None && range.Weekdays.HasFlag(x))
                .Select(x => x switch
                {
                    MikrotikWeekday.Monday => WifiWeekday.Monday,
                    MikrotikWeekday.Tuesday => WifiWeekday.Tuesday,
                    MikrotikWeekday.Wednesday => WifiWeekday.Wednesday,
                    MikrotikWeekday.Thursday => WifiWeekday.Thursday,
                    MikrotikWeekday.Friday => WifiWeekday.Friday,
                    MikrotikWeekday.Saturday => WifiWeekday.Saturday,
                    MikrotikWeekday.Sunday => WifiWeekday.Sunday,
                    _ => WifiWeekday.Unknown,
                }));

        return restriction;
    }

    private static WifiAcl MakeAcl(MikrotikCapsmanAcl x)
    {
        var acl = new WifiAcl
        {
            Id = x.Id,
            Comment = x.Comment,
            IsEnabled = !x.Disabled,
            IsSpecialEntry = x.VlanId is not null || x.VlanMode is not null,
        };

        if (x.Address is not null)
            acl.MacAddress = x.Address?.ToString();

        if (x.PrivatePassword is not null)
            acl.PrivatePassword = x.PrivatePassword;

        if (x.Time is not null)
            acl.TimeRestriction = MakeRestriction(x.Time.Value);

        return acl;
    }
}
