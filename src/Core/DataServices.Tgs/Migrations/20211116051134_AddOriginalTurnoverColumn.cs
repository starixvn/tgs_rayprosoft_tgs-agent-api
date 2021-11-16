using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddOriginalTurnoverColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "original_turnover",
                schema: "report",
                table: "round_aggregates",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "original_turnover",
                schema: "report",
                table: "player_game_rebate_aggregates",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "original_turnover",
                schema: "report",
                table: "player_aggregates",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "original_turnover",
                schema: "report",
                table: "round_aggregates");

            migrationBuilder.DropColumn(
                name: "original_turnover",
                schema: "report",
                table: "player_game_rebate_aggregates");

            migrationBuilder.DropColumn(
                name: "original_turnover",
                schema: "report",
                table: "player_aggregates");
        }
    }
}
