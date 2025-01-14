using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emzi0767.NetworkSelfService.Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "networks",
                columns: table => new
                {
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    dhcp = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    wifi_iflist = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pkey_network_name", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    network = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pkey_user_name", x => x.username);
                    table.ForeignKey(
                        name: "FK_users_networks_network",
                        column: x => x.network,
                        principalTable: "networks",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    remember = table.Column<bool>(type: "boolean", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pkey_session_id", x => x.id);
                    table.UniqueConstraint("ukey_session_id_username", x => new { x.id, x.username });
                    table.ForeignKey(
                        name: "fkey_session_user",
                        column: x => x.username,
                        principalTable: "users",
                        principalColumn: "username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_network_name",
                table: "networks",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_session_id",
                table: "sessions",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_session_username",
                table: "sessions",
                column: "username");

            migrationBuilder.CreateIndex(
                name: "ix_user_name",
                table: "users",
                column: "username");

            migrationBuilder.CreateIndex(
                name: "ix_user_network",
                table: "users",
                column: "network",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sessions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "networks");
        }
    }
}
