using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddNoIncludeNetProfitColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "no_include_net_profit_agent_adjust_amount",
                schema: "report",
                table: "player_aggregates",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "no_include_net_profit_company_adjust_amount",
                schema: "report",
                table: "player_aggregates",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "no_include_net_profit_agent_adjust_amount",
                schema: "dbo",
                table: "agent_dividends",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "no_include_net_profit_company_adjust_amount",
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
                name: "no_include_net_profit_agent_adjust_amount",
                schema: "report",
                table: "player_aggregates");

            migrationBuilder.DropColumn(
                name: "no_include_net_profit_company_adjust_amount",
                schema: "report",
                table: "player_aggregates");

            migrationBuilder.DropColumn(
                name: "no_include_net_profit_agent_adjust_amount",
                schema: "dbo",
                table: "agent_dividends");

            migrationBuilder.DropColumn(
                name: "no_include_net_profit_company_adjust_amount",
                schema: "dbo",
                table: "agent_dividends");
        }
    }
}
