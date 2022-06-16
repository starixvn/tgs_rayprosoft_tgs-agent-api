using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class RemoveRoundOperationRecordForeignKey : Migration
{
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddForeignKey(
            name: "fk_round_operation_records_round_id",
            schema: "bet",
            table: "round_operation_records",
            column: "round_id",
            principalSchema: "bet",
            principalTable: "rounds",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var sql =
            "alter table bet.round_operation_records " +
            "drop constraint if exists fk_round_operation_records_round_id;";

        migrationBuilder.Sql(sql);
    }
}
