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
using System.Threading.Tasks;
using Emzi0767.NetworkSelfService.gRPC;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Emzi0767.NetworkSelfService.Backend.Services;

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

    private Guid GetSessionId(ServerCallContext context)
        => context.GetHttpContext().User.GetSessionId().Value;

    [AllowAnonymous]
    public override async Task<Result> Authenticate(AuthenticationRequest request, ServerCallContext context)
    {
        this._logger.LogInformation("Authentication attempt from '{username}'", request.Username);
        var response = await this._loginHandler.LoginAsync(request, context.CancellationToken);
        var result = new Result
        {
            IsSuccess = response is not null,
        };

        if (response is not null)
            result.Result_ = Any.Pack(response);
        else
            result.Error = new Error
            {
                Code = ErrorCode.InvalidCredentials,
            };

        return result;
    }

    [Authorize(TokenPolicies.RefreshOnlyPolicy)]
    public override async Task<Result> RefreshSession(Empty request, ServerCallContext context)
    {
        this._logger.LogInformation("Session refresh attempt from '{username}'", context.GetHttpContext().User.GetName());
        var response = await this._loginHandler.RefreshTokenAsync(this.GetSessionId(context), context.CancellationToken);
        var result = new Result
        {
            IsSuccess = response is not null,
        };

        if (response is not null)
            result.Result_ = Any.Pack(response);
        else
            result.Error = new Error
            {
                Code = ErrorCode.SessionInvalidOrExpired,
            };

        return result;
    }

    [Authorize]
    public override async Task<Result> DestroySession(Empty request, ServerCallContext context)
    {
        this._logger.LogInformation("Logout request from '{username}'", context.GetHttpContext().User.GetName());
        await this._loginHandler.LogoutAsync(this.GetSessionId(context), context.CancellationToken);
        return new Result { IsSuccess = true };
    }

    [Authorize]
    public override async Task<Result> ChangePassword(PasswordUpdateRequest request, ServerCallContext context)
    {
        this._logger.LogInformation("Changing password for '{username}'", context.GetHttpContext().User.GetName());
        var result = await this._loginHandler.UpdateUserAsync(context.GetHttpContext().User.GetName(), request, context.CancellationToken);
        return result
            ? new Result { IsSuccess = true }
            : new Result { IsSuccess = false, Error = new Error { Code = ErrorCode.InvalidCredentials } };
    }
}
