using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Announcements",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(4000)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    Status = table.Column<int>(nullable: false),
                    From = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    To = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DisplayOrder = table.Column<short>(type: "smallint", nullable: false),
                    VerifiedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ModuleName = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    PlayerCode = table.Column<string>(type: "varchar(16)", nullable: true),
                    Operation = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

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
                    LocalizedName = table.Column<string>(nullable: true),
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
                name: "StationLetters",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(4000)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    AllPlayers = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    VerifiedByUsername = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationLetters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VipLevels",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(type: "varchar(16)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VipLevels", x => x.Id);
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
                name: "Licensees",
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
                    table.PrimaryKey("PK_Licensees", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Licensees_Resellers_ResellerId",
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
                name: "LicenseeBetLimits",
                schema: "dbo",
                columns: table => new
                {
                    BetLimitGroupId = table.Column<int>(nullable: false),
                    LicenseeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseeBetLimits", x => new { x.LicenseeId, x.BetLimitGroupId });
                    table.ForeignKey(
                        name: "FK_LicenseeBetLimits_BetLimitGroups_BetLimitGroupId",
                        column: x => x.BetLimitGroupId,
                        principalSchema: "dbo",
                        principalTable: "BetLimitGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LicenseeBetLimits_Licensees_LicenseeId",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "Licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LicenseeCurrencies",
                schema: "dbo",
                columns: table => new
                {
                    LicenseeId = table.Column<Guid>(nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    LicenseeCurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseeCurrencies", x => new { x.LicenseeId, x.CurrencyCode });
                    table.ForeignKey(
                        name: "FK_LicenseeCurrencies_Currencies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currencies",
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
                name: "LicenseeGameProviderConfigurations",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    LicenseeId = table.Column<Guid>(nullable: false),
                    DefaultLanguageCode = table.Column<string>(type: "nvarchar(6)", nullable: true),
                    GameProviderId = table.Column<Guid>(nullable: false),
                    LicenseeLogoUrl = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    DesktopQueryString = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    MobileQueryString = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    GameProviderConfigurationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseeGameProviderConfigurations", x => x.Id);
                    table.UniqueConstraint("UK_LicenseeGameProviderConfigurations_LicenseeId_GameProviderId", x => new { x.LicenseeId, x.GameProviderId });
                    table.ForeignKey(
                        name: "FK_LicenseeGameProviderConfigurations_Languages_DefaultLanguageCode",
                        column: x => x.DefaultLanguageCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LicenseeGameProviderConfigurations_GameProviders_GameProviderId",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LicenseeGameProviderConfigurations_GameProviderConfigurations_GameProviderId",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviderConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LicenseeGameProviderConfigurations_Licensees_LicenseeId",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "Licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LicenseeGames",
                schema: "dbo",
                columns: table => new
                {
                    GameId = table.Column<Guid>(nullable: false),
                    LicenseeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseeGames", x => new { x.LicenseeId, x.GameId });
                    table.ForeignKey(
                        name: "FK_LicenseeGames_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "dbo",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LicenseeGames_Licensees_LicenseeId",
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
                    CultureCode = table.Column<string>(type: "nvarchar(6)", nullable: false),
                    LicenseeLanguageCode = table.Column<string>(type: "nvarchar(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseeLanguages", x => new { x.LicenseeId, x.CultureCode });
                    table.ForeignKey(
                        name: "FK_LicenseeLanguages_Languages_CultureCode",
                        column: x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
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
                name: "Players",
                schema: "dbo",
                columns: table => new
                {
                    PlayerCode = table.Column<string>(type: "varchar(16)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    RealName = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    Password = table.Column<byte[]>(type: "varbinary(128)", nullable: false),
                    Salt = table.Column<byte[]>(type: "varbinary(128)", nullable: false),
                    CultureCode = table.Column<string>(type: "nvarchar(6)", nullable: false),
                    DefaultWalletId = table.Column<Guid>(nullable: false),
                    DefaultWalletCurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    LicenseeId = table.Column<Guid>(nullable: false),
                    RegisteredOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastActiveOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    BetLimitGroupId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    Email = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    QQ = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    Wechat = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    HasAgent = table.Column<bool>(nullable: false),
                    ParentPlayerCode = table.Column<string>(nullable: true),
                    VipId = table.Column<Guid>(nullable: false),
                    UpdatedByUsername = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false)
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
                        name: "FK_Players_Languages_CultureCode",
                        column: x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Players_Licensees_LicenseeId",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "Licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Players_VipLevels_VipId",
                        column: x => x.VipId,
                        principalSchema: "dbo",
                        principalTable: "VipLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Whitelists",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
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

            migrationBuilder.CreateTable(
                name: "PlayerBanks",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerCode = table.Column<string>(type: "varchar(16)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    BankAccount = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerBanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerBanks_Players_PlayerCode",
                        column: x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "Players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerLoginRecords",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerCode = table.Column<string>(type: "varchar(16)", nullable: false),
                    LicenseeId = table.Column<Guid>(nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    PlatformType = table.Column<short>(type: "smallint", nullable: false),
                    IpAddress = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiredOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerLoginRecords", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_PlayerLoginRecords_Licensees_LicenseeId",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "Licensees",
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
                name: "StationLetterPlayers",
                schema: "dbo",
                columns: table => new
                {
                    StationLetterId = table.Column<Guid>(nullable: false),
                    PlayerCode = table.Column<string>(nullable: false),
                    IsRead = table.Column<bool>(nullable: false),
                    ReadOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationLetterPlayers", x => new { x.StationLetterId, x.PlayerCode });
                    table.ForeignKey(
                        name: "FK_StationLetterPlayers_Players_PlayerCode",
                        column: x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "Players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StationLetterPlayers_StationLetters_StationLetterId",
                        column: x => x.StationLetterId,
                        principalSchema: "dbo",
                        principalTable: "StationLetters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_LicenseeBetLimits_BetLimitGroupId",
                schema: "dbo",
                table: "LicenseeBetLimits",
                column: "BetLimitGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseeCurrencies_CurrencyCode",
                schema: "dbo",
                table: "LicenseeCurrencies",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseeGameProviderConfigurations_DefaultLanguageCode",
                schema: "dbo",
                table: "LicenseeGameProviderConfigurations",
                column: "DefaultLanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseeGameProviderConfigurations_GameProviderId",
                schema: "dbo",
                table: "LicenseeGameProviderConfigurations",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseeGames_GameId",
                schema: "dbo",
                table: "LicenseeGames",
                column: "GameId");

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
                name: "IX_LocalizedStrings_LanguageCultureCode",
                schema: "dbo",
                table: "LocalizedStrings",
                column: "LanguageCultureCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerBanks_PlayerCode",
                schema: "dbo",
                table: "PlayerBanks",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLoginRecords_LicenseeId",
                schema: "dbo",
                table: "PlayerLoginRecords",
                column: "LicenseeId");

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
                name: "IX_Players_CultureCode",
                schema: "dbo",
                table: "Players",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                name: "IX_Players_LicenseeId",
                schema: "dbo",
                table: "Players",
                column: "LicenseeId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_VipId",
                schema: "dbo",
                table: "Players",
                column: "VipId");

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
                name: "IX_StationLetterPlayers_PlayerCode",
                schema: "dbo",
                table: "StationLetterPlayers",
                column: "PlayerCode");

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
                name: "IX_Whitelists_LicenseeId",
                schema: "dbo",
                table: "Whitelists",
                column: "LicenseeId");

            migrationBuilder.CreateIndex(
                name: "IX_Whitelists_ResellerId",
                schema: "dbo",
                table: "Whitelists",
                column: "ResellerId");

            InitAfterRun(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Announcements",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AuditLogs",
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
                name: "LicenseeBetLimits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LicenseeCurrencies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LicenseeGameProviderConfigurations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LicenseeGames",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LicenseeLanguages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LocalizedStrings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PlayerBanks",
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
                name: "StationLetterPlayers",
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
                name: "StationLetters",
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
                name: "Languages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Licensees",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "VipLevels",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Resellers",
                schema: "dbo");
        }
    }
}
