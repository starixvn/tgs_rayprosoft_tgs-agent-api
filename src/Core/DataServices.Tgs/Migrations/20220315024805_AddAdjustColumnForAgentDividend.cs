using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddAdjustColumnForAgentDividend : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "adjustment_amount",
                schema: "dbo",
                table: "agent_dividends",
                newName: "company_adjust_amount");

            migrationBuilder.AddColumn<decimal>(
                name: "agent_adjust_amount",
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
                name: "agent_adjust_amount",
                schema: "dbo",
                table: "agent_dividends");

            migrationBuilder.RenameColumn(
                name: "company_adjust_amount",
                schema: "dbo",
                table: "agent_dividends",
                newName: "adjustment_amount");
        }
    }
}
