using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddColumnsForAgentDividend : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "birthday_amount",
                schema: "dbo",
                table: "agent_dividends",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "season_amount",
                schema: "dbo",
                table: "agent_dividends",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "upgrade_amount",
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
                name: "birthday_amount",
                schema: "dbo",
                table: "agent_dividends");

            migrationBuilder.DropColumn(
                name: "season_amount",
                schema: "dbo",
                table: "agent_dividends");

            migrationBuilder.DropColumn(
                name: "upgrade_amount",
                schema: "dbo",
                table: "agent_dividends");
        }
    }
}
