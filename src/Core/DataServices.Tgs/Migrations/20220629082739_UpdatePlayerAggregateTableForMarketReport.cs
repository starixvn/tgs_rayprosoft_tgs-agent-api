using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class UpdatePlayerAggregateTableForMarketReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "active_player",
                schema: "report",
                table: "player_aggregates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "deposit_count",
                schema: "report",
                table: "player_aggregates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "login_player",
                schema: "report",
                table: "player_aggregates",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "active_player",
                schema: "report",
                table: "player_aggregates");

            migrationBuilder.DropColumn(
                name: "deposit_count",
                schema: "report",
                table: "player_aggregates");

            migrationBuilder.DropColumn(
                name: "login_player",
                schema: "report",
                table: "player_aggregates");
        }
    }
}
