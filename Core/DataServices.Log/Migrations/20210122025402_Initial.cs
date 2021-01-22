using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Log.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                "dbo");

            migrationBuilder.CreateTable(
                "admin_logs",
                schema: "dbo",
                columns: table => new
                {
                    recorded_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    tracking_id = table.Column<Guid>("uuid", nullable: false),
                    id = table.Column<Guid>("uuid", nullable: false),
                    ip = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                    path = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    method = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                    exception = table.Column<string>("text", nullable: true),
                    stack_trace = table.Column<string>("text", nullable: true),
                    server_name = table.Column<string>("character varying(128)", maxLength: 128, nullable: false),
                    request_headers = table.Column<string>("text", nullable: false),
                    request_content = table.Column<string>("text", nullable: false),
                    response_headers = table.Column<string>("text", nullable: false),
                    response_content = table.Column<string>("text", nullable: false),
                    response_code = table.Column<short>("smallint", nullable: false),
                    execution_time_ms = table.Column<int>("integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_admin_logs", x => x.recorded_on);
                });

            migrationBuilder.CreateTable(
                "api_logs",
                schema: "dbo",
                columns: table => new
                {
                    recorded_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    id = table.Column<Guid>("uuid", nullable: false),
                    tracking_id = table.Column<Guid>("uuid", nullable: false),
                    ip = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                    path = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                    method = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                    prefix = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                    server_name = table.Column<string>("character varying(128)", maxLength: 128, nullable: false),
                    request_headers = table.Column<string>("text", nullable: false),
                    request_content = table.Column<string>("text", nullable: false),
                    response_headers = table.Column<string>("text", nullable: false),
                    response_content = table.Column<string>("text", nullable: false),
                    response_code = table.Column<short>("smallint", nullable: false),
                    execution_time_ms = table.Column<int>("integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_api_logs", x => x.recorded_on);
                });

            migrationBuilder.CreateTable(
                "log_records",
                schema: "dbo",
                columns: table => new
                {
                    recorded_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    id = table.Column<Guid>("uuid", nullable: false),
                    tracking_id = table.Column<Guid>("uuid", nullable: false),
                    server_name = table.Column<string>("character varying(128)", maxLength: 128, nullable: false),
                    level = table.Column<int>("integer", nullable: false),
                    logger = table.Column<string>("text", nullable: true),
                    message = table.Column<string>("character varying(512)", maxLength: 512, nullable: false),
                    exception = table.Column<string>("text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_log_records", x => x.recorded_on);
                });

            InitAfterRun(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "admin_logs",
                "dbo");

            migrationBuilder.DropTable(
                "api_logs",
                "dbo");

            migrationBuilder.DropTable(
                "log_records",
                "dbo");
        }
    }
}