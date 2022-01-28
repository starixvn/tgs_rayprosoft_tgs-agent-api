using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class Add_Round_Index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_rounds_game_provider_configuration_id_composite",
                schema: "bet",
                table: "rounds",
                columns: new[] { "game_provider_configuration_id", "wallet_id", "game_id", "external_round_id" });

            migrationBuilder.CreateIndex(
                name: "ix_rounds_wallet_id_composite",
                schema: "bet",
                table: "rounds",
                columns: new[] { "external_round_id", "game_provider_id", "game_id", "wallet_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_rounds_game_provider_configuration_id_composite",
                schema: "bet",
                table: "rounds");

            migrationBuilder.DropIndex(
                name: "ix_rounds_wallet_id_composite",
                schema: "bet",
                table: "rounds");
        }
    }
}
