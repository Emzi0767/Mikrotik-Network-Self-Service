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
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Emzi0767.NetworkSelfService.Backend.Services;

public sealed class GrpcAuthenticationService : Authentication.AuthenticationBase
{
    private readonly ILogger<GrpcAuthenticationService> _logger;

    public GrpcAuthenticationService(ILogger<GrpcAuthenticationService> logger)
    {
        this._logger = logger;
    }

    public override Task<Result> Authenticate(AuthenticationRequest request, ServerCallContext context)
    {
        return Task.FromResult(new Result { IsSuccess = false, Error = new() { Code = ErrorCode.Unknown } });
    }

    public override Task<Result> RefreshSession(SessionRefreshRequest request, ServerCallContext context)
    {
        return Task.FromResult(new Result { IsSuccess = false, Error = new() { Code = ErrorCode.Unknown } });
    }

    public override Task<Result> DestroySession(SessionDestroyRequest request, ServerCallContext context)
    {
        return Task.FromResult(new Result { IsSuccess = false, Error = new() { Code = ErrorCode.Unknown } });
    }
}
