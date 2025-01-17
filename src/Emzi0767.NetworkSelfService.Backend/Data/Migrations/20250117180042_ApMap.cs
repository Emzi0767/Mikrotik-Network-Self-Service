using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emzi0767.NetworkSelfService.Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ap_mappings",
                columns: table => new
                {
                    identity = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    comment = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pkey_mapping_identity", x => x.identity);
                });

            migrationBuilder.CreateIndex(
                name: "ix_mapping_identity",
                table: "ap_mappings",
                column: "identity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ap_mappings");
        }
    }
}
