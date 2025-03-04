﻿// This file is part of Network Self-Service Project.
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
using System.Threading;
using System.Threading.Tasks;
using Emzi0767.NetworkSelfService.gRPC;
using Google.Protobuf.WellKnownTypes;

namespace Emzi0767.NetworkSelfService.Backend.Services;

public sealed class LoginHandler
{
    private readonly SessionRepository _sessions;
    private readonly UserRepository _users;
    private readonly PasswordHashProvider _passwordHashProvider;
    private readonly TokenGenerator _tokenGenerator;

    public LoginHandler(
        SessionRepository sessions,
        UserRepository users,
        PasswordHashProvider passwordHashProvider,
        TokenGenerator tokenGenerator)
    {
        this._sessions = sessions;
        this._users = users;
        this._passwordHashProvider = passwordHashProvider;
        this._tokenGenerator = tokenGenerator;
    }

    public async Task<AuthenticationResponse> LoginAsync(AuthenticationRequest login, CancellationToken cancellationToken = default)
    {
        var user = await this._users.FindUserByNameAsync(login.Username, cancellationToken);
        if (user is null)
            return null;

        if (!this._passwordHashProvider.Verify(login.Password, user.PasswordHash))
            return null;

        var session = await this._sessions.CreateSessionAsync(new()
        {
            Username = user.Username,
            IsRemembered = login.RememberMe,
            ExpiresAt = this._tokenGenerator.GetExpirationDate(login.RememberMe),
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        var tokens = this._tokenGenerator.GenerateTokenPair(user, session, issueRefresh: true);
        return new()
        {
            Session = new()
            {
                Token = tokens.Authentication,
                Refresh = tokens.Refresh,
                ExpiresAt = Timestamp.FromDateTimeOffset(session.ExpiresAt),
                RefreshBy = Timestamp.FromDateTimeOffset(tokens.RefreshBy),
            }
        };
    }

    public async Task LogoutAsync(Guid sessionId, CancellationToken cancellationToken = default)
        => await this._sessions.DeleteSessionAsync(sessionId, cancellationToken);

    public async Task<AuthenticationResponse> RefreshTokenAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await this._sessions.GetSessionAsync(sessionId, cancellationToken);
        if (session is null)
            return null;

        var rotateSession = (session.ExpiresAt - DateTimeOffset.UtcNow).TotalMinutes < 1;
        if (rotateSession)
            session = await this._sessions.UpdateSessionAsync(
                sessionId,
                this._tokenGenerator.GetExpirationDate(session.IsRemembered),
                cancellationToken);

        var user = await this._users.FindUserByNameAsync(session.Username, cancellationToken);
        var tokens = this._tokenGenerator.GenerateTokenPair(user, session, rotateSession);

        var result = new AuthenticationResponse()
        {
            Session = new()
            {
                Token = tokens.Authentication,
                ExpiresAt = Timestamp.FromDateTimeOffset(session.ExpiresAt),
                RefreshBy = Timestamp.FromDateTimeOffset(tokens.RefreshBy),
            }
        };

        if (rotateSession)
            result.Session.Refresh = tokens.Refresh;

        return result;
    }

    public async Task<bool> UpdateUserAsync(string username, PasswordUpdateRequest passwordUpdateRequest, CancellationToken cancellationToken = default)
    {
        if (passwordUpdateRequest.NewPassword != passwordUpdateRequest.ConfirmPassword)
            return false;

        var user = await this._users.FindUserByNameAsync(username, cancellationToken);
        if (user is null)
            return false;

        if (!this._passwordHashProvider.Verify(passwordUpdateRequest.CurrentPassword, user.PasswordHash))
            return false;

        var hash = this._passwordHashProvider.Hash(passwordUpdateRequest.NewPassword);
        user.PasswordHash = hash;
        await this._users.UpdateAsync(user, cancellationToken);
        return true;
    }
}
