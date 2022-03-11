using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddPlayerIncreaseAmountAndPlayerReduceAmountTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "player_increase_amounts",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_code = table.Column<string>(type: "text", nullable: false),
                    player_increase_amount_type = table.Column<short>(type: "smallint", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    bucket_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    remark = table.Column<string>(type: "text", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_increase_amounts_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "player_reduce_amounts",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_code = table.Column<string>(type: "text", nullable: false),
                    player_reduce_amount_type = table.Column<short>(type: "smallint", nullable: false),
                    transfer_target = table.Column<string>(type: "text", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    remark = table.Column<string>(type: "text", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_reduce_amounts_id", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_player_increase_amount_multiple_keys",
                schema: "dbo",
                table: "player_increase_amounts",
                columns: new[] { "created_on", "brand_id", "player_code", "player_increase_amount_type" });

            migrationBuilder.CreateIndex(
                name: "ix_player_reduce_amount_multiple_keys",
                schema: "dbo",
                table: "player_reduce_amounts",
                columns: new[] { "created_on", "brand_id", "player_code", "player_reduce_amount_type" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_increase_amounts",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "player_reduce_amounts",
                schema: "dbo");
        }
    }
}
