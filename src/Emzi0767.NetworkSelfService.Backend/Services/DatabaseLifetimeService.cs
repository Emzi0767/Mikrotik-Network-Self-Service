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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Emzi0767.NetworkSelfService.Backend.Services;

public sealed class DatabaseLifetimeService : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<DatabaseLifetimeService> _logger;

    public DatabaseLifetimeService(IServiceProvider services, ILoggerFactory loggerFactory)
    {
        this._services = services;
        this._logger = loggerFactory.CreateLogger<DatabaseLifetimeService>();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Migrating database");
        using var scope = this._services.CreateScope();
        var srvs = scope.ServiceProvider;

        using var db = srvs.GetRequiredService<NssDbContext>();
        await db.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
