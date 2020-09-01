using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

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
                    Ip = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    Path = table.Column<string>(type: "NVARCHAR(256)", nullable: false),
                    Method = table.Column<string>(type: "NVARCHAR(256)", nullable: false),
                    Prefix = table.Column<string>(type: "NVARCHAR(256)", nullable: false),
                    ServerName = table.Column<string>(type: "NVARCHAR(30)", nullable: false),
                    RequestHeaders = table.Column<string>(type: "NVARCHAR(4000)", nullable: false),
                    RequestContent = table.Column<string>(type: "NVARCHAR(4000)", nullable: false),
                    ResponseHeaders = table.Column<string>(type: "NVARCHAR(4000)", nullable: false),
                    ResponseContent = table.Column<string>(type: "NVARCHAR(4000)", nullable: false),
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
                    ServerName = table.Column<string>(type: "NVARCHAR(64)", nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Logger = table.Column<string>(nullable: true),
                    Message = table.Column<string>(type: "NVARCHAR(512)", nullable: false),
                    Exception = table.Column<string>(type: "NVARCHAR(4000)", nullable: false)
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
                    MachineName = table.Column<string>(type: "NVARCHAR(512)", nullable: true),
                    Level = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    ServerAddress = table.Column<string>(type: "NVARCHAR(512)", nullable: true),
                    RemoteAddress = table.Column<string>(type: "NVARCHAR(512)", nullable: true),
                    Logger = table.Column<string>(type: "NVARCHAR(512)", nullable: true),
                    Port = table.Column<string>(type: "NVARCHAR(255)", nullable: true),
                    Properties = table.Column<string>(type: "NVARCHAR(512)", nullable: true),
                    Callsite = table.Column<string>(type: "NVARCHAR(512)", nullable: true),
                    Message = table.Column<string>(type: "NVARCHAR(4000)", nullable: true),
                    Exception = table.Column<string>(type: "NVARCHAR(4000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLogs", x => x.Id);
                });

            var assembly = Assembly.GetExecutingAssembly();
            migrationBuilder.SqlResource(assembly, "TGS.Apis.LogApi.DataService.SqlScripts.SP.usp_ServiceLogs_InsertServiceLog.sql");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
