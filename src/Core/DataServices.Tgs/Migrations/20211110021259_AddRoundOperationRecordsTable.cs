using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class AddRoundOperationRecordsTable : Migration
{
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "round_operation_records",
            schema: "bet");
    }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "round_operation_records",
            schema: "bet",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                round_id = table.Column<Guid>(type: "uuid", nullable: false),
                operator_id = table.Column<Guid>(type: "uuid", nullable: false),
                old_data = table.Column<string>(type: "jsonb", nullable: false),
                new_data = table.Column<string>(type: "jsonb", nullable: false),
                created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_round_operation_records", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_round_operation_records_round_id",
            schema: "bet",
            table: "round_operation_records",
            column: "round_id");
    }
}
