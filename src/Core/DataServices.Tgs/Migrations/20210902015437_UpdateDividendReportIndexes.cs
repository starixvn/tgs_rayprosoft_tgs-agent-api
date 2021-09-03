using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class UpdateDividendReportIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_player_game_rebate_aggregates_brand_id_report_date",
                schema: "report",
                table: "player_game_rebate_aggregates");

            migrationBuilder.DropIndex(
                name: "ix_player_game_rebate_aggregates_brand_id_report_date_agent_id",
                schema: "report",
                table: "player_game_rebate_aggregates");

            migrationBuilder.DropIndex(
                name: "ix_player_game_rebate_aggregates_brand_id_report_date_player_code",
                schema: "report",
                table: "player_game_rebate_aggregates");

            migrationBuilder.DropIndex(
                name: "ix_player_aggregates_brand_id_report_date",
                schema: "report",
                table: "player_aggregates");

            migrationBuilder.DropIndex(
                name: "ix_player_aggregates_brand_id_report_date_agent_id",
                schema: "report",
                table: "player_aggregates");

            migrationBuilder.DropIndex(
                name: "ix_player_aggregates_brand_id_report_date_player_code",
                schema: "report",
                table: "player_aggregates");

            migrationBuilder.DropIndex(
                name: "ix_agent_dividends_brand_id",
                schema: "dbo",
                table: "agent_dividends");

            migrationBuilder.DropIndex(
                name: "ix_agent_dividends_brand_id_created_on",
                schema: "dbo",
                table: "agent_dividends");

            migrationBuilder.DropIndex(
                name: "ix_agent_dividends_brand_id_created_on_agent_id",
                schema: "dbo",
                table: "agent_dividends");

            migrationBuilder.CreateIndex(
                name: "ix_player_game_rebate_aggregates_multiple_keys",
                schema: "report",
                table: "player_game_rebate_aggregates",
                columns: new[] { "report_date", "brand_id", "agent_id", "player_code" });

            migrationBuilder.CreateIndex(
                name: "ix_player_aggregates_agent_id",
                schema: "report",
                table: "player_aggregates",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_aggregates_multiple_keys",
                schema: "report",
                table: "player_aggregates",
                columns: new[] { "report_date", "brand_id", "agent_id", "player_code" });

            migrationBuilder.CreateIndex(
                name: "ix_agent_dividends_agent_id_brand_id_created_on",
                schema: "dbo",
                table: "agent_dividends",
                columns: new[] { "agent_id", "brand_id", "created_on" });

            migrationBuilder.CreateIndex(
                name: "ix_agent_dividends_agent_id_sent_on",
                schema: "dbo",
                table: "agent_dividends",
                columns: new[] { "agent_id", "sent_on" });

            migrationBuilder.CreateIndex(
                name: "ix_agent_dividends_brand_id_from_to",
                schema: "dbo",
                table: "agent_dividends",
                columns: new[] { "brand_id", "from", "to" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_player_game_rebate_aggregates_multiple_keys",
                schema: "report",
                table: "player_game_rebate_aggregates");

            migrationBuilder.DropIndex(
                name: "ix_player_aggregates_agent_id",
                schema: "report",
                table: "player_aggregates");

            migrationBuilder.DropIndex(
                name: "ix_player_aggregates_multiple_keys",
                schema: "report",
                table: "player_aggregates");

            migrationBuilder.DropIndex(
                name: "ix_agent_dividends_agent_id_brand_id_created_on",
                schema: "dbo",
                table: "agent_dividends");

            migrationBuilder.DropIndex(
                name: "ix_agent_dividends_agent_id_sent_on",
                schema: "dbo",
                table: "agent_dividends");

            migrationBuilder.DropIndex(
                name: "ix_agent_dividends_brand_id_from_to",
                schema: "dbo",
                table: "agent_dividends");

            migrationBuilder.CreateIndex(
                name: "ix_player_game_rebate_aggregates_brand_id_report_date",
                schema: "report",
                table: "player_game_rebate_aggregates",
                columns: new[] { "brand_id", "report_date" });

            migrationBuilder.CreateIndex(
                name: "ix_player_game_rebate_aggregates_brand_id_report_date_agent_id",
                schema: "report",
                table: "player_game_rebate_aggregates",
                columns: new[] { "brand_id", "report_date", "agent_id" });

            migrationBuilder.CreateIndex(
                name: "ix_player_game_rebate_aggregates_brand_id_report_date_player_code",
                schema: "report",
                table: "player_game_rebate_aggregates",
                columns: new[] { "brand_id", "report_date", "player_code" });

            migrationBuilder.CreateIndex(
                name: "ix_player_aggregates_brand_id_report_date",
                schema: "report",
                table: "player_aggregates",
                columns: new[] { "brand_id", "report_date" });

            migrationBuilder.CreateIndex(
                name: "ix_player_aggregates_brand_id_report_date_agent_id",
                schema: "report",
                table: "player_aggregates",
                columns: new[] { "brand_id", "report_date", "agent_id" });

            migrationBuilder.CreateIndex(
                name: "ix_player_aggregates_brand_id_report_date_player_code",
                schema: "report",
                table: "player_aggregates",
                columns: new[] { "brand_id", "report_date", "player_code" });

            migrationBuilder.CreateIndex(
                name: "ix_agent_dividends_brand_id",
                schema: "dbo",
                table: "agent_dividends",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_agent_dividends_brand_id_created_on",
                schema: "dbo",
                table: "agent_dividends",
                columns: new[] { "brand_id", "created_on" });

            migrationBuilder.CreateIndex(
                name: "ix_agent_dividends_brand_id_created_on_agent_id",
                schema: "dbo",
                table: "agent_dividends",
                columns: new[] { "brand_id", "created_on", "agent_id" });
        }
    }
}
