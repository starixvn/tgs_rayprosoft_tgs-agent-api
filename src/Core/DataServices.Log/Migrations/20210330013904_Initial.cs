using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Log.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "admin_logs",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tracking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    path = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    method = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    exception = table.Column<string>(type: "text", nullable: true),
                    stack_trace = table.Column<string>(type: "text", nullable: true),
                    server_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    request_headers = table.Column<string>(type: "text", nullable: false),
                    request_content = table.Column<string>(type: "text", nullable: false),
                    response_headers = table.Column<string>(type: "text", nullable: false),
                    response_content = table.Column<string>(type: "text", nullable: false),
                    response_code = table.Column<short>(type: "smallint", nullable: false),
                    execution_time_ms = table.Column<int>(type: "integer", nullable: false),
                    recorded_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_admin_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "api_logs",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tracking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    path = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    method = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    prefix = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    server_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    request_headers = table.Column<string>(type: "text", nullable: false),
                    request_content = table.Column<string>(type: "text", nullable: false),
                    response_headers = table.Column<string>(type: "text", nullable: false),
                    response_content = table.Column<string>(type: "text", nullable: false),
                    response_code = table.Column<short>(type: "smallint", nullable: false),
                    execution_time_ms = table.Column<int>(type: "integer", nullable: false),
                    recorded_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_api_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "log_records",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tracking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recorded_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    server_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    logger = table.Column<string>(type: "text", nullable: true),
                    message = table.Column<string>(type: "text", nullable: false),
                    exception = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_log_records", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_admin_logs_recorded_on",
                schema: "dbo",
                table: "admin_logs",
                column: "recorded_on");

            migrationBuilder.CreateIndex(
                name: "ix_api_logs_recorded_on",
                schema: "dbo",
                table: "api_logs",
                column: "recorded_on");

            migrationBuilder.CreateIndex(
                name: "ix_log_records_recorded_on",
                schema: "dbo",
                table: "log_records",
                column: "recorded_on");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_logs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "api_logs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "log_records",
                schema: "dbo");
        }
    }
}
