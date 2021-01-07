using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "BrandPaymentAuditLogs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandPaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeforeChannelName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    AfterChannelName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    BeforePaymentStatus = table.Column<int>(type: "int", nullable: false),
                    AfterPaymentStatus = table.Column<int>(type: "int", nullable: false),
                    BeforeTransferLimitFrom = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    AfterTransferLimitFrom = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    BeforeTransferLimitTo = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    AfterTransferLimitTo = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    BeforePlayerListCondition = table.Column<int>(type: "int", nullable: false),
                    AfterPlayerListCondition = table.Column<int>(type: "int", nullable: false),
                    BeforeAllPlayers = table.Column<bool>(type: "bit", nullable: false),
                    AfterAllPlayers = table.Column<bool>(type: "bit", nullable: false),
                    BeforeIdentityAuthentication = table.Column<bool>(type: "bit", nullable: false),
                    AfterIdentityAuthentication = table.Column<bool>(type: "bit", nullable: false),
                    BeforePhoneAuthentication = table.Column<bool>(type: "bit", nullable: false),
                    AfterPhoneAuthentication = table.Column<bool>(type: "bit", nullable: false),
                    BeforeDepositedAmountUnlimited = table.Column<bool>(type: "bit", nullable: false),
                    AfterDepositedAmountUnlimited = table.Column<bool>(type: "bit", nullable: false),
                    BeforeDepositedAmountMin = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    AfterDepositedAmountMin = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    BeforeDepositedAmountMax = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    AfterDepositedAmountMax = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(4000)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Operation = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandPaymentAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                schema: "dbo",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", nullable: false),
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    AutoCloseRound = table.Column<bool>(type: "bit", nullable: false),
                    SupportTestPlayer = table.Column<bool>(type: "bit", nullable: false),
                    AllowKickPlayer = table.Column<bool>(type: "bit", nullable: false),
                    PlaceBetTimeout = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    SettleBetTimeout = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    UniqueId = table.Column<short>(type: "smallint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviders", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                schema: "dbo",
                columns: table => new
                {
                    CultureCode = table.Column<string>(type: "nvarchar(6)", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    LocalizedName = table.Column<string>(type: "nvarchar(64)", nullable: false),
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
                name: "Payments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerAuditLogs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleName = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    Operation = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resellers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    OperationalEmail = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    BusinessType = table.Column<byte>(type: "tinyint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resellers", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleType = table.Column<byte>(type: "tinyint", nullable: false),
                    ResellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LicenseeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsRoot = table.Column<bool>(type: "bit", nullable: false),
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
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "BetLimits",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    MinBet = table.Column<int>(type: "int", nullable: false),
                    MaxBet = table.Column<int>(type: "int", nullable: false),
                    MaxBetPerMatch = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BetLimits", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_BetLimits_Currencies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameProviderBetLimits",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: true),
                    MinBet = table.Column<int>(type: "int", nullable: false),
                    MaxBet = table.Column<int>(type: "int", nullable: false),
                    MaxBetPerMatch = table.Column<int>(type: "int", nullable: true),
                    ExternalSetting = table.Column<string>(type: "nvarchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviderBetLimits", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    ClientId = table.Column<string>(type: "varchar(128)", nullable: false),
                    SecretKey = table.Column<string>(type: "varchar(64)", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DesktopEndpoint = table.Column<string>(type: "nvarchar(1024)", nullable: false),
                    MobileEndpoint = table.Column<string>(type: "nvarchar(1024)", nullable: false),
                    ConfigurationData = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
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
                    GameProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    Id = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    EndpointPath = table.Column<string>(type: "nvarchar(2048)", nullable: true),
                    RuleUrl = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    GameProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    PlatformType = table.Column<byte>(type: "tinyint", nullable: false),
                    SubGame = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HasTurnoverOnTieBet = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Html5 = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AvailableCurrencies = table.Column<string>(type: "nvarchar(1024)", nullable: false, defaultValue: ""),
                    Code = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
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
                name: "MarketTypes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketTypes", x => x.Id);
                    table.UniqueConstraint("AK_MarketTypes_GameProviderId_Code", x => new { x.GameProviderId, x.Code });
                    table.ForeignKey(
                        name: "FK_MarketTypes_GameProviders_Id",
                        column: x => x.Id,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SportTypes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportTypes", x => x.Id);
                    table.UniqueConstraint("AK_SportTypes_GameProviderId_Code", x => new { x.GameProviderId, x.Code });
                    table.ForeignKey(
                        name: "FK_SportTypes_GameProviders_Id",
                        column: x => x.Id,
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
                    GameProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CultureCode = table.Column<string>(type: "nvarchar(6)", nullable: false),
                    GameProviderCultureCode = table.Column<string>(type: "nvarchar(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviderLanguages", x => new { x.GameProviderId, x.CultureCode });
                    table.ForeignKey(
                        name: "FK_GameProviderLanguages_GameProviders_GameProviderId",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameProviderLanguages_Languages_CultureCode",
                        column: x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocalizedStrings",
                schema: "dbo",
                columns: table => new
                {
                    Context = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    CultureCode = table.Column<string>(type: "nvarchar(6)", nullable: false),
                    Translation = table.Column<string>(type: "nvarchar(1024)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizedStrings", x => new { x.Key, x.CultureCode, x.Context });
                    table.ForeignKey(
                        name: "FK_LocalizedStrings_Languages_CultureCode",
                        column: x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "dbo",
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Licensees",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    OperationalEmail = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    ResellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResellerCurrencies",
                schema: "dbo",
                columns: table => new
                {
                    ResellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: false)
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
                    ResellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    ResellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CultureCode = table.Column<string>(type: "nvarchar(6)", nullable: false)
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
                name: "ResellerBetLimits",
                schema: "dbo",
                columns: table => new
                {
                    BetLimitId = table.Column<int>(type: "int", nullable: false),
                    ResellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResellerBetLimits", x => new { x.ResellerId, x.BetLimitId });
                    table.ForeignKey(
                        name: "FK_ResellerBetLimits_BetLimits_BetLimitId",
                        column: x => x.BetLimitId,
                        principalSchema: "dbo",
                        principalTable: "BetLimits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResellerBetLimits_Resellers_ResellerId",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "Resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SportMarketTypes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SportTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarketTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportMarketTypes", x => x.Id);
                    table.UniqueConstraint("AK_SportMarketTypes_SportTypeId_MarketTypeId", x => new { x.SportTypeId, x.MarketTypeId });
                    table.ForeignKey(
                        name: "FK_SportMarketTypes_MarketTypes_MarketTypeId",
                        column: x => x.MarketTypeId,
                        principalSchema: "dbo",
                        principalTable: "MarketTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SportMarketTypes_SportTypes_SportTypeId",
                        column: x => x.SportTypeId,
                        principalSchema: "dbo",
                        principalTable: "SportTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentChannels",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentChannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentChannels_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "dbo",
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentChannels_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "dbo",
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    OperationalEmail = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    TimeZoneId = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    LicenseeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "varchar(128)", nullable: false),
                    SecretKey = table.Column<string>(type: "varchar(32)", nullable: false),
                    ApiKey = table.Column<string>(type: "varchar(64)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Brands_Licensees_LicenseeId",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "Licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LicenseeBetLimits",
                schema: "dbo",
                columns: table => new
                {
                    BetLimitId = table.Column<int>(type: "int", nullable: false),
                    LicenseeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseeBetLimits", x => new { x.LicenseeId, x.BetLimitId });
                    table.ForeignKey(
                        name: "FK_LicenseeBetLimits_BetLimits_BetLimitId",
                        column: x => x.BetLimitId,
                        principalSchema: "dbo",
                        principalTable: "BetLimits",
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
                    LicenseeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: false)
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
                name: "LicenseeGameProviders",
                schema: "dbo",
                columns: table => new
                {
                    LicenseeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseeGameProviders", x => new { x.LicenseeId, x.GameProviderId });
                    table.ForeignKey(
                        name: "FK_LicenseeGameProviders_GameProviders_GameProviderId",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LicenseeGameProviders_Licensees_LicenseeId",
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
                    LicenseeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CultureCode = table.Column<string>(type: "nvarchar(6)", nullable: false)
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
                name: "BrandPayments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    TransferLimitFrom = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    TransferLimitTo = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    PlayerListCondition = table.Column<int>(type: "int", nullable: false),
                    AllPlayers = table.Column<bool>(type: "bit", nullable: false),
                    IdentityAuthentication = table.Column<bool>(type: "bit", nullable: false),
                    PhoneAuthentication = table.Column<bool>(type: "bit", nullable: false),
                    DepositedAmountUnlimited = table.Column<bool>(type: "bit", nullable: false),
                    DepositedAmountMin = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    DepositedAmountMax = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(4000)", nullable: true),
                    FeePercentage = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    LowestFee = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    HighestFee = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandPayments_PaymentChannels_PaymentChannelId",
                        column: x => x.PaymentChannelId,
                        principalSchema: "dbo",
                        principalTable: "PaymentChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandPayments_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "dbo",
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandPayments_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "dbo",
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Announcements",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(4000)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    From = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    To = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DisplayOrder = table.Column<short>(type: "smallint", nullable: false),
                    VerifiedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Announcements_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandBetLimits",
                schema: "dbo",
                columns: table => new
                {
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BetLimitId = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandBetLimits", x => new { x.BrandId, x.BetLimitId })
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_BrandBetLimits_BetLimits_BetLimitId",
                        column: x => x.BetLimitId,
                        principalSchema: "dbo",
                        principalTable: "BetLimits",
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
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    BrandCurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "BrandGameProviders",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    GameProviderConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefaultLanguageCode = table.Column<string>(type: "nvarchar(6)", nullable: false),
                    DesktopQueryString = table.Column<string>(type: "nvarchar(512)", nullable: false, defaultValue: ""),
                    MobileQueryString = table.Column<string>(type: "nvarchar(512)", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandGameProviders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandGameProviders_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BrandGameProviders_GameProviderConfigurations_GameProviderConfigurationId",
                        column: x => x.GameProviderConfigurationId,
                        principalSchema: "dbo",
                        principalTable: "GameProviderConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BrandGameProviders_GameProviders_GameProviderId",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BrandGameProviders_Languages_DefaultLanguageCode",
                        column: x => x.DefaultLanguageCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandGames",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Tag = table.Column<short>(type: "smallint", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Rebate = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveBettingPercentage = table.Column<decimal>(type: "decimal(18,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandGames", x => x.Id);
                    table.UniqueConstraint("AK_BrandGames_BrandId_GameId", x => new { x.BrandId, x.GameId });
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
                name: "BrandGroups",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSports = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandGroups", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_BrandGroups_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandLanguages",
                schema: "dbo",
                columns: table => new
                {
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CultureCode = table.Column<string>(type: "nvarchar(6)", nullable: false),
                    BrandCultureCode = table.Column<string>(type: "nvarchar(6)", nullable: false)
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
                name: "Departments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    ResellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LicenseeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Departments_Licensees_LicenseeId",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "Licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Departments_Resellers_ResellerId",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "Resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StationLetters",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(4000)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(512)", nullable: true),
                    AssignedAllPlayers = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    VerifiedByUsername = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationLetters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StationLetters_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VipLevels",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "varchar(16)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VipLevels", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_VipLevels_Brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Whitelists",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LicenseeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    InternalNote = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "BrandGameProviderSchedules",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandGameProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    From = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    To = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandGameProviderSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandGameProviderSchedules_BrandGameProviders_BrandGameProviderId",
                        column: x => x.BrandGameProviderId,
                        principalSchema: "dbo",
                        principalTable: "BrandGameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandGroupBetLimits",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BetLimitId = table.Column<int>(type: "int", nullable: false),
                    BrandGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandGroupBetLimits", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.UniqueConstraint("AK_BrandGroupBetLimits_BrandGroupId_BetLimitId", x => new { x.BrandGroupId, x.BetLimitId })
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_BrandGroupBetLimits_BetLimits_BetLimitId",
                        column: x => x.BetLimitId,
                        principalSchema: "dbo",
                        principalTable: "BetLimits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BrandGroupBetLimits_BrandGroups_BrandGroupId",
                        column: x => x.BrandGroupId,
                        principalSchema: "dbo",
                        principalTable: "BrandGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BrandGroupGames",
                schema: "dbo",
                columns: table => new
                {
                    BrandGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandGameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandGroupGames", x => new { x.BrandGroupId, x.BrandGameId });
                    table.ForeignKey(
                        name: "FK_BrandGroupGames_BrandGames_BrandGameId",
                        column: x => x.BrandGameId,
                        principalSchema: "dbo",
                        principalTable: "BrandGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BrandGroupGames_BrandGroups_BrandGroupId",
                        column: x => x.BrandGroupId,
                        principalSchema: "dbo",
                        principalTable: "BrandGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "BrandPaymentVipLevels",
                schema: "dbo",
                columns: table => new
                {
                    BrandPaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VipLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandPaymentVipLevels", x => new { x.BrandPaymentId, x.VipLevelId });
                    table.ForeignKey(
                        name: "FK_BrandPaymentVipLevels_BrandPayments_BrandPaymentId",
                        column: x => x.BrandPaymentId,
                        principalSchema: "dbo",
                        principalTable: "BrandPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandPaymentVipLevels_VipLevels_VipLevelId",
                        column: x => x.VipLevelId,
                        principalSchema: "dbo",
                        principalTable: "VipLevels",
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
                    RealName = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    InvitationCode = table.Column<string>(type: "nvarchar(6)", nullable: false),
                    Password = table.Column<byte[]>(type: "varbinary(128)", nullable: false),
                    Salt = table.Column<byte[]>(type: "varbinary(128)", nullable: false),
                    WithdrawPassword = table.Column<byte[]>(type: "varbinary(128)", nullable: false),
                    WithdrawSalt = table.Column<byte[]>(type: "varbinary(128)", nullable: false),
                    DefaultWalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefaultWalletCurrencyCode = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisteredOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastActiveOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BetLimitId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    QQ = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    Wechat = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    HasAgent = table.Column<bool>(type: "bit", nullable: false),
                    IdentityAuthentication = table.Column<bool>(type: "bit", nullable: false),
                    PhoneAuthentication = table.Column<bool>(type: "bit", nullable: false),
                    AccumulatedDeposit = table.Column<decimal>(type: "decimal(18,6)", nullable: false, defaultValue: 0m),
                    ParentPlayerCode = table.Column<string>(type: "varchar(16)", nullable: true),
                    ParentInvitationCode = table.Column<string>(type: "nvarchar(6)", nullable: true),
                    VipId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerCode);
                    table.UniqueConstraint("AK_Players_BrandId_Username", x => new { x.BrandId, x.Username });
                    table.ForeignKey(
                        name: "FK_Players_BetLimits_BetLimitId",
                        column: x => x.BetLimitId,
                        principalSchema: "dbo",
                        principalTable: "BetLimits",
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
                        name: "FK_Players_Players_ParentPlayerCode",
                        column: x => x.ParentPlayerCode,
                        principalSchema: "dbo",
                        principalTable: "Players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Players_VipLevels_VipId",
                        column: x => x.VipId,
                        principalSchema: "dbo",
                        principalTable: "VipLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BrandGroupGameProviderBetLimits",
                schema: "dbo",
                columns: table => new
                {
                    BrandGroupBetLimitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameProviderBetLimitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandGroupGameProviderBetLimits", x => new { x.BrandGroupBetLimitId, x.GameProviderBetLimitId });
                    table.ForeignKey(
                        name: "FK_BrandGroupGameProviderBetLimits_BrandGroupBetLimits_BrandGroupBetLimitId",
                        column: x => x.BrandGroupBetLimitId,
                        principalSchema: "dbo",
                        principalTable: "BrandGroupBetLimits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandGroupGameProviderBetLimits_GameProviderBetLimits_GameProviderBetLimitId",
                        column: x => x.GameProviderBetLimitId,
                        principalSchema: "dbo",
                        principalTable: "GameProviderBetLimits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandPaymentPlayers",
                schema: "dbo",
                columns: table => new
                {
                    PlayerCode = table.Column<string>(type: "varchar(16)", nullable: false),
                    BrandPaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandPaymentPlayers", x => new { x.BrandPaymentId, x.PlayerCode });
                    table.ForeignKey(
                        name: "FK_BrandPaymentPlayers_BrandPayments_BrandPaymentId",
                        column: x => x.BrandPaymentId,
                        principalSchema: "dbo",
                        principalTable: "BrandPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandPaymentPlayers_Players_PlayerCode",
                        column: x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "Players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerBanks",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerCode = table.Column<string>(type: "varchar(16)", nullable: false),
                    RealName = table.Column<string>(type: "nvarchar(32)", nullable: false),
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerCode = table.Column<string>(type: "varchar(16)", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    PlatformType = table.Column<byte>(type: "tinyint", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "PlayerSportsBetLimits",
                schema: "dbo",
                columns: table => new
                {
                    PlayerCode = table.Column<string>(type: "varchar(16)", nullable: false),
                    SportMarketTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinBet = table.Column<int>(type: "int", nullable: false),
                    MaxBet = table.Column<int>(type: "int", nullable: false),
                    MaxBetPerMatch = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSportsBetLimits", x => new { x.PlayerCode, x.SportMarketTypeId })
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_PlayerSportsBetLimits_Players_PlayerCode",
                        column: x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "Players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerSportsBetLimits_SportMarketTypes_SportMarketTypeId",
                        column: x => x.SportMarketTypeId,
                        principalSchema: "dbo",
                        principalTable: "SportMarketTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StationLetterPlayers",
                schema: "dbo",
                columns: table => new
                {
                    StationLetterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerCode = table.Column<string>(type: "varchar(16)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_BrandId",
                schema: "dbo",
                table: "Announcements",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BetLimits_CurrencyCode_Id",
                schema: "dbo",
                table: "BetLimits",
                columns: new[] { "CurrencyCode", "Id" })
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_BrandBetLimits_BetLimitId",
                schema: "dbo",
                table: "BrandBetLimits",
                column: "BetLimitId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandCurrencies_CurrencyCode",
                schema: "dbo",
                table: "BrandCurrencies",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_BrandGameProviders_BrandId_GameProviderId",
                schema: "dbo",
                table: "BrandGameProviders",
                columns: new[] { "BrandId", "GameProviderId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrandGameProviders_DefaultLanguageCode",
                schema: "dbo",
                table: "BrandGameProviders",
                column: "DefaultLanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_BrandGameProviders_GameProviderConfigurationId",
                schema: "dbo",
                table: "BrandGameProviders",
                column: "GameProviderConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandGameProviders_GameProviderId",
                schema: "dbo",
                table: "BrandGameProviders",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandGameProviderSchedules_BrandGameProviderId",
                schema: "dbo",
                table: "BrandGameProviderSchedules",
                column: "BrandGameProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandGames_GameId",
                schema: "dbo",
                table: "BrandGames",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandGroupBetLimits_BetLimitId",
                schema: "dbo",
                table: "BrandGroupBetLimits",
                column: "BetLimitId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandGroupGameProviderBetLimits_GameProviderBetLimitId",
                schema: "dbo",
                table: "BrandGroupGameProviderBetLimits",
                column: "GameProviderBetLimitId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandGroupGames_BrandGameId",
                schema: "dbo",
                table: "BrandGroupGames",
                column: "BrandGameId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandGroups_BrandId",
                schema: "dbo",
                table: "BrandGroups",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandLanguages_CultureCode",
                schema: "dbo",
                table: "BrandLanguages",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                name: "IX_BrandPaymentAuditLogs_BrandPaymentId",
                schema: "dbo",
                table: "BrandPaymentAuditLogs",
                column: "BrandPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandPaymentPlayers_PlayerCode",
                schema: "dbo",
                table: "BrandPaymentPlayers",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                name: "IX_BrandPayments_PaymentChannelId",
                schema: "dbo",
                table: "BrandPayments",
                column: "PaymentChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandPayments_PaymentId",
                schema: "dbo",
                table: "BrandPayments",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandPayments_PaymentMethodId",
                schema: "dbo",
                table: "BrandPayments",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandPaymentVipLevels_VipLevelId",
                schema: "dbo",
                table: "BrandPaymentVipLevels",
                column: "VipLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_LicenseeId",
                schema: "dbo",
                table: "Brands",
                column: "LicenseeId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_BrandId",
                schema: "dbo",
                table: "Departments",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_LicenseeId",
                schema: "dbo",
                table: "Departments",
                column: "LicenseeId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ResellerId",
                schema: "dbo",
                table: "Departments",
                column: "ResellerId");

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
                column: "GameProviderId")
                .Annotation("SqlServer:Include", new[] { "Id", "Code", "Name", "ExternalId", "PlatformType", "Type", "Status", "AvailableCurrencies", "EndpointPath", "SubGame", "HasTurnoverOnTieBet", "RuleUrl", "Description" });

            migrationBuilder.CreateIndex(
                name: "IX_LicenseeBetLimits_BetLimitId",
                schema: "dbo",
                table: "LicenseeBetLimits",
                column: "BetLimitId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseeCurrencies_CurrencyCode",
                schema: "dbo",
                table: "LicenseeCurrencies",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseeGameProviders_GameProviderId",
                schema: "dbo",
                table: "LicenseeGameProviders",
                column: "GameProviderId");

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
                name: "IX_LocalizedStrings_CultureCode",
                schema: "dbo",
                table: "LocalizedStrings",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentChannels_PaymentId",
                schema: "dbo",
                table: "PaymentChannels",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentChannels_PaymentMethodId",
                schema: "dbo",
                table: "PaymentChannels",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_PaymentId",
                schema: "dbo",
                table: "PaymentMethods",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerBanks_PlayerCode",
                schema: "dbo",
                table: "PlayerBanks",
                column: "PlayerCode");

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
                name: "IX_Players_BetLimitId",
                schema: "dbo",
                table: "Players",
                column: "BetLimitId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_ParentPlayerCode",
                schema: "dbo",
                table: "Players",
                column: "ParentPlayerCode");

            migrationBuilder.CreateIndex(
                name: "IX_Players_VipId",
                schema: "dbo",
                table: "Players",
                column: "VipId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSportsBetLimits_SportMarketTypeId",
                schema: "dbo",
                table: "PlayerSportsBetLimits",
                column: "SportMarketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResellerBetLimits_BetLimitId",
                schema: "dbo",
                table: "ResellerBetLimits",
                column: "BetLimitId");

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
                name: "IX_SportMarketTypes_MarketTypeId",
                schema: "dbo",
                table: "SportMarketTypes",
                column: "MarketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StationLetterPlayers_PlayerCode",
                schema: "dbo",
                table: "StationLetterPlayers",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                name: "IX_StationLetters_BrandId",
                schema: "dbo",
                table: "StationLetters",
                column: "BrandId");

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

            migrationBuilder.CreateIndex(
                name: "IX_VipLevels_BrandId",
                schema: "dbo",
                table: "VipLevels",
                column: "BrandId");

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

            InitAfterRun(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Announcements",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandBetLimits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandCurrencies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandGameProviderSchedules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandGroupGameProviderBetLimits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandGroupGames",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandLanguages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandPaymentAuditLogs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandPaymentPlayers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandPaymentVipLevels",
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
                name: "LicenseeGameProviders",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LicenseeLanguages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LocalizedStrings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PlayerAuditLogs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PlayerBanks",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PlayerLoginRecords",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PlayerSportsBetLimits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ResellerBetLimits",
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
                name: "Users",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Wallets",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Whitelists",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandGameProviders",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandGroupBetLimits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "GameProviderBetLimits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandGames",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandPayments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SportMarketTypes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "StationLetters",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Departments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Players",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "GameProviderConfigurations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Languages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BrandGroups",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Games",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PaymentChannels",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MarketTypes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SportTypes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BetLimits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "VipLevels",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PaymentMethods",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "GameProviders",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Currencies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Brands",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Payments",
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
