using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class FixPlayerDailyGamingStatisticsPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_player_daily_gaming_statistics",
                schema: "dbo",
                table: "player_daily_gaming_statistics");

            migrationBuilder.AddPrimaryKey(
                name: "pk_player_daily_gaming_statistics",
                schema: "dbo",
                table: "player_daily_gaming_statistics",
                columns: new[] { "report_date", "player_code", "gp_code", "sub_gp_code" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_player_daily_gaming_statistics",
                schema: "dbo",
                table: "player_daily_gaming_statistics");

            migrationBuilder.AddPrimaryKey(
                name: "pk_player_daily_gaming_statistics",
                schema: "dbo",
                table: "player_daily_gaming_statistics",
                columns: new[] { "report_date", "player_code" });
        }
    }
}
