using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddExtraInfoForGameProviderRelatedTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "transfer_wallet",
                schema: "dbo",
                table: "game_providers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "player_code",
                schema: "bet",
                table: "game_actions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "transfer_wallet",
                schema: "dbo",
                table: "game_providers");

            migrationBuilder.DropColumn(
                name: "player_code",
                schema: "bet",
                table: "game_actions");
        }
    }
}
