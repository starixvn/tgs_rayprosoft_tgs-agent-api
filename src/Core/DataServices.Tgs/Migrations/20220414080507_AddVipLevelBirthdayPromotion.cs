using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddVipLevelBirthdayPromotion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "player_vip_level_birthday_promotions",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_vip_level_snapshot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    birthday = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_vip_level_birthday_promotions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_player_vip_level_birthday_promotions_brand_id_vip_id_player_code",
                schema: "dbo",
                table: "player_vip_level_birthday_promotions",
                columns: new[] { "brand_id", "vip_id", "player_code" });

            migrationBuilder.CreateIndex(
                name: "ix_player_vip_level_birthday_promotions_player_code_birthday",
                schema: "dbo",
                table: "player_vip_level_birthday_promotions",
                columns: new[] { "player_code", "birthday" });

            migrationBuilder.CreateIndex(
                name: "ix_player_vip_level_birthday_promotions_player_vip_level_snapshot_id",
                schema: "dbo",
                table: "player_vip_level_birthday_promotions",
                column: "player_vip_level_snapshot_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_vip_level_birthday_promotions",
                schema: "dbo");
        }
    }
}
