using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddFailedGameRecordOperationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "failed_game_record_operations",
                schema: "bet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_provider_code = table.Column<string>(type: "text", nullable: true),
                    external_round_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    external_id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    detail = table.Column<string>(type: "text", nullable: false),
                    auto_status = table.Column<short>(type: "smallint", nullable: false),
                    proces_status = table.Column<short>(type: "smallint", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_failed_game_record_operations", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_failed_game_record_operations_game_provider_id_auto_status_proces_status",
                schema: "bet",
                table: "failed_game_record_operations",
                columns: new[] { "game_provider_id", "auto_status", "proces_status" });

            migrationBuilder.CreateIndex(
                name: "ux_failed_game_record_operations_external_round_id_player_code_game_provider_id_game_id",
                schema: "bet",
                table: "failed_game_record_operations",
                columns: new[] { "external_round_id", "player_code", "game_provider_id", "game_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "failed_game_record_operations",
                schema: "bet");
        }
    }
}
