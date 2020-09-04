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
                name: "AdminLogs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Ip = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    Method = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    ServerName = table.Column<string>(nullable: true),
                    RequestHeaders = table.Column<string>(nullable: true),
                    RequestContent = table.Column<string>(nullable: true),
                    ResponseHeaders = table.Column<string>(nullable: true),
                    ResponseContent = table.Column<string>(nullable: true),
                    ResponseCode = table.Column<int>(nullable: false),
                    ExecutionTimeMs = table.Column<int>(nullable: false),
                    RecordedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminLogs", x => x.Id);
                });

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
                    ResponseCode = table.Column<short>(type: "smallint", nullable: false),
                    ExecutionTimeMs = table.Column<int>(type: "int", nullable: false),
                    RecordedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogRecords",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TrackingId = table.Column<Guid>(nullable: false),
                    RecordedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ServerName = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Logger = table.Column<string>(nullable: true),
                    Message = table.Column<string>(type: "nvarchar(512)", nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(4000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceLogs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecordedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    MachineName = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    ServerAddress = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    RemoteAddress = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    Logger = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    Port = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    Callsite = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(4000)", nullable: true),
                    Exception = table.Column<string>(type: "nvarchar(4000)", nullable: true),
                    ServerName = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    Https = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLogs", x => x.Id);
                });
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

            migrationBuilder.DropTable(
                name: "ServiceLogs",
                schema: "dbo");
        }
    }
}
