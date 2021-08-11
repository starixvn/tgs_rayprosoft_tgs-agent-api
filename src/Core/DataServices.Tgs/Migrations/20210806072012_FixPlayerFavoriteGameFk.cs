using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class FixPlayerFavoriteGameFk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_player_favorite_games_brand_id",
                schema: "dbo",
                table: "player_favorite_games");

            migrationBuilder.DropIndex(
                name: "ix_player_favorite_games_player_code",
                schema: "dbo",
                table: "player_favorite_games");

            migrationBuilder.CreateIndex(
                name: "ix_player_favorite_games_brand_id",
                schema: "dbo",
                table: "player_favorite_games",
                column: "brand_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_player_favorite_games_brand_id",
                schema: "dbo",
                table: "player_favorite_games");

            migrationBuilder.CreateIndex(
                name: "ix_player_favorite_games_brand_id",
                schema: "dbo",
                table: "player_favorite_games",
                column: "brand_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_player_favorite_games_player_code",
                schema: "dbo",
                table: "player_favorite_games",
                column: "player_code",
                unique: true);
        }
    }
}
