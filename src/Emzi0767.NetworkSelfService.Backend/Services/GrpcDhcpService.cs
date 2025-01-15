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

    public override async Task<Result> GetInfo(Empty request, ServerCallContext context)
    {
        return new() { IsSuccess = false };
    }

    public override async Task<Result> GetLeases(Empty request, ServerCallContext context)
    {
        return new() { IsSuccess = false };
    }

    public override async Task<Result> QueryLeaseEligibility(DhcpAddressEligibilityQuery request, ServerCallContext context)
    {
        return new() { IsSuccess = false };
    }

    public override async Task<Result> CreateLease(DhcpLeaseCreateRequest request, ServerCallContext context)
    {
        return new() { IsSuccess = false };
    }

    public override async Task<Result> DeleteLease(DhcpLeaseDeleteRequest request, ServerCallContext context)
    {
        return new() { IsSuccess = false };
    }
}
