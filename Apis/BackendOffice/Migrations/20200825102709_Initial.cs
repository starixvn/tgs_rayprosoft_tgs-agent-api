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
                name: "Menus",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Icon = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    Translation = table.Column<string>(type: "NVARCHAR(256)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "NVARCHAR(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    Code = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "NVARCHAR(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RoleType = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    DisplayName = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(128)", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "NVARCHAR(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleMenus",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false),
                    MenuId = table.Column<Guid>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    IsDisplay = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMenus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleMenus_Menus_MenuId",
                        column: x => x.MenuId,
                        principalSchema: "dbo",
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleMenus_RoleMenus_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "dbo",
                        principalTable: "RoleMenus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleMenus_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Roles",
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
                    ResellerId = table.Column<Guid>(nullable: true),
                    LicenseeId = table.Column<Guid>(nullable: true),
                    BrandId = table.Column<Guid>(nullable: true),
                    Username = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    Firstname = table.Column<string>(type: "NVARCHAR(30)", nullable: false),
                    Lastname = table.Column<string>(type: "NVARCHAR(30)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR(128)", nullable: false),
                    TimeZone = table.Column<string>(type: "NVARCHAR(256)", nullable: false),
                    Password = table.Column<byte[]>(type: "VARBINARY(128)", nullable: false),
                    Salt = table.Column<byte[]>(type: "VARBINARY(128)", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleMenuPermissions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RoleMenuId = table.Column<Guid>(nullable: false),
                    PermissionId = table.Column<Guid>(nullable: false),
                    IsDisplay = table.Column<bool>(nullable: false),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMenuPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleMenuPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "dbo",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleMenuPermissions_RoleMenus_RoleMenuId",
                        column: x => x.RoleMenuId,
                        principalSchema: "dbo",
                        principalTable: "RoleMenus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenuPermissions_PermissionId",
                schema: "dbo",
                table: "RoleMenuPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenuPermissions_RoleMenuId",
                schema: "dbo",
                table: "RoleMenuPermissions",
                column: "RoleMenuId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_MenuId",
                schema: "dbo",
                table: "RoleMenus",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_ParentId",
                schema: "dbo",
                table: "RoleMenus",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_RoleId",
                schema: "dbo",
                table: "RoleMenus",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                schema: "dbo",
                table: "Users",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleMenuPermissions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RoleMenus",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Menus",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "dbo");
        }
    }
}
