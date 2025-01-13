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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emzi0767.NetworkSelfService.Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Emzi0767.NetworkSelfService.Backend.Services;

public sealed class SessionRepository
{
    private readonly NssDbContext _db;

    public SessionRepository(NssDbContext db)
    {
        this._db = db;
    }

    public async Task<DbSession> GetSessionAsync(Guid id, CancellationToken cancellationToken = default)
        => await this._db.Sessions
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<DbSession> CreateSessionAsync(DbSession session, CancellationToken cancellationToken = default)
    {
        session.Id = Guid.NewGuid();
        await this._db.Sessions.AddAsync(session, cancellationToken);
        await this._db.SaveChangesAsync(cancellationToken);

        return session;
    }

    public async Task<DbSession> UpdateSessionAsync(Guid id, DateTimeOffset expiresAt, CancellationToken cancellationToken = default)
    {
        var dbSession = await this._db.Sessions
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        dbSession.ExpiresAt = expiresAt;
        this._db.Sessions.Update(dbSession);
        await this._db.SaveChangesAsync(cancellationToken);

        return dbSession;
    }

    public async Task DeleteSessionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await this._db.Sessions
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        await this._db.SaveChangesAsync(cancellationToken);
    }
}
