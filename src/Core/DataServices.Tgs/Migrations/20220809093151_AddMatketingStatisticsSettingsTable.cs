using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddMatketingStatisticsSettingsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "marketing_statistics_settings",
                schema: "dbo",
                columns: table => new
                {
                    record_key = table.Column<string>(type: "text", nullable: false),
                    processed_time = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_marketing_statistics_settings", x => x.record_key);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "marketing_statistics_settings",
                schema: "dbo");
        }
    }
}
