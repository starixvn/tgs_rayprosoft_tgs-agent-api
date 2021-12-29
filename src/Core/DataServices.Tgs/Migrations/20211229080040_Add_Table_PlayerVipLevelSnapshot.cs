using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class Add_Table_PlayerVipLevelSnapshot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "player_vip_level_snapshots",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    snapshot_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    data = table.Column<string>(type: "jsonb", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_vip_level_snapshots", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pk_player_vip_level_snapshots_player_code",
                schema: "dbo",
                table: "player_vip_level_snapshots",
                column: "brand_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_vip_level_snapshots",
                schema: "dbo");
        }
    }
}
