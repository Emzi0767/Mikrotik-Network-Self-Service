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
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Emzi0767.NetworkSelfService.Backend.Configuration;
using Emzi0767.NetworkSelfService.Backend.Data;
using Emzi0767.NetworkSelfService.gRPC;
using Emzi0767.NetworkSelfService.Mikrotik;
using Emzi0767.NetworkSelfService.Mikrotik.Entities;
using Emzi0767.NetworkSelfService.Mikrotik.Types;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Enum = System.Enum;

namespace Emzi0767.NetworkSelfService.Backend.Services;

[Authorize]
public sealed class GrpcWifiService : Wifi.WifiBase
{
    private readonly ILogger<GrpcWifiService> _logger;
    private readonly UserRepository _users;
    private readonly NetworkRepository _networks;
    private readonly ApMappingRepository _apMappings;
    private readonly MikrotikProvider _mikrotikProvider;
    private readonly MikrotikConfiguration _config;

    public GrpcWifiService(
        ILogger<GrpcWifiService> logger,
        UserRepository users,
        NetworkRepository networks,
        ApMappingRepository apMappings,
        MikrotikProvider mikrotikProvider,
        IOptions<MikrotikConfiguration> config)
    {
        this._logger = logger;
        this._users = users;
        this._networks = networks;
        this._apMappings = apMappings;
        this._mikrotikProvider = mikrotikProvider;
        this._config = config.Value;
    }

    private string GetUsername(ServerCallContext context)
        => context.GetHttpContext().User.GetName();

    private async Task<WifiBasicInfo> GetBasicsAsync(ServerCallContext context)
    {
        var user = await this._users.GetWithNetworkAsync(this.GetUsername(context), context.CancellationToken);
        var network = user.Network;
        var apMap = await this._apMappings.GetMappingDictionaryAsync(context.CancellationToken);
        return new(user, network, apMap);
    }

    private async Task<List<MikrotikCapsmanInterfaceConfiguration>> GetConfigurationsAsync(string interfaceList, ServerCallContext context)
        => await this._mikrotikProvider.Get<MikrotikCapsmanInterfaceConfiguration>()
            .Where(x => x.InterfaceListName == interfaceList)
            .AsAsyncQueryable()
            .ToListAsync(context.CancellationToken);

    private async Task<MikrotikCapsmanDatapath> GetDatapathAsync(string interfaceList, ServerCallContext context)
        => await this._mikrotikProvider.Get<MikrotikCapsmanDatapath>()
            .FirstOrDefaultAsync(x => x.InterfaceList == interfaceList, context.CancellationToken);

    private async IAsyncEnumerable<WifiAcl> GetAclsAsync(string interfaceList, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var acl in this._mikrotikProvider.Get<MikrotikCapsmanAcl>()
            .Where(x => x.InterfaceList == interfaceList)
            .AsAsyncQueryable())
            yield return MakeAcl(acl);
    }

    private async Task<WifiRecentAttemptsResponse> GetRecentAttemptsAsync(IReadOnlyDictionary<string, string> ifaceApMap, IEnumerable<WifiAcl> wifiAcls, ServerCallContext context)
    {
        var log = (await this._mikrotikProvider.Get<MikrotikLogEntry>()
                .ToListAsync(context.CancellationToken))
            .Where(x => x.Topics.SequenceEqual([ "caps", "info" ]) && x.Message.EndsWith("rejected, forbidden by access-list"))
            .DistinctBy(x => x.Message)
            .Select(x => WifiLoggedAttempt.Parse(x.Message))
            .Where(x => ifaceApMap.ContainsKey(x.Interface))
            .DistinctBy(x => x.Address)
            .Select(x => x.Address)
            .Select(x => x.ToString())
            .Except(wifiAcls.Select(x => x.MacAddress).ToHashSet());

        var recentAttempts = new WifiRecentAttemptsResponse();
        recentAttempts.MacAddresses.AddRange(log);

        return recentAttempts;
    }

    private async Task<IReadOnlyDictionary<string, string>> GetInterfaceApMapAsync(IReadOnlyDictionary<string, string> apMap, IEnumerable<MikrotikCapsmanInterfaceConfiguration> configurations, ServerCallContext context)
    {
        var radios = await this._mikrotikProvider.Get<MikrotikCapsmanRadio>()
            .ToListAsync(context.CancellationToken);

        var radioMap = radios.ToDictionary(x => x.InterfaceName, x => x.Identity);
        return configurations
            .ToDictionary(x => x.Name, x => apMap[radioMap[x.MasterInterfaceName is null or "none" ? x.Name : x.MasterInterfaceName]]);
    }

    private async Task<IEnumerable<WifiConnectedDevice>> GetConnectedDevicesAsync(IEnumerable<MikrotikCapsmanInterfaceConfiguration> configurations, IReadOnlyDictionary<string, string> ifaceApMap, ServerCallContext context)
    {
        var ifaces = configurations.Select(x => x.Name).ToArray();
        var registrations = await this._mikrotikProvider.Get<MikrotikCapsmanRegistration>()
            .WhereIn(x => x.InterfaceName, ifaces)
            .AsAsyncQueryable()
            .ToListAsync(context.CancellationToken);

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

        return registrations.Select(x => new WifiConnectedDevice
            {
                MacAddress = x.MacAddress.ToString(),
                Ap = ifaceApMap[x.InterfaceName],
                Comment = x.Comment,
                Band = bandMap[x.InterfaceName],
            })
            .ToArray();
    }

    private async Task<WifiInfo> GetWifiInfoAsync(ServerCallContext context)
    {
        var (user, network, apMap) = await this.GetBasicsAsync(context);
        var configurations = await this.GetConfigurationsAsync(network.WirelessInterfaceList, context);
        var datapath = await this.GetDatapathAsync(network.WirelessInterfaceList, context);
        var ifaceApMap = await this.GetInterfaceApMapAsync(apMap, configurations, context);

        var config = new WifiConfigResponse
        {
            Ssid = configurations.First().Ssid,
            IsolateClients = !(datapath.EnableLocalForwarding && datapath.EnableClientToClientForwarding),
        };

        var wifiAcls = await this.GetAclsAsync(network.WirelessInterfaceList).EToListAsync(context.CancellationToken);
        var recentAttempts = await this.GetRecentAttemptsAsync(ifaceApMap, wifiAcls, context);
        var connected = await this.GetConnectedDevicesAsync(configurations, ifaceApMap, context);

        return new(
            user,
            network,
            apMap,
            config,
            wifiAcls,
            recentAttempts,
            connected);
    }

    private async Task<WifiConfigResponse> GetWifiConfigurationAsync(ServerCallContext context)
    {
        var (user, network, apMap) = await this.GetBasicsAsync(context);
        var configurations = await this.GetConfigurationsAsync(network.WirelessInterfaceList, context);
        var datapath = await this.GetDatapathAsync(network.WirelessInterfaceList, context);

        return new()
        {
            Ssid = configurations.First().Ssid,
            IsolateClients = !(datapath.EnableLocalForwarding && datapath.EnableClientToClientForwarding),
        };
    }

    public override async Task<Result> GetInformation(Empty request, ServerCallContext context)
    {
        var info = await this.GetWifiInfoAsync(context);
        this._logger.LogInformation("Get Wi-Fi info for '{username}'", info.User.Username);

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
        this._logger.LogInformation("Get Wi-Fi configuration for '{username}'", this.GetUsername(context));
        var config = await this.GetWifiConfigurationAsync(context);

        return new() { IsSuccess = true, Result_ = Any.Pack(config), };
    }

    public override async Task<Result> GetAcls(Empty request, ServerCallContext context)
    {
        var (_, network, _) = await this.GetBasicsAsync(context);
        this._logger.LogInformation("Get Wi-Fi configuration for '{username}'", this.GetUsername(context));
        var wifiAcls = await this.GetAclsAsync(network.WirelessInterfaceList).EToListAsync(context.CancellationToken);
        var acls = new WifiAclResponse();
        acls.Acls.AddRange(wifiAcls);

        return new() { IsSuccess = true, Result_ = Any.Pack(acls), };
    }

    public override async Task<Result> GetRecentConnectionAttempts(Empty request, ServerCallContext context)
    {
        var (_, network, apMap) = await this.GetBasicsAsync(context);
        var configurations = await this.GetConfigurationsAsync(network.WirelessInterfaceList, context);
        var ifaceApMap = await this.GetInterfaceApMapAsync(apMap, configurations, context);
        var wifiAcls = await this.GetAclsAsync(network.WirelessInterfaceList).EToListAsync(context.CancellationToken);
        var recents = await this.GetRecentAttemptsAsync(ifaceApMap, wifiAcls, context);

        return new() { IsSuccess = true, Result_ = Any.Pack(recents), };
    }

    public override async Task<Result> GetConnectedDevices(Empty request, ServerCallContext context)
    {
        var (_, network, apMap) = await this.GetBasicsAsync(context);
        var configurations = await this.GetConfigurationsAsync(network.WirelessInterfaceList, context);
        var ifaceApMap = await this.GetInterfaceApMapAsync(apMap, configurations, context);
        var connected = await this.GetConnectedDevicesAsync(configurations, ifaceApMap, context);
        var ret = new WifiConnectedDevicesResponse();
        ret.Devices.AddRange(connected);

        return new() { IsSuccess = true, Result_ = Any.Pack(ret), };
    }

    public override async Task<Result> UpdateConfiguration(WifiUpdateRequest request, ServerCallContext context)
    {
        if (!request.HasSsid && !request.HasPassword && !request.HasIsolateClients)
            return new() { IsSuccess = true, };

        var (_, network, _) = await this.GetBasicsAsync(context);
        var datapath = await this._mikrotikProvider.Get<MikrotikCapsmanDatapath>()
            .FirstOrDefaultAsync(x => x.InterfaceList == network.WirelessInterfaceList, context.CancellationToken);

        var configurations = await this._mikrotikProvider.Get<MikrotikCapsmanConfiguration>()
            .Where(x => x.DatapathName == datapath.Name)
            .AsAsyncQueryable()
            .ToListAsync(context.CancellationToken);

        if (request.HasSsid)
        {
            await Task.WhenAll(configurations.Select(c => c.Modify()
                .Set(x => x.Ssid, request.Ssid)
                .CommitAsync(context.CancellationToken)));
        }

        if (request.HasPassword)
        {
            var securityProfileName = configurations.First().SecurityProfileName;
            var securityProfile = await this._mikrotikProvider.Get<MikrotikCapsmanSecurityProfile>()
                .FirstOrDefaultAsync(x => x.Name == securityProfileName, context.CancellationToken);

            await securityProfile.Modify()
                .Set(x => x.Password, request.Password)
                .CommitAsync(context.CancellationToken);
        }

        if (request.HasIsolateClients)
        {
            await datapath.Modify()
                .Set(x => x.EnableClientToClientForwarding, !request.IsolateClients)
                .Set(x => x.EnableLocalForwarding, !request.IsolateClients)
                .CommitAsync(context.CancellationToken);
        }

        return new() { IsSuccess = true, };
    }

    public override async Task<Result> CreateAcl(WifiAclCreateRequest request, ServerCallContext context)
    {
        var (_, network, _) = await this.GetBasicsAsync(context);

        var mac = MacAddress.Parse(request.MacAddress, CultureInfo.InvariantCulture);

        var baseAclQuery = this._mikrotikProvider.Get<MikrotikCapsmanAcl>();
        var baseAcl = this._config.PlaceBeforeAcl is null or { Count: 0 }
            ? await baseAclQuery.LastOrDefaultAsync(x => x.Action == MikrotikCapsmanAclAction.Reject && x.InterfaceList == "any", context.CancellationToken)
            : await baseAclQuery.WhereMapped(this._config.PlaceBeforeAcl)
                .AsAsyncQueryable()
                .LastOrDefaultAsync(context.CancellationToken);

        if (baseAcl is null)
            return new() { IsSuccess = false, };

        var acl = this._mikrotikProvider.Create<MikrotikCapsmanAcl>()
            .Set(x => x.InterfaceList, network.WirelessInterfaceList)
            .Set(x => x.Address, mac)
            .Set(x => x.InterfaceList, network.WirelessInterfaceList)
            .Set(x => x.AllowSignalOutOfRange, TimeSpan.FromSeconds(10))
            .Set(x => x.Action, MikrotikCapsmanAclAction.Accept)
            .Set(x => x.EnableRadiusAccounting, false)
            .Set(x => x.Comment, request.Comment);

        if (request.HasPrivatePassword)
        {
            if (request.PrivatePassword.Length is < 8 or > 63)
                return new() { IsSuccess = false, };

            acl = acl.Set(x => x.PrivatePassword, request.PrivatePassword);
        }

        if (request.TimeRestriction is not null)
        {
            var timeRestriction = new MikrotikTimeRange(
                request.TimeRestriction.Start.ToTimeSpan(),
                request.TimeRestriction.End.ToTimeSpan(),
                request.TimeRestriction.Days.Select(x => x switch
                    {
                        WifiWeekday.Monday => MikrotikWeekday.Monday,
                        WifiWeekday.Tuesday => MikrotikWeekday.Tuesday,
                        WifiWeekday.Wednesday => MikrotikWeekday.Wednesday,
                        WifiWeekday.Thursday => MikrotikWeekday.Thursday,
                        WifiWeekday.Friday => MikrotikWeekday.Friday,
                        WifiWeekday.Saturday => MikrotikWeekday.Saturday,
                        WifiWeekday.Sunday => MikrotikWeekday.Sunday,
                        _ => MikrotikWeekday.None,
                    })
                    .Aggregate(MikrotikWeekday.None, (c, n) => c | n));

            acl = acl.Set(x => x.Time, timeRestriction);
        }

        acl.Extras.PlaceBefore = baseAcl;
        await acl.CommitAsync(context.CancellationToken);
        return new() { IsSuccess = true, };
    }

    public override async Task<Result> DeleteAcl(WifiAclDeleteRequest request, ServerCallContext context)
    {
        var (_, network, _) = await this.GetBasicsAsync(context);

        var acl = await this._mikrotikProvider.Get<MikrotikCapsmanAcl>()
            .FirstOrDefaultAsync(x => x.InterfaceList == network.WirelessInterfaceList && x.Id == request.Identifier, context.CancellationToken);

        if (acl is null)
            return new() { IsSuccess = false, };

        await acl.DeleteAsync(context.CancellationToken);
        return new() { IsSuccess = true, };
    }

    public override async Task<Result> UpdateAcl(WifiAclUpdateRequest request, ServerCallContext context)
    {
        var (_, network, _) = await this.GetBasicsAsync(context);
        var acl = await this._mikrotikProvider.Get<MikrotikCapsmanAcl>()
            .FirstOrDefaultAsync(x => x.InterfaceList == network.WirelessInterfaceList && x.Id == request.Identifier, context.CancellationToken);

        var aclMod = acl.Modify();
        if (request.HasMacAddress)
        {
            var mac = MacAddress.Parse(request.MacAddress, CultureInfo.InvariantCulture);
            aclMod = aclMod.Set(x => x.Address, mac);
        }

        if (request.HasComment)
            aclMod = aclMod.Set(x => x.Comment, request.Comment);

        if (request.HasPrivatePassword)
        {
            if (request.PrivatePassword.Length is < 8 or > 63)
                return new() { IsSuccess = false, };

            aclMod = aclMod.Set(x => x.PrivatePassword, request.PrivatePassword);
        }

        if (request.HasRemovePrivatePassword && request.RemovePrivatePassword)
            aclMod = aclMod.Set(x => x.PrivatePassword, null);

        if (request.TimeRestriction is not null)
        {
            var timeRestriction = new MikrotikTimeRange(
                request.TimeRestriction.Start.ToTimeSpan(),
                request.TimeRestriction.End.ToTimeSpan(),
                request.TimeRestriction.Days.Select(x => x switch
                    {
                        WifiWeekday.Monday => MikrotikWeekday.Monday,
                        WifiWeekday.Tuesday => MikrotikWeekday.Tuesday,
                        WifiWeekday.Wednesday => MikrotikWeekday.Wednesday,
                        WifiWeekday.Thursday => MikrotikWeekday.Thursday,
                        WifiWeekday.Friday => MikrotikWeekday.Friday,
                        WifiWeekday.Saturday => MikrotikWeekday.Saturday,
                        WifiWeekday.Sunday => MikrotikWeekday.Sunday,
                        _ => MikrotikWeekday.None,
                    })
                    .Aggregate(MikrotikWeekday.None, (c, n) => c | n));

            aclMod = aclMod.Set(x => x.Time, timeRestriction);
        }

        if (request.HasRemoveTimeRestriction && request.RemoveTimeRestriction)
            aclMod = aclMod.Set(x => x.Time, null);

        if (request.HasIsEnabled)
            aclMod = aclMod.Set(x => x.Disabled, !request.IsEnabled);

        await aclMod.CommitAsync(context.CancellationToken);
        return new() { IsSuccess = true, };
    }

    private readonly record struct WifiBasicInfo(
        DbUser User,
        DbNetwork Network,
        IReadOnlyDictionary<string, string> ApMap);

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
            Enum.GetValues<MikrotikWeekday>()
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
            IsSpecialEntry = x.VlanId is not null || x.VlanMode is not null || x.Address is null,
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
