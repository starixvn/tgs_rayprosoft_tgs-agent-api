using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class Add_Table_PlayerVipLevelAdjustRecord : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "player_vip_level_adjust_records",
            schema: "dbo",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                old_vip_id = table.Column<Guid>(type: "uuid", nullable: false),
                new_vip_id = table.Column<Guid>(type: "uuid", nullable: false),
                operator_id = table.Column<Guid>(type: "uuid", nullable: false),
                remark = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_player_vip_level_adjust_records", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_player_vip_level_adjust_records_created_on",
            schema: "dbo",
            table: "player_vip_level_adjust_records",
            column: "created_on");

        migrationBuilder.CreateIndex(
            name: "ix_player_vip_level_adjust_records_player_code",
            schema: "dbo",
            table: "player_vip_level_adjust_records",
            column: "player_code");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "player_vip_level_adjust_records",
            schema: "dbo");
    }
}
