using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class PlayerBonusAggregateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "player_bonus_aggregates",
                schema: "report",
                columns: table => new
                {
                    player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    hour = table.Column<int>(type: "integer", nullable: false),
                    minute = table.Column<int>(type: "integer", nullable: false),
                    report_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    rebate_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    promotion_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    upgrade_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    season_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    birthday_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_bonus_aggregates_id", x => new { x.player_code, x.hour, x.minute, x.report_date });
                });

            migrationBuilder.CreateIndex(
                name: "ix_player_bonus_aggregates_multiple_keys",
                schema: "report",
                table: "player_bonus_aggregates",
                columns: new[] { "report_date", "brand_id", "agent_id", "player_code" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_bonus_aggregates",
                schema: "report");
        }
    }
}
