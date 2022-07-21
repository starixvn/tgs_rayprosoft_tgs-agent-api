using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class Add_FK_GamePlayerTransformIdnlive_Players_PlayerCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "fk_game_player_transform_idnlive_players_player_code",
                schema: "dbo",
                table: "game_player_transform_idnlive",
                column: "player_code",
                principalSchema: "dbo",
                principalTable: "players",
                principalColumn: "player_code",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_game_player_transform_idnlive_players_player_code",
                schema: "dbo",
                table: "game_player_transform_idnlive");
        }
    }
}
