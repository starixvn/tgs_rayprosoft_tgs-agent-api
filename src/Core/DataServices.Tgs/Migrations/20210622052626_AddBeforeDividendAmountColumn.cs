using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddBeforeDividendAmountColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                "agent_dividend_id",
                schema: "dbo",
                table: "agent_dividends",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                "before_dividend_amount",
                schema: "dbo",
                table: "agent_dividends",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "agent_dividend_id",
                schema: "dbo",
                table: "agent_dividends");

            migrationBuilder.DropColumn(
                "before_dividend_amount",
                schema: "dbo",
                table: "agent_dividends");
        }
    }
}