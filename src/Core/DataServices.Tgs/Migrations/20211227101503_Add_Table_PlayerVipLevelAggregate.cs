using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class Add_Table_PlayerVipLevelAggregate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "player_vip_level_aggregates",
                schema: "dbo",
                columns: table => new
                {
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    from_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    to_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    deposit_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    turnover = table.Column<decimal>(type: "numeric", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    update_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_vip_level_aggregates", x => new { x.brand_id, x.player_code, x.from_on, x.to_on });
                });

            migrationBuilder.CreateIndex(
                name: "ix_player_vip_level_aggregates_player_code",
                schema: "dbo",
                table: "player_vip_level_aggregates",
                columns: new[] { "player_code", "from_on", "to_on" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_vip_level_aggregates",
                schema: "dbo");
        }
    }
}
