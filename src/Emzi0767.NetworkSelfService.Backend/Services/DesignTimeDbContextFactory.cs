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
using Emzi0767.NetworkSelfService.Backend.Configuration;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Emzi0767.NetworkSelfService.Backend.Services;

/// <summary>
/// Provides database context instances at design time.
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<NssDbContext>
{
    /// <summary>
    /// Creates a database context for use by the designer.
    /// </summary>
    /// <param name="args">Arguments to the program.</param>
    /// <returns>Instantiated context.</returns>
    public NssDbContext CreateDbContext(string[] args)
    {
        Environment.SetEnvironmentVariable("NSS2__CONFIGURATION__JSON", "config.dev.json");

        var pgConfig = new PostgresConfiguration();
        new ConfigurationBuilder()
            .Build()
            .SetupConfiguration()
            .GetRequiredSection("Postgres")
            .Bind(pgConfig);

        return new(new(pgConfig));
    }
}
