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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Emzi0767.NetworkSelfService.Backend.Services;

[ValidateAntiForgeryToken]
public sealed class GrpcAuthenticationService : Authentication.AuthenticationBase
{
    private readonly ILogger<GrpcAuthenticationService> _logger;
    private readonly LoginHandler _loginHandler;

    public GrpcAuthenticationService(
        ILogger<GrpcAuthenticationService> logger,
        LoginHandler loginHandler)
    {
        this._logger = logger;
        this._loginHandler = loginHandler;
    }

    [AllowAnonymous]
    public override async Task<Result> Authenticate(AuthenticationRequest request, ServerCallContext context)
    {
        this._logger.LogInformation("Authentication attempt from '{username}'", request.Username);
        var result = await this._loginHandler.LoginAsync(request, context.CancellationToken);
        return new Result { IsSuccess = result.Session is not null, Result_ = Any.Pack(result), };
    }

    [Authorize(Policy = TokenPolicies.RefreshOnlyPolicy)]
    public override async Task<Result> RefreshSession(SessionRefreshRequest request, ServerCallContext context)
    {
        this._logger.LogInformation("Session refresh attempt from '{username}'", context.GetHttpContext().User.GetName());
        var result = await this._loginHandler.RefreshTokenAsync(context.GetHttpContext().User.GetSessionId().Value, context.CancellationToken);
        return new Result { IsSuccess = result.Session is not null, Result_ = Any.Pack(result), };
    }

    [Authorize]
    public override async Task<Result> DestroySession(SessionDestroyRequest request, ServerCallContext context)
    {
        this._logger.LogInformation("Logout request from '{username}'", context.GetHttpContext().User.GetName());
        await this._loginHandler.LogoutAsync(context.GetHttpContext().User.GetSessionId().Value, context.CancellationToken);
        return new Result { IsSuccess = true };
    }
}
