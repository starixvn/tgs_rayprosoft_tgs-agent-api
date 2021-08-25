using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddAmountColumnsForPlayerAggregate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "net_profit",
                schema: "report",
                table: "player_aggregates");

            migrationBuilder.AddColumn<decimal>(
                "rebate_amount",
                schema: "report",
                table: "player_aggregates",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                "dividend_amount",
                schema: "report",
                table: "player_aggregates",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                "promotion_amount",
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
                "dividend_amount",
                schema: "report",
                table: "player_aggregates");

            migrationBuilder.DropColumn(
                "promotion_amount",
                schema: "report",
                table: "player_aggregates");

            migrationBuilder.RenameColumn(
                "rebate_amount",
                schema: "report",
                table: "player_aggregates",
                newName: "net_profit");
        }
    }
}