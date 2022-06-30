using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddAgentDailyPaymentStatisticsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "agent_daily_payment_statistics",
                schema: "dbo",
                columns: table => new
                {
                    agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    report_date = table.Column<DateOnly>(type: "date", nullable: false),
                    first_deposit_players = table.Column<int>(type: "integer", nullable: false),
                    undeposit_again_players = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agent_daily_payment_statistics", x => new { x.report_date, x.brand_id, x.agent_id });
                });

            migrationBuilder.CreateTable(
                name: "background_payment_statistics_settings",
                schema: "dbo",
                columns: table => new
                {
                    record_key = table.Column<string>(type: "text", nullable: false),
                    processed_time = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_background_payment_statistics_settings", x => x.record_key);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agent_daily_payment_statistics",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "background_payment_statistics_settings",
                schema: "dbo");
        }
    }
}
