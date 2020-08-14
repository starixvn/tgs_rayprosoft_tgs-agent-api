using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Migrations
{
    public partial class addTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Player_Brands_BrandId",
                schema: "dbo",
                table: "Player");

            migrationBuilder.DropForeignKey(
                name: "FK_Player_Languages_LanguageCode",
                schema: "dbo",
                table: "Player");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Player",
                schema: "dbo",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "Resellers");

            migrationBuilder.DropColumn(
                name: "DemoUrl",
                schema: "dbo",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                schema: "dbo",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ZeroTurnoverOnTieBet",
                schema: "dbo",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "IomCompliant",
                schema: "dbo",
                table: "GameProviders");

            migrationBuilder.DropColumn(
                name: "PopupHeight",
                schema: "dbo",
                table: "GameProviders");

            migrationBuilder.DropColumn(
                name: "PopupWidth",
                schema: "dbo",
                table: "GameProviders");

            migrationBuilder.DropColumn(
                name: "ShortName",
                schema: "dbo",
                table: "GameProviders");

            migrationBuilder.DropColumn(
                name: "SupportBetHistory",
                schema: "dbo",
                table: "GameProviders");

            migrationBuilder.DropColumn(
                name: "SupportDemoMode",
                schema: "dbo",
                table: "GameProviders");

            migrationBuilder.DropColumn(
                name: "SupportIFrameDesktop",
                schema: "dbo",
                table: "GameProviders");

            migrationBuilder.DropColumn(
                name: "SupportIFrameMobile",
                schema: "dbo",
                table: "GameProviders");

            migrationBuilder.DropColumn(
                name: "SupportMultipleGamesInDirectGameLauncher",
                schema: "dbo",
                table: "GameProviders");

            migrationBuilder.DropColumn(
                name: "SupportMultipleGamesInLobby",
                schema: "dbo",
                table: "GameProviders");

            migrationBuilder.DropColumn(
                name: "ClientId",
                schema: "dbo",
                table: "GameProviderConfigurations");

            migrationBuilder.DropColumn(
                name: "ClientSecret",
                schema: "dbo",
                table: "GameProviderConfigurations");

            migrationBuilder.DropColumn(
                name: "Endpoint",
                schema: "dbo",
                table: "GameProviderConfigurations");

            migrationBuilder.DropColumn(
                name: "SecurityKey",
                schema: "dbo",
                table: "GameProviderConfigurations");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "GameProviderConfigurations");

            migrationBuilder.DropColumn(
                name: "SuppressDescription",
                schema: "dbo",
                table: "GameProviderConfigurations");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "dbo",
                table: "Player");

            migrationBuilder.RenameTable(
                name: "Player",
                schema: "dbo",
                newName: "Players",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_Player_LanguageCode",
                schema: "dbo",
                table: "Players",
                newName: "IX_Players_LanguageCode");

            migrationBuilder.RenameIndex(
                name: "IX_Player_BrandId",
                schema: "dbo",
                table: "Players",
                newName: "IX_Players_BrandId");

            migrationBuilder.AddColumn<string>(
                name: "DesktopEndpoint",
                schema: "dbo",
                table: "GameProviderConfigurations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileEndpoint",
                schema: "dbo",
                table: "GameProviderConfigurations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecretKey",
                schema: "dbo",
                table: "GameProviderConfigurations",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PlayerCode",
                schema: "dbo",
                table: "Players",
                type: "varchar(16)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Players",
                schema: "dbo",
                table: "Players",
                column: "PlayerCode");

            migrationBuilder.CreateTable(
                name: "BrandGames",
                schema: "dbo",
                columns: table => new
                {
                    GameId = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandGames", x => new { x.BrandId, x.GameId });
                    table.ForeignKey(
                        name: "FK_BrandGames_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandGames_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "dbo",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocalizedStrings",
                schema: "dbo",
                columns: table => new
                {
                    Context = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    CultureCode = table.Column<string>(nullable: false),
                    Translation = table.Column<string>(nullable: true),
                    LanguageCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizedStrings", x => new { x.Context, x.CultureCode, x.Code });
                    table.ForeignKey(
                        name: "FK_LocalizedStrings_Languages_LanguageCode",
                        column: x => x.LanguageCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "LanguageCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "dbo",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreatedByUsername = table.Column<string>(nullable: true),
                    UpdatedByUsername = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    PlayerCode = table.Column<string>(nullable: true),
                    CurrencyCode = table.Column<string>(nullable: true),
                    ExternalCurrencyCode = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Balance = table.Column<decimal>(nullable: false),
                    CreatedOn = table.Column<string>(nullable: true),
                    LastModifiedOn = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_Currecies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currecies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Wallets_Players_PlayerCode",
                        column: x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "Players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Whitelists",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: true),
                    LicenseeId = table.Column<Guid>(nullable: true),
                    ResellerId = table.Column<Guid>(nullable: true),
                    IpAddress = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    InternalNote = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreatedByUsername = table.Column<string>(nullable: true),
                    UpdatedByUsername = table.Column<string>(nullable: true),
                    Type = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Whitelists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Whitelists_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Whitelists_Licensees_LicenseeId",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "Licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Whitelists_Resellers_ResellerId",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "Resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandGames_GameId",
                schema: "dbo",
                table: "BrandGames",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalizedStrings_LanguageCode",
                schema: "dbo",
                table: "LocalizedStrings",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_CurrencyCode",
                schema: "dbo",
                table: "Wallets",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_PlayerCode",
                schema: "dbo",
                table: "Wallets",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                name: "IX_Whitelists_BrandId",
                schema: "dbo",
                table: "Whitelists",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Whitelists_LicenseeId",
                schema: "dbo",
                table: "Whitelists",
                column: "LicenseeId");

            migrationBuilder.CreateIndex(
                name: "IX_Whitelists_ResellerId",
                schema: "dbo",
                table: "Whitelists",
                column: "ResellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Brands_BrandId",
                schema: "dbo",
                table: "Players",
                column: "BrandId",
                principalSchema: "dbo",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Languages_LanguageCode",
                schema: "dbo",
                table: "Players",
                column: "LanguageCode",
                principalSchema: "dbo",
                principalTable: "Languages",
                principalColumn: "LanguageCode",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Brands_BrandId",
                schema: "dbo",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Languages_LanguageCode",
                schema: "dbo",
                table: "Players");

            migrationBuilder.DropTable(
                name: "BrandGames",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LocalizedStrings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Settings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Wallets",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Whitelists",
                schema: "dbo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Players",
                schema: "dbo",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "DesktopEndpoint",
                schema: "dbo",
                table: "GameProviderConfigurations");

            migrationBuilder.DropColumn(
                name: "MobileEndpoint",
                schema: "dbo",
                table: "GameProviderConfigurations");

            migrationBuilder.DropColumn(
                name: "SecretKey",
                schema: "dbo",
                table: "GameProviderConfigurations");

            migrationBuilder.RenameTable(
                name: "Players",
                schema: "dbo",
                newName: "Player",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_Players_LanguageCode",
                schema: "dbo",
                table: "Player",
                newName: "IX_Player_LanguageCode");

            migrationBuilder.RenameIndex(
                name: "IX_Players_BrandId",
                schema: "dbo",
                table: "Player",
                newName: "IX_Player_BrandId");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "dbo",
                table: "Resellers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DemoUrl",
                schema: "dbo",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                schema: "dbo",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ZeroTurnoverOnTieBet",
                schema: "dbo",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IomCompliant",
                schema: "dbo",
                table: "GameProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PopupHeight",
                schema: "dbo",
                table: "GameProviders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PopupWidth",
                schema: "dbo",
                table: "GameProviders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                schema: "dbo",
                table: "GameProviders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SupportBetHistory",
                schema: "dbo",
                table: "GameProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SupportDemoMode",
                schema: "dbo",
                table: "GameProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SupportIFrameDesktop",
                schema: "dbo",
                table: "GameProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SupportIFrameMobile",
                schema: "dbo",
                table: "GameProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SupportMultipleGamesInDirectGameLauncher",
                schema: "dbo",
                table: "GameProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SupportMultipleGamesInLobby",
                schema: "dbo",
                table: "GameProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                schema: "dbo",
                table: "GameProviderConfigurations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientSecret",
                schema: "dbo",
                table: "GameProviderConfigurations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endpoint",
                schema: "dbo",
                table: "GameProviderConfigurations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecurityKey",
                schema: "dbo",
                table: "GameProviderConfigurations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "dbo",
                table: "GameProviderConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SuppressDescription",
                schema: "dbo",
                table: "GameProviderConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "PlayerCode",
                schema: "dbo",
                table: "Player",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(16)");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "dbo",
                table: "Player",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Player",
                schema: "dbo",
                table: "Player",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Player_Brands_BrandId",
                schema: "dbo",
                table: "Player",
                column: "BrandId",
                principalSchema: "dbo",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Player_Languages_LanguageCode",
                schema: "dbo",
                table: "Player",
                column: "LanguageCode",
                principalSchema: "dbo",
                principalTable: "Languages",
                principalColumn: "LanguageCode",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
