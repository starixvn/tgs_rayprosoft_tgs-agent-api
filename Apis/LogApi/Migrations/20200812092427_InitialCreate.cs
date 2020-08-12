using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Apis.LogApi.Migrations
{
    public partial class InitialCreate : Migration
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiLogs",
                schema: "dbo");
        }
    }
}
