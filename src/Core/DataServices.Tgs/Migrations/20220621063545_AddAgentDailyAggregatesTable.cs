using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddAgentDailyAggregatesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "agent_daily_aggregates",
                schema: "report",
                columns: table => new
                {
                    agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    report_date = table.Column<DateOnly>(type: "date", nullable: false),
                    first_deposit_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    gaming_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    register_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agent_daily_aggregate_report_date_agent_id", x => new { x.report_date, x.agent_id });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agent_daily_aggregates",
                schema: "report");
        }
    }
}
