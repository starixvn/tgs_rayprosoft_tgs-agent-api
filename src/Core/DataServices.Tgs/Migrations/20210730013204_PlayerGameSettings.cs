using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class PlayerGameSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "player_game_settings",
                schema: "dbo",
                columns: table => new
                {
                    player_code = table.Column<string>(type: "character varying(16)", nullable: false),
                    is_auto_transfer_to_game = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_game_settings", x => x.player_code);
                    table.ForeignKey(
                        name: "fk_player_game_settings_players",
                        column: x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_game_settings",
                schema: "dbo");
        }
    }
}
