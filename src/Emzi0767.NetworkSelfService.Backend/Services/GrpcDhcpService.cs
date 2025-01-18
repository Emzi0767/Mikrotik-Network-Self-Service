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
using System.Net;
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

namespace Emzi0767.NetworkSelfService.Backend.Services;

[Authorize]
public sealed class GrpcDhcpService : Dhcp.DhcpBase
{
    private readonly ILogger<GrpcDhcpService> _logger;
    private readonly UserRepository _users;
    private readonly NetworkRepository _networks;
    private readonly MikrotikProvider _mikrotikProvider;

    public GrpcDhcpService(
        ILogger<GrpcDhcpService> logger,
        UserRepository users,
        NetworkRepository networks,
        MikrotikProvider mikrotikProvider)
    {
        this._logger = logger;
        this._users = users;
        this._networks = networks;
        this._mikrotikProvider = mikrotikProvider;
    }

    private string GetUsername(ServerCallContext context)
        => context.GetHttpContext().User.GetName();

    private async Task<DhcpInfo> GetDhcpInfoAsync(ServerCallContext context)
    {
        var user = await this._users.GetWithNetworkAsync(this.GetUsername(context), context.CancellationToken);
        var network = user.Network;

        var dhcpServer = await this._mikrotikProvider
            .Get<MikrotikDhcpServer>()
            .FirstOrDefaultAsync(x => x.Name == network.Name, context.CancellationToken);

        var ipAddress = await this._mikrotikProvider
            .Get<MikrotikIPv4Address>()
            .FirstOrDefaultAsync(x => x.Interface == dhcpServer.Interface, context.CancellationToken);

        var ipPool = await this._mikrotikProvider
            .Get<MikrotikIPv4Pool>()
            .FirstOrDefaultAsync(x => x.Name == dhcpServer.AddressPoolName, context.CancellationToken);

        return new(user, network, dhcpServer, ipAddress, ipPool);
    }

    private async IAsyncEnumerable<DhcpLease> GetLeasesAsync(ServerCallContext context)
    {
        var user = await this._users.GetWithNetworkAsync(this.GetUsername(context), context.CancellationToken);
        var network = user.Network;

        var dhcpServer = await this._mikrotikProvider
            .Get<MikrotikDhcpServer>()
            .FirstOrDefaultAsync(x => x.Name == network.Name, context.CancellationToken);

        await foreach (var lease in this.GetLeasesAsync(dhcpServer, context).WithCancellation(context.CancellationToken).ConfigureAwait(false))
            yield return lease;
    }

    private async IAsyncEnumerable<DhcpLease> GetLeasesAsync(MikrotikDhcpServer server, ServerCallContext context)
    {
        var leases = this._mikrotikProvider
            .Get<MikrotikDhcpLease>()
            .Where(x => x.Server == server.Name)
            .AsAsyncQueryable();

        await foreach (var lease in leases.WithCancellation(context.CancellationToken).ConfigureAwait(false))
        {
            var mtLease = new DhcpLease()
            {
                Id = lease.Id,
                MacAddress = lease.MacAddress.ToString(),
                IpAddress = lease.Address.ToString(),
                IsDynamic = lease.IsDynamic,
            };

            if (lease.Hostname is not null)
                mtLease.Hostname = lease.Hostname;

            yield return mtLease;
        }
    }

    private async Task<IEnumerable<MikrotikDhcpLease>> GetLeaseInfosAsync(MikrotikDhcpServer server, ServerCallContext context)
        => await this._mikrotikProvider
            .Get<MikrotikDhcpLease>()
            .Where(x => x.Server == server.Name)
            .AsAsyncQueryable()
            .ToListAsync(context.CancellationToken);

    public override async Task<Result> GetInfo(Empty request, ServerCallContext context)
    {
        var info = await this.GetDhcpInfoAsync(context);
        this._logger.LogInformation("Get DHCP info for '{username}'", info.User.Username);

        var subnet = info.IpAddress.Address.AsSubnet();
        var range = info.IpPool.Ranges.Single();
        var leases = await this.GetLeasesAsync(info.DhcpServer, context)
            .EToListAsync(context.CancellationToken);

        var response = new DhcpInfoResponse
        {
            Configuration = new()
            {
                Network = subnet.Address.ToString(),
                Mask = subnet.CidrMask,
                MaskDotted = subnet.Mask.ToString(),
                Broadcast = subnet.Broadcast.ToString(),
            },
            DhcpRange = new()
            {
                StartIp = range.Start.ToString(),
                EndIp = range.End.ToString(),
            },
            RouterAddress = info.IpAddress.Address.Address.ToString(),
        };

        response.Leases.AddRange(leases);

        return new()
        {
            IsSuccess = true,
            Result_ = Any.Pack(response),
        };
    }

    public override async Task<Result> GetLeases(Empty request, ServerCallContext context)
    {
        this._logger.LogInformation("Get DHCP leases for '{username}'", this.GetUsername(context));
        var leases = await this.GetLeasesAsync(context)
            .EToListAsync(context.CancellationToken);

        var packed = new DhcpLeases();
        packed.Leases.AddRange(leases);

        return new()
        {
            IsSuccess = true,
            Result_ = Any.Pack(packed),
        };
    }

    public override async Task<Result> QueryLeaseEligibility(DhcpAddressEligibilityQuery request, ServerCallContext context)
    {
        var info = await this.GetDhcpInfoAsync(context);
        this._logger.LogInformation("Query eligibility of '{ip}' for '{username}'", request.IpAddress, info.User.Username);

        var subnet = info.IpAddress.Address.AsSubnet();
        var range = info.IpPool.Ranges.Single();
        var leases = await this.GetLeaseInfosAsync(info.DhcpServer, context);

        var address = IPAddress.Parse(request.IpAddress);
        var mac = MacAddress.Parse(request.MacAddress, CultureInfo.InvariantCulture);

        var result = new DhcpAddressEligibilityResponse();
        var flags = result.Flags;
        if (range.Contains(address))
            flags.Add(DhcpAddressEligibility.DhcpOverlap);

        if (!subnet.Contains(address))
        {
            if (address.Equals(subnet.Address))
                flags.Add(DhcpAddressEligibility.BaseConflict);
            else if (address.Equals(subnet.Broadcast))
                flags.Add(DhcpAddressEligibility.BroadcastConflict);
            else
                flags.Add(DhcpAddressEligibility.OutOfRange);
        }

        if (address.Equals(info.IpAddress.Address.Address))
            flags.Add(DhcpAddressEligibility.InfrastructureConflict);

        foreach (var lease in leases)
        {
            if (address.Equals(lease.Address) && !mac.Equals(lease.MacAddress))
                flags.Add(lease.IsDynamic
                    ? DhcpAddressEligibility.ActiveLeaseConflict
                    : DhcpAddressEligibility.StaticLeaseConflict);
        }

        if (flags.Count == 0)
            flags.Add(DhcpAddressEligibility.Ok);

        return new()
        {
            IsSuccess = true,
            Result_ = Any.Pack(result),
        };
    }

    public override async Task<Result> CreateLease(DhcpLeaseCreateRequest request, ServerCallContext context)
    {
        var info = await this.GetDhcpInfoAsync(context);
        this._logger.LogInformation("Create lease of '{ip}' for '{username}'", request.IpAddress, info.User.Username);

        var subnet = info.IpAddress.Address.AsSubnet();

        var address = IPAddress.Parse(request.IpAddress);
        var mac = MacAddress.Parse(request.MacAddress, CultureInfo.InvariantCulture);

        if (!subnet.Contains(address) || address.Equals(subnet.Address) || address.Equals(subnet.Broadcast))
            return new() { IsSuccess = false, };

        if (address.Equals(info.IpAddress.Address.Address))
            return new() { IsSuccess = false, };

        await this._mikrotikProvider.Create<MikrotikDhcpLease>()
            .Set(x => x.Server, info.Network.DhcpServer)
            .Set(x => x.Address, address)
            .Set(x => x.MacAddress, mac)
            .CommitAsync(context.CancellationToken);

        return new() { IsSuccess = true };
    }

    public override async Task<Result> DeleteLease(DhcpLeaseDeleteRequest request, ServerCallContext context)
    {
        var info = await this.GetDhcpInfoAsync(context);
        this._logger.LogInformation("Create lease '{id}' for '{username}'", request.Id, info.User.Username);

        var lease = await this._mikrotikProvider.Get<MikrotikDhcpLease>()
            .FirstOrDefaultAsync(x => x.Server == info.Network.DhcpServer && x.Id == request.Id, context.CancellationToken);

        if (lease is null)
            return new() { IsSuccess = false, };

        await lease.DeleteAsync(context.CancellationToken);
        return new() { IsSuccess = true };
    }

    private readonly record struct DhcpInfo(
        DbUser User,
        DbNetwork Network,
        MikrotikDhcpServer DhcpServer,
        MikrotikIPv4Address IpAddress,
        MikrotikIPv4Pool IpPool);
}
