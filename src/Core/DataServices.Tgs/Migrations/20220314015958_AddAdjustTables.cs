using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddAdjustTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "adjust_agent_records",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: true),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    adjustment_type = table.Column<short>(type: "smallint", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    transfer_type = table.Column<short>(type: "smallint", nullable: false),
                    transfer_target = table.Column<string>(type: "text", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    remark = table.Column<string>(type: "text", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_adjust_agent_records_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "adjust_transfer_records",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_from_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_to_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transfer_type = table.Column<short>(type: "smallint", nullable: false),
                    transfer_from_username = table.Column<string>(type: "text", nullable: true),
                    transfer_to_username = table.Column<string>(type: "text", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_adjust_transfer_record_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "player_adjust_records",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: true),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_code = table.Column<string>(type: "text", nullable: false),
                    adjustment_type = table.Column<short>(type: "smallint", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    transfer_type = table.Column<short>(type: "smallint", nullable: false),
                    transfer_target = table.Column<string>(type: "text", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    bucket_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    remark = table.Column<string>(type: "text", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_adjust_record_id", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_adjust_agent_record_multiple_keys",
                schema: "dbo",
                table: "adjust_agent_records",
                columns: new[] { "created_on", "brand_id", "agent_id", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_adjust_transfer_record_multiple_keys",
                schema: "dbo",
                table: "adjust_transfer_records",
                columns: new[] { "created_on", "transfer_type", "transfer_from_username", "transfer_to_username" });

            migrationBuilder.CreateIndex(
                name: "ix_player_adjust_record_multiple_keys",
                schema: "dbo",
                table: "player_adjust_records",
                columns: new[] { "created_on", "brand_id", "player_code", "type" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adjust_agent_records",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "adjust_transfer_records",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "player_adjust_records",
                schema: "dbo");
        }
    }
}
