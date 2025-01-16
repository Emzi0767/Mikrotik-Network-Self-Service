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

using System.Threading.Tasks;
using Emzi0767.NetworkSelfService.gRPC;
using Emzi0767.NetworkSelfService.Mikrotik;
using Emzi0767.NetworkSelfService.Mikrotik.Entities;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Emzi0767.NetworkSelfService.Backend.Services;

[Authorize]
public sealed class GrpcLandingService : Landing.LandingBase
{
    private readonly ILogger<GrpcLandingService> _logger;
    private readonly NetworkRepository _networks;
    private readonly UserRepository _users;
    private readonly MikrotikProvider _mikrotik;

    public GrpcLandingService(
        ILogger<GrpcLandingService> logger,
        NetworkRepository networks,
        UserRepository users,
        MikrotikProvider mikrotik)
    {
        this._logger = logger;
        this._networks = networks;
        this._users = users;
        this._mikrotik = mikrotik;
    }

    private string GetUsername(ServerCallContext context)
        => context.GetHttpContext().User.GetName();

    public override async Task<Result> GetInformation(Empty request, ServerCallContext context)
    {
        var username = this.GetUsername(context);
        this._logger.LogInformation("Get network details for '{username}'", username);

        var user = await this._users.GetWithNetworkAsync(username, context.CancellationToken);
        var network = user.Network;
        var dhcp = await this._mikrotik.Get<MikrotikDhcpServer>()
            .FirstOrDefaultAsync(x => x.Name == network.DhcpServer, context.CancellationToken);

        var address = await this._mikrotik.Get<MikrotikIPv4Address>()
            .FirstOrDefaultAsync(x => x.Interface == dhcp.Interface, context.CancellationToken);

        var subnet = address.Address.AsSubnet();
        return new()
        {
            IsSuccess = true,
            Result_ = Any.Pack(new LandingResponse
            {
                Username = username,
                NetworkAddress = subnet.Address.ToString(),
                MaskCidr = subnet.CidrMask,
                MaskDotted = subnet.Mask.ToString(),
            }),
        };
    }
}
