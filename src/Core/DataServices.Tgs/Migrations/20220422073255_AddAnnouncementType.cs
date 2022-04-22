using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddAnnouncementType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_agent_bank_infos_players_player_code",
                schema: "dbo",
                table: "agent_bank_infos");

            migrationBuilder.AddColumn<string>(
                name: "type",
                schema: "dbo",
                table: "announcements",
                type: "text",
                nullable: false,
                defaultValue: "marquee");

            migrationBuilder.AddForeignKey(
                name: "fk_agent_bank_infos_players_player_temp_id",
                schema: "dbo",
                table: "agent_bank_infos",
                column: "player_code",
                principalSchema: "dbo",
                principalTable: "players",
                principalColumn: "player_code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_agent_bank_infos_players_player_temp_id",
                schema: "dbo",
                table: "agent_bank_infos");

            migrationBuilder.DropColumn(
                name: "type",
                schema: "dbo",
                table: "announcements");

            migrationBuilder.AddForeignKey(
                name: "fk_agent_bank_infos_players_player_code",
                schema: "dbo",
                table: "agent_bank_infos",
                column: "player_code",
                principalSchema: "dbo",
                principalTable: "players",
                principalColumn: "player_code");
        }
    }
}
