using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Currecies",
                schema: "dbo",
                columns: table => new
                {
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    NativeName = table.Column<string>(nullable: true),
                    Symbol = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreatedByUsername = table.Column<string>(nullable: true),
                    UpdatedByUsername = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currecies", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "GameProviders",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    ShortName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PopupWidth = table.Column<int>(nullable: true),
                    PopupHeight = table.Column<int>(nullable: true),
                    AutoCloseRound = table.Column<bool>(nullable: false),
                    SupportTestPlayer = table.Column<bool>(nullable: false),
                    SupportBetHistory = table.Column<bool>(nullable: false),
                    SupportMultipleGamesInLobby = table.Column<bool>(nullable: false),
                    SupportMultipleGamesInDirectGameLauncher = table.Column<bool>(nullable: false),
                    IomCompliant = table.Column<bool>(nullable: false),
                    SupportIFrameDesktop = table.Column<bool>(nullable: false),
                    SupportIFrameMobile = table.Column<bool>(nullable: false),
                    SupportDemoMode = table.Column<bool>(nullable: false),
                    PlaceBetTimeout = table.Column<int>(nullable: false),
                    SettleBetTimeout = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                schema: "dbo",
                columns: table => new
                {
                    LanguageCode = table.Column<string>(nullable: false),
                    CultureCode = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    LocalizedName = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreatedByUsername = table.Column<string>(nullable: true),
                    UpdatedByUsername = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageCode);
                });

            migrationBuilder.CreateTable(
                name: "Resellers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    OperationalEmail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resellers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameProviderConfigurations",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ClientId = table.Column<string>(nullable: true),
                    ClientSecret = table.Column<string>(nullable: true),
                    SecurityKey = table.Column<string>(nullable: true),
                    GameProviderId = table.Column<Guid>(nullable: false),
                    Endpoint = table.Column<string>(nullable: true),
                    ConfigurationData = table.Column<string>(nullable: true),
                    SuppressDescription = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviderConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameProviderConfigurations_GameProviders_GameProviderId",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameProviderCurrency",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GameProviderId = table.Column<Guid>(nullable: false),
                    CurrencyCode = table.Column<string>(nullable: true),
                    GameProviderCurrencyCode = table.Column<string>(nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviderCurrency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameProviderCurrency_Currecies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currecies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameProviderCurrency_GameProviders_GameProviderId",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    ExternalId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    EndpointPath = table.Column<string>(nullable: true),
                    DemoUrl = table.Column<string>(nullable: true),
                    RuleUrl = table.Column<string>(nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false),
                    GameProviderId = table.Column<Guid>(nullable: false),
                    GameType = table.Column<int>(nullable: false),
                    PlatformType = table.Column<byte>(nullable: false),
                    ZeroTurnoverOnTieBet = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_GameProviders_GameProviderId",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameProviderLanguage",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GameProviderId = table.Column<Guid>(nullable: false),
                    CultureCode = table.Column<string>(nullable: true),
                    LanguageCode = table.Column<string>(nullable: true),
                    GameProviderCultureCode = table.Column<string>(nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviderLanguage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameProviderLanguage_GameProviders_GameProviderId",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameProviderLanguage_Languages_LanguageCode",
                        column: x => x.LanguageCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "LanguageCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Licensees",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    OperationalEmail = table.Column<string>(nullable: true),
                    BrandLimit = table.Column<int>(nullable: false),
                    ResellerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licensees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Licensees_Resellers_ResellerId",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "Resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResellerCurrencies",
                schema: "dbo",
                columns: table => new
                {
                    ResellerId = table.Column<Guid>(nullable: false),
                    CurrencyCode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResellerCurrencies", x => new { x.ResellerId, x.CurrencyCode });
                    table.ForeignKey(
                        name: "FK_ResellerCurrencies_Currecies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currecies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResellerCurrencies_Resellers_ResellerId",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "Resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResellerGameProviders",
                schema: "dbo",
                columns: table => new
                {
                    ResellerId = table.Column<Guid>(nullable: false),
                    GameProviderId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResellerGameProviders", x => new { x.ResellerId, x.GameProviderId });
                    table.ForeignKey(
                        name: "FK_ResellerGameProviders_GameProviders_GameProviderId",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResellerGameProviders_Resellers_ResellerId",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "Resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResellerLanguages",
                schema: "dbo",
                columns: table => new
                {
                    ResellerId = table.Column<Guid>(nullable: false),
                    CultureCode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResellerLanguages", x => new { x.ResellerId, x.CultureCode });
                    table.ForeignKey(
                        name: "FK_ResellerLanguages_Languages_CultureCode",
                        column: x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "LanguageCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResellerLanguages_Resellers_ResellerId",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "Resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    OperationalEmail = table.Column<string>(nullable: true),
                    TimeZoneId = table.Column<string>(nullable: true),
                    ResellerId = table.Column<Guid>(nullable: false),
                    LicenseeId = table.Column<Guid>(nullable: false),
                    LoginUrl = table.Column<string>(nullable: true),
                    CashierUrl = table.Column<string>(nullable: true),
                    HelpUrl = table.Column<string>(nullable: true),
                    TermsUrl = table.Column<string>(nullable: true),
                    MobileLoginUrl = table.Column<string>(nullable: true),
                    MobileCashierUrl = table.Column<string>(nullable: true),
                    MobileHelpUrl = table.Column<string>(nullable: true),
                    MobileTermsUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Brands_Licensees_LicenseeId",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "Licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Brands_Resellers_ResellerId",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "Resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LicenseeCurrencies",
                schema: "dbo",
                columns: table => new
                {
                    LicenseeId = table.Column<Guid>(nullable: false),
                    CurrencyCode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseeCurrencies", x => new { x.LicenseeId, x.CurrencyCode });
                    table.ForeignKey(
                        name: "FK_LicenseeCurrencies_Currecies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currecies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LicenseeCurrencies_Licensees_LicenseeId",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "Licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LicenseeLanguages",
                schema: "dbo",
                columns: table => new
                {
                    LicenseeId = table.Column<Guid>(nullable: false),
                    CultureCode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseeLanguages", x => new { x.LicenseeId, x.CultureCode });
                    table.ForeignKey(
                        name: "FK_LicenseeLanguages_Languages_CultureCode",
                        column: x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "LanguageCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LicenseeLanguages_Licensees_LicenseeId",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "Licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandCurrency",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    CurrencyCode = table.Column<string>(nullable: true),
                    BrandCurrencyCode = table.Column<string>(nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandCurrency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandCurrency_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandCurrency_Currecies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currecies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BrandLanguage",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    CultureCode = table.Column<string>(nullable: true),
                    LanguageCode = table.Column<string>(nullable: true),
                    BrandLanguageCode = table.Column<string>(nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandLanguage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandLanguage_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandLanguage_Languages_LanguageCode",
                        column: x => x.LanguageCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "LanguageCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Player",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    ExternalId = table.Column<string>(nullable: true),
                    PlayerCode = table.Column<string>(nullable: true),
                    IpAddress = table.Column<string>(nullable: true),
                    CultureCode = table.Column<string>(nullable: true),
                    LanguageCode = table.Column<string>(nullable: true),
                    DefaultWalletId = table.Column<Guid>(nullable: false),
                    DefaultWalletCurrencyCode = table.Column<string>(nullable: true),
                    BrandId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    LastActiveOn = table.Column<DateTimeOffset>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    BetLimitGroupId = table.Column<int>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    LastLoginUrl = table.Column<string>(nullable: true),
                    LastCashierUrl = table.Column<string>(nullable: true),
                    LastHelpUrl = table.Column<string>(nullable: true),
                    LastTermsUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Player_Languages_LanguageCode",
                        column: x => x.LanguageCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "LanguageCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandCurrency_BrandId",
                schema: "dbo",
                table: "BrandCurrency",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandCurrency_CurrencyCode",
                schema: "dbo",
                table: "BrandCurrency",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_BrandLanguage_BrandId",
                schema: "dbo",
                table: "BrandLanguage",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandLanguage_LanguageCode",
                schema: "dbo",
                table: "BrandLanguage",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_LicenseeId",
                schema: "dbo",
                table: "Brands",
                column: "LicenseeId");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_ResellerId",
                schema: "dbo",
                table: "Brands",
                column: "ResellerId");

            migrationBuilder.CreateIndex(
                name: "IX_GameProviderConfigurations_GameProviderId",
                schema: "dbo",
                table: "GameProviderConfigurations",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_GameProviderCurrency_CurrencyCode",
                schema: "dbo",
                table: "GameProviderCurrency",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_GameProviderCurrency_GameProviderId",
                schema: "dbo",
                table: "GameProviderCurrency",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_GameProviderLanguage_GameProviderId",
                schema: "dbo",
                table: "GameProviderLanguage",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_GameProviderLanguage_LanguageCode",
                schema: "dbo",
                table: "GameProviderLanguage",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameProviderId",
                schema: "dbo",
                table: "Games",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseeCurrencies_CurrencyCode",
                schema: "dbo",
                table: "LicenseeCurrencies",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseeLanguages_CultureCode",
                schema: "dbo",
                table: "LicenseeLanguages",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                name: "IX_Licensees_ResellerId",
                schema: "dbo",
                table: "Licensees",
                column: "ResellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_BrandId",
                schema: "dbo",
                table: "Player",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_LanguageCode",
                schema: "dbo",
                table: "Player",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_ResellerCurrencies_CurrencyCode",
                schema: "dbo",
                table: "ResellerCurrencies",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_ResellerGameProviders_GameProviderId",
                schema: "dbo",
                table: "ResellerGameProviders",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ResellerLanguages_CultureCode",
                schema: "dbo",
                table: "ResellerLanguages",
                column: "CultureCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandCurrency",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandLanguage",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "GameProviderConfigurations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "GameProviderCurrency",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "GameProviderLanguage",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Games",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LicenseeCurrencies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LicenseeLanguages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Player",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ResellerCurrencies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ResellerGameProviders",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ResellerLanguages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Brands",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Currecies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "GameProviders",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Languages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Licensees",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Resellers",
                schema: "dbo");
        }
    }
}
