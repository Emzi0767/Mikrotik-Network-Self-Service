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
using Emzi0767.NetworkSelfService.Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Emzi0767.NetworkSelfService.Backend.Services;

public sealed class NssDbContext : DbContext
{
    public DbSet<DbUser> Users { get; set; }
    public DbSet<DbSession> Sessions { get; set; }
    public DbSet<DbNetwork> Networks { get; set; }

    private readonly ILoggerFactory _loggerFactory;
    private readonly bool _logSensitive;
    private readonly string _connectionString;
    private readonly NpgsqlDataSource _dataSource;

    // internal with csp
    internal NssDbContext(ConnectionStringProvider connectionStringProvider)
        : this(null, null, connectionStringProvider)
    { }

    /// <summary>
    /// Initializes the database context.
    /// </summary>
    /// <param name="loggerFactory">Logger factory to use for logging.</param>
    /// <param name="appConfig">Application configuration.</param>
    /// <param name="connectionStringProvider">Connection string provider, configured with appropriate settings.</param>
    public NssDbContext(
        ILoggerFactory loggerFactory,
        IOptions<ApplicationConfiguration> appConfig,
        ConnectionStringProvider connectionStringProvider
    )
    {
        this._loggerFactory = loggerFactory;
        this._logSensitive = appConfig.Value?.LogSensitive ?? false;
        this._connectionString = connectionStringProvider.ConnectionString;
        this._dataSource = connectionStringProvider.DataSource;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (this._logSensitive && this._loggerFactory is not null)
        {
            optionsBuilder = optionsBuilder.UseLoggerFactory(this._loggerFactory)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }

        optionsBuilder.UseNpgsql(
            this._dataSource,
            options => options.MigrationsAssembly(this.GetType().Assembly.FullName)
                .MigrationsHistoryTable("sys_efcore_migrations")
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DbUser>(e =>
        {
            e.ToTable("users")
                .Ignore(m => m.Network)
                .Ignore(m => m.Sessions);

            e.Property(m => m.Username)
                .IsRequired()
                .HasMaxLength(256)
                .ValueGeneratedNever()
                .HasColumnName("username");

            e.Property(m => m.PasswordHash)
                .IsRequired()
                .HasMaxLength(256)
                .ValueGeneratedNever()
                .HasColumnName("password");

            e.Property(m => m.NetworkName)
                .IsRequired()
                .HasMaxLength(256)
                .ValueGeneratedNever()
                .HasColumnName("network");

            e.HasKey(m => m.Username)
                .HasName("pkey_user_name");

            e.HasIndex(m => m.Username)
                .HasDatabaseName("ix_user_name");

            e.HasIndex(m => m.NetworkName)
                .HasDatabaseName("ix_user_network");

            e.HasOne(m => m.Network)
                .WithOne(m => m.User)
                .HasForeignKey<DbUser>(m => m.NetworkName)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fkey_user_network");
        });

        modelBuilder.Entity<DbSession>(e =>
        {
            e.ToTable("sessions")
                .Ignore(m => m.User);

            e.Property(m => m.Id)
                .IsRequired()
                .ValueGeneratedNever()
                .HasColumnName("id");

            e.Property(m => m.Username)
                .IsRequired()
                .HasMaxLength(256)
                .ValueGeneratedNever()
                .HasColumnName("username");

            e.Property(m => m.IsRemembered)
                .IsRequired()
                .ValueGeneratedNever()
                .HasColumnName("remember");

            e.Property(m => m.ExpiresAt)
                .IsRequired()
                .ValueGeneratedNever()
                .HasColumnName("expires_at");

            e.Property(m => m.CreatedAt)
                .IsRequired()
                .ValueGeneratedNever()
                .HasColumnName("created_at");

            e.HasKey(m => m.Id)
                .HasName("pkey_session_id");

            e.HasAlternateKey(m => new { m.Id, m.Username })
                .HasName("ukey_session_id_username");

            e.HasIndex(m => m.Id)
                .HasDatabaseName("ix_session_id");

            e.HasIndex(m => m.Username)
                .HasDatabaseName("ix_session_username");

            e.HasOne(m => m.User)
                .WithMany(m => m.Sessions)
                .HasForeignKey(m => m.Username)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fkey_session_user");
        });

        modelBuilder.Entity<DbNetwork>(e =>
        {
            e.ToTable("networks")
                .Ignore(m => m.User);

            e.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(256)
                .ValueGeneratedNever()
                .HasColumnName("name");

            e.Property(m => m.DhcpServer)
                .IsRequired()
                .HasMaxLength(256)
                .ValueGeneratedNever()
                .HasColumnName("dhcp");

            e.Property(m => m.WirelessInterfaceList)
                .IsRequired()
                .HasMaxLength(256)
                .ValueGeneratedNever()
                .HasColumnName("wifi_iflist");

            e.HasKey(m => m.Name)
                .HasName("pkey_network_name");

            e.HasIndex(m => m.Name)
                .HasDatabaseName("ix_network_name");
        });
    }
}
