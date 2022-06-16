using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class AddOriginalTurnoverColumnForAgentDividends : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "turnover",
            schema: "dbo",
            table: "player_bucket_histories",
            type: "numeric(18,6)",
            precision: 18,
            scale: 6,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric");

        migrationBuilder.AddColumn<decimal>(
            name: "original_turnover",
            schema: "dbo",
            table: "agent_dividends",
            type: "numeric(18,6)",
            precision: 18,
            scale: 6,
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<decimal>(
            name: "valid_bet_amount",
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
            name: "original_turnover",
            schema: "dbo",
            table: "agent_dividends");

        migrationBuilder.DropColumn(
            name: "valid_bet_amount",
            schema: "dbo",
            table: "agent_dividends");

        migrationBuilder.AlterColumn<decimal>(
            name: "turnover",
            schema: "dbo",
            table: "player_bucket_histories",
            type: "numeric",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric(18,6)",
            oldPrecision: 18,
            oldScale: 6);
    }
}
