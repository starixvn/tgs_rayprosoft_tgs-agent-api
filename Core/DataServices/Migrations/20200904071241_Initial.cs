using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "BetLimitGroups",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BetLimitGroups", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                schema: "dbo",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    Name = table.Column<string>(nullable: true),
                    NativeName = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    Symbol = table.Column<string>(type: "nvarchar(16)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "GameProviders",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    AutoCloseRound = table.Column<bool>(nullable: false),
                    SupportTestPlayer = table.Column<bool>(nullable: false),
                    PlaceBetTimeout = table.Column<short>(type: "smallint", nullable: false),
                    SettleBetTimeout = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviders", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                schema: "dbo",
                columns: table => new
                {
                    CultureCode = table.Column<string>(type: "nvarchar(6)", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.CultureCode);
                });

            migrationBuilder.CreateTable(
                name: "Resellers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    OperationalEmail = table.Column<string>(type: "nvarchar(128)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resellers", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                name: "GameProviderBetLimits",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    GameProviderId = table.Column<Guid>(nullable: false),
                    BetLimitGroupId = table.Column<int>(nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviderBetLimits", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_GameProviderBetLimits_BetLimitGroups_BetLimitGroupId",
                        column: x => x.BetLimitGroupId,
                        principalSchema: "dbo",
                        principalTable: "BetLimitGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameProviderBetLimits_Currencies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameProviderBetLimits_GameProviders_GameProviderId",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameProviderConfigurations",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    SecretKey = table.Column<string>(type: "varchar(64)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    GameProviderId = table.Column<Guid>(nullable: false),
                    DesktopEndpoint = table.Column<string>(type: "nvarchar(1024)", nullable: false),
                    MobileEndpoint = table.Column<string>(type: "nvarchar(1024)", nullable: false),
                    ConfigurationData = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviderConfigurations", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_GameProviderConfigurations_GameProviders_GameProviderId",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameProviderCurrencies",
                schema: "dbo",
                columns: table => new
                {
                    GameProviderId = table.Column<Guid>(nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    GameProviderCurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviderCurrencies", x => new { x.GameProviderId, x.CurrencyCode });
                    table.ForeignKey(
                        name: "FK_GameProviderCurrencies_Currencies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameProviderCurrencies_GameProviders_GameProviderId",
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
                    Code = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    EndpointPath = table.Column<string>(type: "nvarchar(2048)", nullable: true),
                    RuleUrl = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    GameProviderId = table.Column<Guid>(nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    PlatformType = table.Column<short>(type: "smallint", nullable: false),
                    SubGame = table.Column<bool>(nullable: false, defaultValue: false),
                    HasTurnoverOnTieBet = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Games_GameProviders_GameProviderId",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameProviderLanguages",
                schema: "dbo",
                columns: table => new
                {
                    GameProviderId = table.Column<Guid>(nullable: false),
                    CultureCode = table.Column<string>(type: "nvarchar(6)", nullable: false),
                    GameProviderCultureCode = table.Column<string>(type: "nvarchar(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviderLanguages", x => new { x.GameProviderId, x.CultureCode });
                    table.ForeignKey(
                        name: "FK_GameProviderLanguages_Languages_CultureCode",
                        column: x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameProviderLanguages_GameProviders_GameProviderId",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocalizedStrings",
                schema: "dbo",
                columns: table => new
                {
                    Context = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    CultureCode = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    Translation = table.Column<string>(type: "nvarchar(1024)", nullable: false),
                    LanguageCultureCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizedStrings", x => new { x.Context, x.CultureCode, x.Code });
                    table.ForeignKey(
                        name: "FK_LocalizedStrings_Languages_LanguageCultureCode",
                        column: x => x.LanguageCultureCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    TimeZoneId = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    ResellerId = table.Column<Guid>(nullable: false),
                    LoginUrl = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    CashierUrl = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    HelpUrl = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    TermsUrl = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    MobileLoginUrl = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    MobileCashierUrl = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    MobileHelpUrl = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    MobileTermsUrl = table.Column<string>(type: "nvarchar(512)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Brands_Resellers_ResellerId",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "Resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        name: "FK_ResellerCurrencies_Currencies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currencies",
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
                        principalColumn: "CultureCode",
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
                name: "BrandBetLimits",
                schema: "dbo",
                columns: table => new
                {
                    BetLimitGroupId = table.Column<int>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandBetLimits", x => new { x.BrandId, x.BetLimitGroupId });
                    table.ForeignKey(
                        name: "FK_BrandBetLimits_BetLimitGroups_BetLimitGroupId",
                        column: x => x.BetLimitGroupId,
                        principalSchema: "dbo",
                        principalTable: "BetLimitGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandBetLimits_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandCurrencies",
                schema: "dbo",
                columns: table => new
                {
                    BrandId = table.Column<Guid>(nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    BrandCurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandCurrencies", x => new { x.BrandId, x.CurrencyCode });
                    table.ForeignKey(
                        name: "FK_BrandCurrencies_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandCurrencies_Currencies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandGameProviderConfigurations",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    DefaultLanguageCode = table.Column<string>(type: "nvarchar(6)", nullable: true),
                    GameProviderId = table.Column<Guid>(nullable: false),
                    BrandLogoUrl = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    DesktopQueryString = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    MobileQueryString = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    GameProviderConfigurationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandGameProviderConfigurations", x => x.Id);
                    table.UniqueConstraint("UK_BrandGameProviderConfigurations_BrandId_GameProviderId", x => new { x.BrandId, x.GameProviderId });
                    table.ForeignKey(
                        name: "FK_BrandGameProviderConfigurations_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BrandGameProviderConfigurations_Languages_DefaultLanguageCode",
                        column: x => x.DefaultLanguageCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BrandGameProviderConfigurations_GameProviders_GameProviderId",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BrandGameProviderConfigurations_GameProviderConfigurations_GameProviderId",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviderConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "BrandLanguages",
                schema: "dbo",
                columns: table => new
                {
                    BrandId = table.Column<Guid>(nullable: false),
                    CultureCode = table.Column<string>(type: "nvarchar(6)", nullable: false),
                    BrandLanguageCode = table.Column<string>(type: "nvarchar(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandLanguages", x => new { x.BrandId, x.CultureCode });
                    table.ForeignKey(
                        name: "FK_BrandLanguages_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandLanguages_Languages_CultureCode",
                        column: x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                schema: "dbo",
                columns: table => new
                {
                    PlayerCode = table.Column<string>(type: "varchar(16)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    CultureCode = table.Column<string>(type: "nvarchar(6)", nullable: false),
                    DefaultWalletId = table.Column<Guid>(nullable: false),
                    DefaultWalletCurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastActiveOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    BetLimitGroupId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    LastLoginUrl = table.Column<string>(type: "nvarchar(1024)", nullable: true),
                    LastCashierUrl = table.Column<string>(type: "nvarchar(1024)", nullable: true),
                    LastHelpUrl = table.Column<string>(type: "nvarchar(1024)", nullable: true),
                    LastTermsUrl = table.Column<string>(type: "nvarchar(1024)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerCode);
                    table.ForeignKey(
                        name: "FK_Players_BetLimitGroups_BetLimitGroupId",
                        column: x => x.BetLimitGroupId,
                        principalSchema: "dbo",
                        principalTable: "BetLimitGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Players_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Players_Languages_CultureCode",
                        column: x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
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
                    IpAddress = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    InternalNote = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
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
                        name: "FK_Whitelists_Resellers_ResellerId",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "Resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerLoginRecords",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerCode = table.Column<string>(type: "varchar(16)", nullable: false),
                    BrandId = table.Column<Guid>(nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    PlatformType = table.Column<short>(type: "smallint", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiredOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerLoginRecords", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_PlayerLoginRecords_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerLoginRecords_Players_PlayerCode",
                        column: x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "Players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerCode = table.Column<string>(type: "varchar(16)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    ExternalCurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_Currencies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wallets_Players_PlayerCode",
                        column: x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "Players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandBetLimits_BetLimitGroupId",
                schema: "dbo",
                table: "BrandBetLimits",
                column: "BetLimitGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandCurrencies_CurrencyCode",
                schema: "dbo",
                table: "BrandCurrencies",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_BrandGameProviderConfigurations_DefaultLanguageCode",
                schema: "dbo",
                table: "BrandGameProviderConfigurations",
                column: "DefaultLanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_BrandGameProviderConfigurations_GameProviderId",
                schema: "dbo",
                table: "BrandGameProviderConfigurations",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandGames_GameId",
                schema: "dbo",
                table: "BrandGames",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandLanguages_CultureCode",
                schema: "dbo",
                table: "BrandLanguages",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_ResellerId",
                schema: "dbo",
                table: "Brands",
                column: "ResellerId");

            migrationBuilder.CreateIndex(
                name: "IX_GameProviderBetLimits_BetLimitGroupId",
                schema: "dbo",
                table: "GameProviderBetLimits",
                column: "BetLimitGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GameProviderBetLimits_CurrencyCode",
                schema: "dbo",
                table: "GameProviderBetLimits",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_GameProviderBetLimits_GameProviderId",
                schema: "dbo",
                table: "GameProviderBetLimits",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_GameProviderConfigurations_GameProviderId",
                schema: "dbo",
                table: "GameProviderConfigurations",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_GameProviderCurrencies_CurrencyCode",
                schema: "dbo",
                table: "GameProviderCurrencies",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_GameProviderLanguages_CultureCode",
                schema: "dbo",
                table: "GameProviderLanguages",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameProviderId",
                schema: "dbo",
                table: "Games",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalizedStrings_LanguageCultureCode",
                schema: "dbo",
                table: "LocalizedStrings",
                column: "LanguageCultureCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLoginRecords_BrandId",
                schema: "dbo",
                table: "PlayerLoginRecords",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLoginRecords_PlayerCode",
                schema: "dbo",
                table: "PlayerLoginRecords",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                name: "IX_Players_BetLimitGroupId",
                schema: "dbo",
                table: "Players",
                column: "BetLimitGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_BrandId",
                schema: "dbo",
                table: "Players",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_CultureCode",
                schema: "dbo",
                table: "Players",
                column: "CultureCode");

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
                name: "IX_Whitelists_ResellerId",
                schema: "dbo",
                table: "Whitelists",
                column: "ResellerId");

            #region Store Procedure

            var assembly = Assembly.GetExecutingAssembly();

            migrationBuilder.SqlResource(assembly,
                "TGS.Core.DataServices.SqlScripts.SP.usp_GameProvider_GetGameProviderIdByCode.sql");
            migrationBuilder.SqlResource(assembly,
                "TGS.Core.DataServices.SqlScripts.SP.usp_GameProvider_GetCurrenciesByInternalCode.sql");
            migrationBuilder.SqlResource(assembly,
                "TGS.Core.DataServices.SqlScripts.SP.usp_GameProvider_GetCurrenciesByExternalCode.sql");
            migrationBuilder.SqlResource(assembly,
                "TGS.Core.DataServices.SqlScripts.SP.usp_Player_GetPlayerForCache.sql");
            migrationBuilder.SqlResource(assembly,
                "TGS.Core.DataServices.SqlScripts.SP.usp_Wallet_GetBalance.sql");
            migrationBuilder.SqlResource(assembly,
               "TGS.Core.DataServices.SqlScripts.SP.usp_Brand_GetBrandForCache.sql");
            migrationBuilder.SqlResource(assembly,
               "TGS.Core.DataServices.SqlScripts.SP.usp_Brand_GetBrandGameProviderConfigurationForCache.sql");
            migrationBuilder.SqlResource(assembly,
               "TGS.Core.DataServices.SqlScripts.SP.usp_GameProvider_GetGameProviderForCache.sql");
            migrationBuilder.SqlResource(assembly,
               "TGS.Core.DataServices.SqlScripts.SP.usp_GameProvider_GetGameProviderConfigurationForCache.sql");
            migrationBuilder.SqlResource(assembly,
                "TGS.Core.DataServices.SqlScripts.SP.usp_GameProvider_GetLanguagesByInternalCode.sql");
            migrationBuilder.SqlResource(assembly,
                "TGS.Core.DataServices.SqlScripts.SP.usp_GameProvider_GetLanguagesByExternalCode.sql");
            migrationBuilder.SqlResource(assembly,
                "TGS.Core.DataServices.SqlScripts.SP.usp_GameProvider_GetGameProviderBetLimitCode.sql");
            migrationBuilder.SqlResource(assembly,
                "TGS.Core.DataServices.SqlScripts.SP.usp_Player_GetPlayerLoginRecords.sql");
            migrationBuilder.SqlResource(assembly,
                "TGS.Core.DataServices.SqlScripts.SP.usp_Brand_GetBrandGamesForCache.sql");
            migrationBuilder.SqlResource(assembly,
                "TGS.Core.DataServices.SqlScripts.SP.usp_GameProvider_GetGamesForCache.sql");

            migrationBuilder.SqlResource(assembly,
                "TGS.Core.DataServices.SqlScripts.TYPE.IntValuesType.sql");

            #endregion Store Procedure
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandBetLimits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandCurrencies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandGameProviderConfigurations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandGames",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandLanguages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "GameProviderBetLimits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "GameProviderCurrencies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "GameProviderLanguages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LocalizedStrings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PlayerLoginRecords",
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
                name: "Settings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Wallets",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Whitelists",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "GameProviderConfigurations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Games",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Currencies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Players",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "GameProviders",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BetLimitGroups",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Brands",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Languages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Resellers",
                schema: "dbo");
        }
    }
}
