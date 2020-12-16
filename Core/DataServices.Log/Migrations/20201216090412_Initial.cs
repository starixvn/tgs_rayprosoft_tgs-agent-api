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
                name: "AdminLogs",
                schema: "dbo",
                columns: table => new
                {
                    RecordedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    Method = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(4000)", nullable: true),
                    StackTrace = table.Column<string>(type: "nvarchar(4000)", nullable: true),
                    ServerName = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    RequestHeaders = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    RequestContent = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    ResponseHeaders = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    ResponseContent = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    ResponseCode = table.Column<short>(type: "smallint", nullable: false),
                    ExecutionTimeMs = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminLogs", x => x.RecordedOn);
                });

            migrationBuilder.CreateTable(
                name: "ApiLogs",
                schema: "dbo",
                columns: table => new
                {
                    RecordedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    ServerName = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    RequestHeaders = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    RequestContent = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    ResponseHeaders = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    ResponseContent = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    ResponseCode = table.Column<short>(type: "smallint", nullable: false),
                    ExecutionTimeMs = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiLogs", x => x.RecordedOn);
                });

            migrationBuilder.CreateTable(
                name: "LogRecords",
                schema: "dbo",
                columns: table => new
                {
                    RecordedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServerName = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Logger = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(512)", nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(4000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogRecords", x => x.RecordedOn);
                });

            InitAfterRun(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminLogs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ApiLogs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LogRecords",
                schema: "dbo");
        }
    }
}
