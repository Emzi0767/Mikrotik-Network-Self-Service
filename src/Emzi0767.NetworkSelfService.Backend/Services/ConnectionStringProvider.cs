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

using Emzi0767.NetworkSelfService.Backend.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Emzi0767.NetworkSelfService.Backend.Services;

/// <summary>
/// Handles converting PostgreSQL configuration into connection strings.
/// </summary>
public sealed class ConnectionStringProvider
{
    /// <summary>
    /// Gets the connection string constructed by this provider.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// Gets the data source constructed by this provider.
    /// </summary>
    public NpgsqlDataSource DataSource { get; }

    private readonly PostgresConfiguration _config;

    /// <summary>
    /// Creates a new connection string provider.
    /// </summary>
    /// <param name="opts">Options to build a string from.</param>
    public ConnectionStringProvider(IOptions<PostgresConfiguration> opts)
        : this(opts.Value)
    { }

    internal ConnectionStringProvider(PostgresConfiguration cfg)
    {
        this._config = cfg;

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = this._config.Hostname,
            Port = this._config.Port,
            Username = this._config.Username,
            Password = this._config.Password,
            Database = this._config.Database,

            SslMode = this._config.EnableTls
                ? SslMode.VerifyFull
                : SslMode.Disable,

            RootCertificate = this._config.EnableTls
                ? this._config.CaCertificate
                : null,
        };

        if (this._config.MutualTls)
        {
            builder.SslCertificate = this._config.ClientCertificate;
            builder.SslKey = this._config.ClientKey ?? this._config.ClientCertificate;
            builder.SslPassword = Extensions.UTF8.GetString(this._config.ClientKeyPassword);
        }

        this.ConnectionString = builder.ConnectionString;

        var dsb = new NpgsqlDataSourceBuilder(this.ConnectionString);
        this.DataSource = dsb.Build();
    }
}
