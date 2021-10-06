using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddPlayerPhoneUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_players_brand_id",
                schema: "dbo",
                table: "players");

            migrationBuilder.CreateIndex(
                name: "ux_players_brand_phone",
                schema: "dbo",
                table: "players",
                columns: new[] { "brand_id", "area_code", "phone" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_players_brand_phone",
                schema: "dbo",
                table: "players");

            migrationBuilder.CreateIndex(
                name: "ix_players_brand_id",
                schema: "dbo",
                table: "players",
                column: "brand_id");
        }
    }
}
