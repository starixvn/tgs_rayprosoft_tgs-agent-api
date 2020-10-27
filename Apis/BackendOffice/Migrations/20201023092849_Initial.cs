using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Apis.BackendOffice.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Departments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentVendors",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentVendors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RoleType = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    MenuSetting = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "dbo",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "PaymentChannels",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    PaymentVendorId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentChannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentChannels_PaymentVendors_PaymentVendorId",
                        column: x => x.PaymentVendorId,
                        principalSchema: "dbo",
                        principalTable: "PaymentVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    RoleId = table.Column<Guid>(nullable: false),
                    RoleType = table.Column<byte>(type: "tinyint", nullable: false),
                    DepartmentId = table.Column<Guid>(nullable: false),
                    ResellerId = table.Column<Guid>(nullable: true),
                    LicenseeId = table.Column<Guid>(nullable: true),
                    Username = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    LastLoginIp = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    LastLoginTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    Password = table.Column<byte[]>(type: "varbinary(128)", nullable: false),
                    Salt = table.Column<byte[]>(type: "varbinary(128)", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "dbo",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentChannels_PaymentVendorId",
                schema: "dbo",
                table: "PaymentChannels",
                column: "PaymentVendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentId",
                schema: "dbo",
                table: "Users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                schema: "dbo",
                table: "Users",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentChannels",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Settings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PaymentVendors",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Departments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "dbo");
        }
    }
}
