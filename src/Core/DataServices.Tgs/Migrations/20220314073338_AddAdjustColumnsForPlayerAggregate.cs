using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class AddAdjustColumnsForPlayerAggregate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "adjustment_amount",
            schema: "report",
            table: "player_aggregates",
            newName: "company_adjust_amount");

        migrationBuilder.AddColumn<decimal>(
            name: "agent_adjust_amount",
            schema: "report",
            table: "player_aggregates",
            type: "numeric(18,6)",
            precision: 18,
            scale: 6,
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<decimal>(
            name: "system_adjust_amount",
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
            name: "agent_adjust_amount",
            schema: "report",
            table: "player_aggregates");

        migrationBuilder.DropColumn(
            name: "system_adjust_amount",
            schema: "report",
            table: "player_aggregates");

        migrationBuilder.RenameColumn(
            name: "company_adjust_amount",
            schema: "report",
            table: "player_aggregates",
            newName: "adjustment_amount");
    }
}
