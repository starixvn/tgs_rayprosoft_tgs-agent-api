using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Apis.LogApi.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "ApiLogs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TrackingId = table.Column<Guid>(nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    ServerName = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    RequestHeaders = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    RequestContent = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    ResponseHeaders = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    ResponseContent = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    ResponseCode = table.Column<int>(type: "int", nullable: false),
                    ExecutionTimeMs = table.Column<int>(type: "int", nullable: false),
                    RecordedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventLogs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ServerName = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(512)", nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    RecordedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogRecords",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TrackingId = table.Column<Guid>(nullable: false),
                    RecordedOn = table.Column<DateTimeOffset>(nullable: false),
                    ServerName = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false),
                    Logger = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogRecords", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiLogs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "EventLogs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LogRecords",
                schema: "dbo");
        }
    }
}
