using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddGameLocaliztion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "localization",
                schema: "dbo",
                table: "games",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "localization",
                schema: "dbo",
                table: "game_providers",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "localization",
                schema: "dbo",
                table: "games");

            migrationBuilder.DropColumn(
                name: "localization",
                schema: "dbo",
                table: "game_providers");
        }
    }
}
