﻿// <auto-generated />
using System;
using Emzi0767.NetworkSelfService.Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Emzi0767.NetworkSelfService.Backend.Data.Migrations
{
    [DbContext(typeof(NssDbContext))]
    partial class NssDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Emzi0767.NetworkSelfService.Backend.Data.DbApMap", b =>
                {
                    b.Property<string>("Identity")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("identity");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("comment");

                    b.HasKey("Identity")
                        .HasName("pkey_mapping_identity");

                    b.HasIndex("Identity")
                        .HasDatabaseName("ix_mapping_identity");

                    b.ToTable("ap_mappings", (string)null);
                });

            modelBuilder.Entity("Emzi0767.NetworkSelfService.Backend.Data.DbNetwork", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("name");

                    b.Property<string>("DhcpServer")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("dhcp");

                    b.Property<string>("WirelessInterfaceList")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("wifi_iflist");

                    b.HasKey("Name")
                        .HasName("pkey_network_name");

                    b.HasIndex("Name")
                        .HasDatabaseName("ix_network_name");

                    b.ToTable("networks", (string)null);
                });

            modelBuilder.Entity("Emzi0767.NetworkSelfService.Backend.Data.DbSession", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset>("ExpiresAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expires_at");

                    b.Property<bool>("IsRemembered")
                        .HasColumnType("boolean")
                        .HasColumnName("remember");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pkey_session_id");

                    b.HasAlternateKey("Id", "Username")
                        .HasName("ukey_session_id_username");

                    b.HasIndex("Id")
                        .HasDatabaseName("ix_session_id");

                    b.HasIndex("Username")
                        .HasDatabaseName("ix_session_username");

                    b.ToTable("sessions", (string)null);
                });

            modelBuilder.Entity("Emzi0767.NetworkSelfService.Backend.Data.DbUser", b =>
                {
                    b.Property<string>("Username")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("username");

                    b.Property<string>("NetworkName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("network");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("password");

                    b.HasKey("Username")
                        .HasName("pkey_user_name");

                    b.HasIndex("NetworkName")
                        .IsUnique()
                        .HasDatabaseName("ix_user_network");

                    b.HasIndex("Username")
                        .HasDatabaseName("ix_user_name");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("Emzi0767.NetworkSelfService.Backend.Data.DbSession", b =>
                {
                    b.HasOne("Emzi0767.NetworkSelfService.Backend.Data.DbUser", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("Username")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fkey_session_user");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Emzi0767.NetworkSelfService.Backend.Data.DbUser", b =>
                {
                    b.HasOne("Emzi0767.NetworkSelfService.Backend.Data.DbNetwork", "Network")
                        .WithMany()
                        .HasForeignKey("NetworkName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Network");
                });

            modelBuilder.Entity("Emzi0767.NetworkSelfService.Backend.Data.DbUser", b =>
                {
                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}
