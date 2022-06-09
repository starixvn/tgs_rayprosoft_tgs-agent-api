using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class UpdateAgentDividendAndPlayerAggregateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "deposit_handling_fee",
                schema: "report",
                table: "player_aggregates",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "withdraw_handling_fee",
                schema: "report",
                table: "player_aggregates",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "deposit_handling_fee",
                schema: "dbo",
                table: "agent_dividends",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "withdraw_handling_fee",
                schema: "dbo",
                table: "agent_dividends",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "deposit_handling_fee",
                schema: "report",
                table: "player_aggregates");

            migrationBuilder.DropColumn(
                name: "withdraw_handling_fee",
                schema: "report",
                table: "player_aggregates");

            migrationBuilder.DropColumn(
                name: "deposit_handling_fee",
                schema: "dbo",
                table: "agent_dividends");

            migrationBuilder.DropColumn(
                name: "withdraw_handling_fee",
                schema: "dbo",
                table: "agent_dividends");
        }
    }
}
