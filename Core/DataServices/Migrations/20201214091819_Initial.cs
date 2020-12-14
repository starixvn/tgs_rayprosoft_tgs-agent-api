using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                "dbo");

            migrationBuilder.CreateTable(
                "Announcements",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Subject = table.Column<string>("nvarchar(256)", nullable: true),
                    Content = table.Column<string>("nvarchar(4000)", nullable: true),
                    Remarks = table.Column<string>("nvarchar(512)", nullable: true),
                    Status = table.Column<int>("int", nullable: false),
                    From = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    To = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    DisplayOrder = table.Column<short>("smallint", nullable: false),
                    VerifiedByUsername = table.Column<string>("nvarchar(32)", nullable: true),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "BetLimitGroups",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>("nvarchar(20)", nullable: false),
                    Name = table.Column<string>("nvarchar(255)", nullable: false),
                    Description = table.Column<string>("nvarchar(512)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BetLimitGroups", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                "Currencies",
                schema: "dbo",
                columns: table => new
                {
                    Code = table.Column<string>("nvarchar(16)", nullable: false),
                    Name = table.Column<string>("nvarchar(64)", nullable: false),
                    NativeName = table.Column<string>("nvarchar(64)", nullable: true),
                    Symbol = table.Column<string>("nvarchar(16)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Code);
                });

            migrationBuilder.CreateTable(
                "Departments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Name = table.Column<string>("nvarchar(50)", nullable: false),
                    Description = table.Column<string>("nvarchar(128)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "GameProviders",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Status = table.Column<short>("smallint", nullable: false),
                    Description = table.Column<string>("nvarchar(128)", nullable: true),
                    AutoCloseRound = table.Column<bool>("bit", nullable: false),
                    SupportTestPlayer = table.Column<bool>("bit", nullable: false),
                    AllowKickPlayer = table.Column<bool>("bit", nullable: false),
                    PlaceBetTimeout = table.Column<short>("smallint", nullable: false, defaultValue: (short) 0),
                    SettleBetTimeout = table.Column<short>("smallint", nullable: false, defaultValue: (short) 0),
                    UniqueId = table.Column<short>("smallint", nullable: false),
                    Code = table.Column<string>("nvarchar(16)", nullable: false),
                    Name = table.Column<string>("nvarchar(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviders", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                "Languages",
                schema: "dbo",
                columns: table => new
                {
                    CultureCode = table.Column<string>("nvarchar(6)", nullable: false),
                    LanguageCode = table.Column<string>("nvarchar(10)", nullable: false),
                    Name = table.Column<string>("nvarchar(64)", nullable: false),
                    LocalizedName = table.Column<string>("nvarchar(64)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.CultureCode);
                });

            migrationBuilder.CreateTable(
                "PaymentAuditLogs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    PaymentId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    BeforeChannelName = table.Column<string>("nvarchar(50)", nullable: true),
                    AfterChannelName = table.Column<string>("nvarchar(50)", nullable: false),
                    BeforePaymentStatus = table.Column<int>("int", nullable: false),
                    AfterPaymentStatus = table.Column<int>("int", nullable: false),
                    BeforeTransferLimitFrom = table.Column<decimal>("decimal(18,6)", nullable: false),
                    AfterTransferLimitFrom = table.Column<decimal>("decimal(18,6)", nullable: false),
                    BeforeTransferLimitTo = table.Column<decimal>("decimal(18,6)", nullable: false),
                    AfterTransferLimitTo = table.Column<decimal>("decimal(18,6)", nullable: false),
                    BeforePlayerListCondition = table.Column<int>("int", nullable: false),
                    AfterPlayerListCondition = table.Column<int>("int", nullable: false),
                    BeforeAllPlayers = table.Column<bool>("bit", nullable: false),
                    AfterAllPlayers = table.Column<bool>("bit", nullable: false),
                    BeforeIdentityAuthentication = table.Column<bool>("bit", nullable: false),
                    AfterIdentityAuthentication = table.Column<bool>("bit", nullable: false),
                    BeforePhoneAuthentication = table.Column<bool>("bit", nullable: false),
                    AfterPhoneAuthentication = table.Column<bool>("bit", nullable: false),
                    BeforeDepositedAmountUnlimited = table.Column<bool>("bit", nullable: false),
                    AfterDepositedAmountUnlimited = table.Column<bool>("bit", nullable: false),
                    BeforeDepositedAmountMin = table.Column<decimal>("decimal(18,6)", nullable: false),
                    AfterDepositedAmountMin = table.Column<decimal>("decimal(18,6)", nullable: false),
                    BeforeDepositedAmountMax = table.Column<decimal>("decimal(18,6)", nullable: false),
                    AfterDepositedAmountMax = table.Column<decimal>("decimal(18,6)", nullable: false),
                    Reason = table.Column<string>("nvarchar(4000)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    Operation = table.Column<short>("smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "Payments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Description = table.Column<string>("nvarchar(128)", nullable: true),
                    Code = table.Column<string>("nvarchar(50)", nullable: false),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "PlayerAuditLogs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    ModuleName = table.Column<string>("nvarchar(256)", nullable: false),
                    Username = table.Column<string>("nvarchar(32)", nullable: true),
                    Operation = table.Column<string>("nvarchar(256)", nullable: true),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "Resellers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Status = table.Column<byte>("tinyint", nullable: false),
                    Description = table.Column<string>("nvarchar(256)", nullable: true),
                    OperationalEmail = table.Column<string>("nvarchar(128)", nullable: true),
                    BusinessType = table.Column<byte>("tinyint", nullable: false),
                    Code = table.Column<string>("nvarchar(16)", nullable: false),
                    Name = table.Column<string>("nvarchar(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resellers", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                "Roles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    RoleType = table.Column<byte>("tinyint", nullable: false),
                    ResellerId = table.Column<Guid>("uniqueidentifier", nullable: true),
                    LicenseeId = table.Column<Guid>("uniqueidentifier", nullable: true),
                    BrandId = table.Column<Guid>("uniqueidentifier", nullable: true),
                    Name = table.Column<string>("nvarchar(50)", nullable: false),
                    DisplayName = table.Column<string>("nvarchar(50)", nullable: true),
                    MenuSetting = table.Column<string>("nvarchar(4000)", nullable: false),
                    Description = table.Column<string>("nvarchar(128)", nullable: true),
                    Status = table.Column<byte>("tinyint", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "Settings",
                schema: "dbo",
                columns: table => new
                {
                    Key = table.Column<string>("nvarchar(32)", nullable: false),
                    Value = table.Column<string>("nvarchar(4000)", nullable: false),
                    Description = table.Column<string>("nvarchar(256)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                "StationLetters",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Subject = table.Column<string>("nvarchar(256)", nullable: true),
                    Content = table.Column<string>("nvarchar(4000)", nullable: true),
                    Remarks = table.Column<string>("nvarchar(512)", nullable: true),
                    AssignedAllPlayers = table.Column<bool>("bit", nullable: false),
                    Status = table.Column<int>("int", nullable: false),
                    VerifiedByUsername = table.Column<string>("nvarchar(256)", nullable: true),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationLetters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "VipLevels",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Code = table.Column<string>("varchar(16)", nullable: false),
                    Name = table.Column<string>("nvarchar(32)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VipLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "GameProviderBetLimits",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Code = table.Column<string>("nvarchar(255)", nullable: false),
                    GameProviderId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    BetLimitGroupId = table.Column<int>("int", nullable: false),
                    CurrencyCode = table.Column<string>("nvarchar(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviderBetLimits", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        "FK_GameProviderBetLimits_BetLimitGroups_BetLimitGroupId",
                        x => x.BetLimitGroupId,
                        principalSchema: "dbo",
                        principalTable: "BetLimitGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_GameProviderBetLimits_Currencies_CurrencyCode",
                        x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_GameProviderBetLimits_GameProviders_GameProviderId",
                        x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "GameProviderConfigurations",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Name = table.Column<string>("nvarchar(255)", nullable: true),
                    Description = table.Column<string>("nvarchar(256)", nullable: true),
                    ClientId = table.Column<string>("varchar(128)", nullable: false),
                    SecretKey = table.Column<string>("varchar(64)", nullable: false),
                    Status = table.Column<int>("int", nullable: false),
                    GameProviderId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    DesktopEndpoint = table.Column<string>("nvarchar(1024)", nullable: false),
                    MobileEndpoint = table.Column<string>("nvarchar(1024)", nullable: false),
                    ConfigurationData = table.Column<string>("nvarchar(4000)", nullable: false),
                    IsDefault = table.Column<bool>("bit", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviderConfigurations", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        "FK_GameProviderConfigurations_GameProviders_GameProviderId",
                        x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "GameProviderCurrencies",
                schema: "dbo",
                columns: table => new
                {
                    GameProviderId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    CurrencyCode = table.Column<string>("nvarchar(16)", nullable: false),
                    GameProviderCurrencyCode = table.Column<string>("nvarchar(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviderCurrencies", x => new {x.GameProviderId, x.CurrencyCode});
                    table.ForeignKey(
                        "FK_GameProviderCurrencies_Currencies_CurrencyCode",
                        x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_GameProviderCurrencies_GameProviders_GameProviderId",
                        x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Games",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>("int", nullable: false),
                    Status = table.Column<byte>("tinyint", nullable: false),
                    ExternalId = table.Column<string>("nvarchar(32)", nullable: false),
                    Description = table.Column<string>("nvarchar(128)", nullable: true),
                    EndpointPath = table.Column<string>("nvarchar(2048)", nullable: true),
                    RuleUrl = table.Column<string>("nvarchar(512)", nullable: true),
                    GameProviderId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Type = table.Column<byte>("tinyint", nullable: false),
                    PlatformType = table.Column<byte>("tinyint", nullable: false),
                    SubGame = table.Column<bool>("bit", nullable: false, defaultValue: false),
                    HasTurnoverOnTieBet = table.Column<bool>("bit", nullable: false, defaultValue: false),
                    AvailableCurrencies = table.Column<string>("nvarchar(1024)", nullable: false, defaultValue: ""),
                    Code = table.Column<string>("nvarchar(128)", nullable: false),
                    Name = table.Column<string>("nvarchar(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        "FK_Games_GameProviders_GameProviderId",
                        x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "GameProviderLanguages",
                schema: "dbo",
                columns: table => new
                {
                    GameProviderId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    CultureCode = table.Column<string>("nvarchar(6)", nullable: false),
                    GameProviderCultureCode = table.Column<string>("nvarchar(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameProviderLanguages", x => new {x.GameProviderId, x.CultureCode});
                    table.ForeignKey(
                        "FK_GameProviderLanguages_GameProviders_GameProviderId",
                        x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_GameProviderLanguages_Languages_CultureCode",
                        x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "LocalizedStrings",
                schema: "dbo",
                columns: table => new
                {
                    Context = table.Column<string>("nvarchar(64)", nullable: false),
                    Key = table.Column<string>("nvarchar(64)", nullable: false),
                    CultureCode = table.Column<string>("nvarchar(6)", nullable: false),
                    Translation = table.Column<string>("nvarchar(1024)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizedStrings", x => new {x.Key, x.CultureCode, x.Context});
                    table.ForeignKey(
                        "FK_LocalizedStrings_Languages_CultureCode",
                        x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "PaymentMethods",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Description = table.Column<string>("nvarchar(128)", nullable: true),
                    PaymentVendorId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Code = table.Column<string>("nvarchar(50)", nullable: false),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                    table.ForeignKey(
                        "FK_PaymentMethods_Payments_PaymentVendorId",
                        x => x.PaymentVendorId,
                        principalSchema: "dbo",
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Licensees",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Status = table.Column<byte>("tinyint", nullable: false),
                    Description = table.Column<string>("nvarchar(256)", nullable: true),
                    OperationalEmail = table.Column<string>("nvarchar(128)", nullable: true),
                    ResellerId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Code = table.Column<string>("nvarchar(16)", nullable: false),
                    Name = table.Column<string>("nvarchar(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licensees", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        "FK_Licensees_Resellers_ResellerId",
                        x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "Resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "ResellerBetLimits",
                schema: "dbo",
                columns: table => new
                {
                    BetLimitGroupId = table.Column<int>("int", nullable: false),
                    ResellerId = table.Column<Guid>("uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResellerBetLimits", x => new {x.ResellerId, x.BetLimitGroupId});
                    table.ForeignKey(
                        "FK_ResellerBetLimits_BetLimitGroups_BetLimitGroupId",
                        x => x.BetLimitGroupId,
                        principalSchema: "dbo",
                        principalTable: "BetLimitGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_ResellerBetLimits_Resellers_ResellerId",
                        x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "Resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "ResellerCurrencies",
                schema: "dbo",
                columns: table => new
                {
                    ResellerId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    CurrencyCode = table.Column<string>("nvarchar(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResellerCurrencies", x => new {x.ResellerId, x.CurrencyCode});
                    table.ForeignKey(
                        "FK_ResellerCurrencies_Currencies_CurrencyCode",
                        x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_ResellerCurrencies_Resellers_ResellerId",
                        x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "Resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "ResellerGameProviders",
                schema: "dbo",
                columns: table => new
                {
                    ResellerId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    GameProviderId = table.Column<Guid>("uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResellerGameProviders", x => new {x.ResellerId, x.GameProviderId});
                    table.ForeignKey(
                        "FK_ResellerGameProviders_GameProviders_GameProviderId",
                        x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_ResellerGameProviders_Resellers_ResellerId",
                        x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "Resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "ResellerLanguages",
                schema: "dbo",
                columns: table => new
                {
                    ResellerId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    CultureCode = table.Column<string>("nvarchar(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResellerLanguages", x => new {x.ResellerId, x.CultureCode});
                    table.ForeignKey(
                        "FK_ResellerLanguages_Languages_CultureCode",
                        x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_ResellerLanguages_Resellers_ResellerId",
                        x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "Resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Users",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Status = table.Column<byte>("tinyint", nullable: false),
                    RoleId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Username = table.Column<string>("nvarchar(50)", nullable: false),
                    LastLoginIp = table.Column<string>("nvarchar(128)", nullable: true),
                    LastLoginTime = table.Column<DateTimeOffset>("datetimeoffset", nullable: true),
                    Name = table.Column<string>("nvarchar(30)", nullable: false),
                    TimeZone = table.Column<string>("nvarchar(256)", nullable: true),
                    Password = table.Column<byte[]>("varbinary(128)", nullable: false),
                    Salt = table.Column<byte[]>("varbinary(128)", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        "FK_Users_Departments_DepartmentId",
                        x => x.DepartmentId,
                        principalSchema: "dbo",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Users_Roles_RoleId",
                        x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "PaymentChannels",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Description = table.Column<string>("nvarchar(128)", nullable: true),
                    PaymentVendorId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    PaymentMethodId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Code = table.Column<string>("nvarchar(50)", nullable: false),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentChannels", x => x.Id);
                    table.ForeignKey(
                        "FK_PaymentChannels_PaymentMethods_PaymentMethodId",
                        x => x.PaymentMethodId,
                        principalSchema: "dbo",
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_PaymentChannels_Payments_PaymentVendorId",
                        x => x.PaymentVendorId,
                        principalSchema: "dbo",
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Brands",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Status = table.Column<byte>("tinyint", nullable: false),
                    Description = table.Column<string>("nvarchar(256)", nullable: true),
                    OperationalEmail = table.Column<string>("nvarchar(128)", nullable: true),
                    TimeZoneId = table.Column<string>("nvarchar(256)", nullable: true),
                    LicenseeId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>("varchar(128)", nullable: false),
                    SecretKey = table.Column<string>("varchar(32)", nullable: false),
                    ApiKey = table.Column<string>("varchar(64)", nullable: false),
                    Code = table.Column<string>("nvarchar(16)", nullable: false),
                    Name = table.Column<string>("nvarchar(255)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        "FK_Brands_Licensees_LicenseeId",
                        x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "Licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "LicenseeBetLimits",
                schema: "dbo",
                columns: table => new
                {
                    BetLimitGroupId = table.Column<int>("int", nullable: false),
                    LicenseeId = table.Column<Guid>("uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseeBetLimits", x => new {x.LicenseeId, x.BetLimitGroupId});
                    table.ForeignKey(
                        "FK_LicenseeBetLimits_BetLimitGroups_BetLimitGroupId",
                        x => x.BetLimitGroupId,
                        principalSchema: "dbo",
                        principalTable: "BetLimitGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_LicenseeBetLimits_Licensees_LicenseeId",
                        x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "Licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "LicenseeCurrencies",
                schema: "dbo",
                columns: table => new
                {
                    LicenseeId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    CurrencyCode = table.Column<string>("nvarchar(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseeCurrencies", x => new {x.LicenseeId, x.CurrencyCode});
                    table.ForeignKey(
                        "FK_LicenseeCurrencies_Currencies_CurrencyCode",
                        x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_LicenseeCurrencies_Licensees_LicenseeId",
                        x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "Licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "LicenseeGameProviders",
                schema: "dbo",
                columns: table => new
                {
                    LicenseeId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    GameProviderId = table.Column<Guid>("uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseeGameProviders", x => new {x.LicenseeId, x.GameProviderId});
                    table.ForeignKey(
                        "FK_LicenseeGameProviders_GameProviders_GameProviderId",
                        x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_LicenseeGameProviders_Licensees_LicenseeId",
                        x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "Licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "LicenseeLanguages",
                schema: "dbo",
                columns: table => new
                {
                    LicenseeId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    CultureCode = table.Column<string>("nvarchar(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseeLanguages", x => new {x.LicenseeId, x.CultureCode});
                    table.ForeignKey(
                        "FK_LicenseeLanguages_Languages_CultureCode",
                        x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_LicenseeLanguages_Licensees_LicenseeId",
                        x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "Licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "BrandPayments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    DisplayOrder = table.Column<int>("int", nullable: false),
                    PaymentVendorId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    PaymentMethodId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    PaymentChannelId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    TransferType = table.Column<int>("int", nullable: false),
                    ChannelName = table.Column<string>("nvarchar(50)", nullable: false),
                    PaymentStatus = table.Column<int>("int", nullable: false),
                    TransferLimitFrom = table.Column<decimal>("decimal(18,6)", nullable: false),
                    TransferLimitTo = table.Column<decimal>("decimal(18,6)", nullable: false),
                    PlayerListCondition = table.Column<int>("int", nullable: false),
                    AllPlayers = table.Column<bool>("bit", nullable: false),
                    IdentityAuthentication = table.Column<bool>("bit", nullable: false),
                    PhoneAuthentication = table.Column<bool>("bit", nullable: false),
                    DepositedAmountUnlimited = table.Column<bool>("bit", nullable: false),
                    DepositedAmountMin = table.Column<decimal>("decimal(18,6)", nullable: false),
                    DepositedAmountMax = table.Column<decimal>("decimal(18,6)", nullable: false),
                    Reason = table.Column<string>("nvarchar(4000)", nullable: true),
                    FeePercentage = table.Column<decimal>("decimal(18,6)", nullable: true),
                    LowestFee = table.Column<decimal>("decimal(18,6)", nullable: true),
                    HighestFee = table.Column<decimal>("decimal(18,6)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandPayments", x => x.Id);
                    table.ForeignKey(
                        "FK_BrandPayments_PaymentChannels_PaymentChannelId",
                        x => x.PaymentChannelId,
                        principalSchema: "dbo",
                        principalTable: "PaymentChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_BrandPayments_PaymentMethods_PaymentMethodId",
                        x => x.PaymentMethodId,
                        principalSchema: "dbo",
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_BrandPayments_Payments_PaymentVendorId",
                        x => x.PaymentVendorId,
                        principalSchema: "dbo",
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "BrandBetLimits",
                schema: "dbo",
                columns: table => new
                {
                    BetLimitGroupId = table.Column<int>("int", nullable: false),
                    BrandId = table.Column<Guid>("uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandBetLimits", x => new {x.BrandId, x.BetLimitGroupId});
                    table.ForeignKey(
                        "FK_BrandBetLimits_BetLimitGroups_BetLimitGroupId",
                        x => x.BetLimitGroupId,
                        principalSchema: "dbo",
                        principalTable: "BetLimitGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_BrandBetLimits_Brands_BrandId",
                        x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "BrandCurrencies",
                schema: "dbo",
                columns: table => new
                {
                    BrandId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    CurrencyCode = table.Column<string>("nvarchar(16)", nullable: false),
                    BrandCurrencyCode = table.Column<string>("nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandCurrencies", x => new {x.BrandId, x.CurrencyCode});
                    table.ForeignKey(
                        "FK_BrandCurrencies_Brands_BrandId",
                        x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_BrandCurrencies_Currencies_CurrencyCode",
                        x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "BrandGameProviders",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    BrandId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    GameProviderId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Status = table.Column<int>("int", nullable: false),
                    GameProviderConfigurationId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    DefaultLanguageCode = table.Column<string>("nvarchar(6)", nullable: false),
                    DesktopQueryString = table.Column<string>("nvarchar(512)", nullable: false, defaultValue: ""),
                    MobileQueryString = table.Column<string>("nvarchar(512)", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandGameProviders", x => x.Id);
                    table.ForeignKey(
                        "FK_BrandGameProviders_Brands_BrandId",
                        x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_BrandGameProviders_GameProviderConfigurations_GameProviderConfigurationId",
                        x => x.GameProviderConfigurationId,
                        principalSchema: "dbo",
                        principalTable: "GameProviderConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_BrandGameProviders_GameProviders_GameProviderId",
                        x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "GameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_BrandGameProviders_Languages_DefaultLanguageCode",
                        x => x.DefaultLanguageCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "BrandGames",
                schema: "dbo",
                columns: table => new
                {
                    GameId = table.Column<int>("int", nullable: false),
                    BrandId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Status = table.Column<int>("int", nullable: false),
                    Tag = table.Column<short>("smallint", nullable: false),
                    DisplayOrder = table.Column<int>("int", nullable: false),
                    Rebate = table.Column<bool>("bit", nullable: false),
                    EffectiveBettingPercentage = table.Column<decimal>("decimal(18,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandGames", x => new {x.BrandId, x.GameId});
                    table.ForeignKey(
                        "FK_BrandGames_Brands_BrandId",
                        x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_BrandGames_Games_GameId",
                        x => x.GameId,
                        principalSchema: "dbo",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "BrandLanguages",
                schema: "dbo",
                columns: table => new
                {
                    BrandId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    CultureCode = table.Column<string>("nvarchar(6)", nullable: false),
                    BrandCultureCode = table.Column<string>("nvarchar(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandLanguages", x => new {x.BrandId, x.CultureCode});
                    table.ForeignKey(
                        "FK_BrandLanguages_Brands_BrandId",
                        x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_BrandLanguages_Languages_CultureCode",
                        x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Players",
                schema: "dbo",
                columns: table => new
                {
                    PlayerCode = table.Column<string>("varchar(16)", nullable: false),
                    Username = table.Column<string>("nvarchar(32)", nullable: false),
                    RealName = table.Column<string>("nvarchar(32)", nullable: true),
                    InvitationCode = table.Column<string>("nvarchar(6)", nullable: false),
                    Password = table.Column<byte[]>("varbinary(128)", nullable: false),
                    Salt = table.Column<byte[]>("varbinary(128)", nullable: false),
                    CultureCode = table.Column<string>("nvarchar(6)", nullable: false),
                    DefaultWalletId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    DefaultWalletCurrencyCode = table.Column<string>("nvarchar(16)", nullable: false),
                    BrandId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    RegisteredOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    LastActiveOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    Status = table.Column<int>("int", nullable: false),
                    BetLimitGroupId = table.Column<int>("int", nullable: false),
                    Type = table.Column<byte>("tinyint", nullable: false),
                    Email = table.Column<string>("nvarchar(64)", nullable: true),
                    Phone = table.Column<string>("nvarchar(32)", nullable: true),
                    QQ = table.Column<string>("nvarchar(64)", nullable: true),
                    Wechat = table.Column<string>("nvarchar(64)", nullable: true),
                    HasAgent = table.Column<bool>("bit", nullable: false),
                    IdentityAuthentication = table.Column<bool>("bit", nullable: false),
                    PhoneAuthentication = table.Column<bool>("bit", nullable: false),
                    AccumulatedDeposit = table.Column<decimal>("decimal(18,6)", nullable: false, defaultValue: 0m),
                    ParentPlayerCode = table.Column<string>("varchar(16)", nullable: true),
                    ParentInvitationCode = table.Column<string>("nvarchar(6)", nullable: true),
                    VipId = table.Column<Guid>("uniqueidentifier", nullable: true),
                    UpdatedByUsername = table.Column<string>("nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerCode);
                    table.ForeignKey(
                        "FK_Players_BetLimitGroups_BetLimitGroupId",
                        x => x.BetLimitGroupId,
                        principalSchema: "dbo",
                        principalTable: "BetLimitGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Players_Brands_BrandId",
                        x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Players_Languages_CultureCode",
                        x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "Languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Players_Players_ParentPlayerCode",
                        x => x.ParentPlayerCode,
                        principalSchema: "dbo",
                        principalTable: "Players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Players_VipLevels_VipId",
                        x => x.VipId,
                        principalSchema: "dbo",
                        principalTable: "VipLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Whitelists",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    ResellerId = table.Column<Guid>("uniqueidentifier", nullable: true),
                    LicenseeId = table.Column<Guid>("uniqueidentifier", nullable: true),
                    BrandId = table.Column<Guid>("uniqueidentifier", nullable: true),
                    IpAddress = table.Column<string>("nvarchar(64)", nullable: false),
                    Description = table.Column<string>("nvarchar(256)", nullable: false),
                    InternalNote = table.Column<string>("nvarchar(256)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    CreatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    UpdatedByUsername = table.Column<string>("nvarchar(32)", nullable: false),
                    Type = table.Column<short>("smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Whitelists", x => x.Id);
                    table.ForeignKey(
                        "FK_Whitelists_Brands_BrandId",
                        x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Whitelists_Licensees_LicenseeId",
                        x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "Licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Whitelists_Resellers_ResellerId",
                        x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "Resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "PaymentVipLevels",
                schema: "dbo",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    VipLevelId = table.Column<Guid>("uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentVipLevels", x => new {x.PaymentId, x.VipLevelId});
                    table.ForeignKey(
                        "FK_PaymentVipLevels_BrandPayments_PaymentId",
                        x => x.PaymentId,
                        principalSchema: "dbo",
                        principalTable: "BrandPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_PaymentVipLevels_VipLevels_VipLevelId",
                        x => x.VipLevelId,
                        principalSchema: "dbo",
                        principalTable: "VipLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "BrandGameProviderSchedules",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    BrandGameProviderId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    From = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    To = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    Status = table.Column<byte>("tinyint", nullable: false),
                    DayOfWeek = table.Column<int>("int", nullable: true),
                    IsDefault = table.Column<bool>("bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandGameProviderSchedules", x => x.Id);
                    table.ForeignKey(
                        "FK_BrandGameProviderSchedules_BrandGameProviders_BrandGameProviderId",
                        x => x.BrandGameProviderId,
                        principalSchema: "dbo",
                        principalTable: "BrandGameProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "PaymentPlayerList",
                schema: "dbo",
                columns: table => new
                {
                    PlayerCode = table.Column<string>("varchar(16)", nullable: false),
                    PaymentId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    ListType = table.Column<int>("int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentPlayerList", x => new {x.PaymentId, x.PlayerCode});
                    table.ForeignKey(
                        "FK_PaymentPlayerList_BrandPayments_PaymentId",
                        x => x.PaymentId,
                        principalSchema: "dbo",
                        principalTable: "BrandPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_PaymentPlayerList_Players_PlayerCode",
                        x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "Players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "PlayerBanks",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    PlayerCode = table.Column<string>("varchar(16)", nullable: false),
                    RealName = table.Column<string>("nvarchar(32)", nullable: false),
                    BankName = table.Column<string>("nvarchar(32)", nullable: false),
                    BankAccount = table.Column<string>("nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerBanks", x => x.Id);
                    table.ForeignKey(
                        "FK_PlayerBanks_Players_PlayerCode",
                        x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "Players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "PlayerLoginRecords",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    PlayerCode = table.Column<string>("varchar(16)", nullable: false),
                    BrandId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Status = table.Column<short>("smallint", nullable: false),
                    PlatformType = table.Column<byte>("tinyint", nullable: false),
                    IpAddress = table.Column<string>("nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    ExpiredOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerLoginRecords", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        "FK_PlayerLoginRecords_Brands_BrandId",
                        x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_PlayerLoginRecords_Players_PlayerCode",
                        x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "Players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "StationLetterPlayers",
                schema: "dbo",
                columns: table => new
                {
                    StationLetterId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    PlayerCode = table.Column<string>("varchar(16)", nullable: false),
                    IsRead = table.Column<bool>("bit", nullable: false),
                    ReadOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationLetterPlayers", x => new {x.StationLetterId, x.PlayerCode});
                    table.ForeignKey(
                        "FK_StationLetterPlayers_Players_PlayerCode",
                        x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "Players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_StationLetterPlayers_StationLetters_StationLetterId",
                        x => x.StationLetterId,
                        principalSchema: "dbo",
                        principalTable: "StationLetters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Wallets",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    PlayerCode = table.Column<string>("varchar(16)", nullable: false),
                    CurrencyCode = table.Column<string>("nvarchar(16)", nullable: false),
                    ExternalCurrencyCode = table.Column<string>("nvarchar(16)", nullable: false),
                    Balance = table.Column<decimal>("decimal(18,6)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>("datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        "FK_Wallets_Currencies_CurrencyCode",
                        x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Wallets_Players_PlayerCode",
                        x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "Players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_BrandBetLimits_BetLimitGroupId",
                schema: "dbo",
                table: "BrandBetLimits",
                column: "BetLimitGroupId");

            migrationBuilder.CreateIndex(
                "IX_BrandCurrencies_CurrencyCode",
                schema: "dbo",
                table: "BrandCurrencies",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                "IX_BrandGameProviders_BrandId_GameProviderId",
                schema: "dbo",
                table: "BrandGameProviders",
                columns: new[] {"BrandId", "GameProviderId"},
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_BrandGameProviders_DefaultLanguageCode",
                schema: "dbo",
                table: "BrandGameProviders",
                column: "DefaultLanguageCode");

            migrationBuilder.CreateIndex(
                "IX_BrandGameProviders_GameProviderConfigurationId",
                schema: "dbo",
                table: "BrandGameProviders",
                column: "GameProviderConfigurationId");

            migrationBuilder.CreateIndex(
                "IX_BrandGameProviders_GameProviderId",
                schema: "dbo",
                table: "BrandGameProviders",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                "IX_BrandGameProviderSchedules_BrandGameProviderId",
                schema: "dbo",
                table: "BrandGameProviderSchedules",
                column: "BrandGameProviderId");

            migrationBuilder.CreateIndex(
                "IX_BrandGames_GameId",
                schema: "dbo",
                table: "BrandGames",
                column: "GameId");

            migrationBuilder.CreateIndex(
                "IX_BrandLanguages_CultureCode",
                schema: "dbo",
                table: "BrandLanguages",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                "IX_BrandPayments_PaymentChannelId",
                schema: "dbo",
                table: "BrandPayments",
                column: "PaymentChannelId");

            migrationBuilder.CreateIndex(
                "IX_BrandPayments_PaymentMethodId",
                schema: "dbo",
                table: "BrandPayments",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                "IX_BrandPayments_PaymentVendorId",
                schema: "dbo",
                table: "BrandPayments",
                column: "PaymentVendorId");

            migrationBuilder.CreateIndex(
                "IX_Brands_LicenseeId",
                schema: "dbo",
                table: "Brands",
                column: "LicenseeId");

            migrationBuilder.CreateIndex(
                "IX_GameProviderBetLimits_BetLimitGroupId",
                schema: "dbo",
                table: "GameProviderBetLimits",
                column: "BetLimitGroupId");

            migrationBuilder.CreateIndex(
                "IX_GameProviderBetLimits_CurrencyCode",
                schema: "dbo",
                table: "GameProviderBetLimits",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                "IX_GameProviderBetLimits_GameProviderId",
                schema: "dbo",
                table: "GameProviderBetLimits",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                "IX_GameProviderConfigurations_GameProviderId",
                schema: "dbo",
                table: "GameProviderConfigurations",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                "IX_GameProviderCurrencies_CurrencyCode",
                schema: "dbo",
                table: "GameProviderCurrencies",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                "IX_GameProviderLanguages_CultureCode",
                schema: "dbo",
                table: "GameProviderLanguages",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                    "IX_Games_GameProviderId",
                    schema: "dbo",
                    table: "Games",
                    column: "GameProviderId")
                .Annotation("SqlServer:Include",
                    new[]
                    {
                        "Id", "Code", "Name", "ExternalId", "PlatformType", "Type", "Status", "AvailableCurrencies",
                        "EndpointPath", "SubGame", "HasTurnoverOnTieBet", "RuleUrl", "Description"
                    });

            migrationBuilder.CreateIndex(
                "IX_LicenseeBetLimits_BetLimitGroupId",
                schema: "dbo",
                table: "LicenseeBetLimits",
                column: "BetLimitGroupId");

            migrationBuilder.CreateIndex(
                "IX_LicenseeCurrencies_CurrencyCode",
                schema: "dbo",
                table: "LicenseeCurrencies",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                "IX_LicenseeGameProviders_GameProviderId",
                schema: "dbo",
                table: "LicenseeGameProviders",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                "IX_LicenseeLanguages_CultureCode",
                schema: "dbo",
                table: "LicenseeLanguages",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                "IX_Licensees_ResellerId",
                schema: "dbo",
                table: "Licensees",
                column: "ResellerId");

            migrationBuilder.CreateIndex(
                "IX_LocalizedStrings_CultureCode",
                schema: "dbo",
                table: "LocalizedStrings",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                "IX_PaymentAuditLogs_PaymentId",
                schema: "dbo",
                table: "PaymentAuditLogs",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                "IX_PaymentChannels_PaymentMethodId",
                schema: "dbo",
                table: "PaymentChannels",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                "IX_PaymentChannels_PaymentVendorId",
                schema: "dbo",
                table: "PaymentChannels",
                column: "PaymentVendorId");

            migrationBuilder.CreateIndex(
                "IX_PaymentMethods_PaymentVendorId",
                schema: "dbo",
                table: "PaymentMethods",
                column: "PaymentVendorId");

            migrationBuilder.CreateIndex(
                "IX_PaymentPlayerList_PlayerCode",
                schema: "dbo",
                table: "PaymentPlayerList",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                "IX_PaymentVipLevels_VipLevelId",
                schema: "dbo",
                table: "PaymentVipLevels",
                column: "VipLevelId");

            migrationBuilder.CreateIndex(
                "IX_PlayerBanks_PlayerCode",
                schema: "dbo",
                table: "PlayerBanks",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                "IX_PlayerLoginRecords_BrandId",
                schema: "dbo",
                table: "PlayerLoginRecords",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                "IX_PlayerLoginRecords_PlayerCode",
                schema: "dbo",
                table: "PlayerLoginRecords",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                "IX_Players_BetLimitGroupId",
                schema: "dbo",
                table: "Players",
                column: "BetLimitGroupId");

            migrationBuilder.CreateIndex(
                "IX_Players_CultureCode",
                schema: "dbo",
                table: "Players",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                "IX_Players_ParentPlayerCode",
                schema: "dbo",
                table: "Players",
                column: "ParentPlayerCode");

            migrationBuilder.CreateIndex(
                "IX_Players_VipId",
                schema: "dbo",
                table: "Players",
                column: "VipId");

            migrationBuilder.CreateIndex(
                "UX_Players_BrandId_Username",
                schema: "dbo",
                table: "Players",
                columns: new[] {"BrandId", "Username"},
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_ResellerBetLimits_BetLimitGroupId",
                schema: "dbo",
                table: "ResellerBetLimits",
                column: "BetLimitGroupId");

            migrationBuilder.CreateIndex(
                "IX_ResellerCurrencies_CurrencyCode",
                schema: "dbo",
                table: "ResellerCurrencies",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                "IX_ResellerGameProviders_GameProviderId",
                schema: "dbo",
                table: "ResellerGameProviders",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                "IX_ResellerLanguages_CultureCode",
                schema: "dbo",
                table: "ResellerLanguages",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                "IX_StationLetterPlayers_PlayerCode",
                schema: "dbo",
                table: "StationLetterPlayers",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                "IX_Users_DepartmentId",
                schema: "dbo",
                table: "Users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                "IX_Users_RoleId",
                schema: "dbo",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                "IX_Wallets_CurrencyCode",
                schema: "dbo",
                table: "Wallets",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                "IX_Wallets_PlayerCode",
                schema: "dbo",
                table: "Wallets",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                "IX_Whitelists_BrandId",
                schema: "dbo",
                table: "Whitelists",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                "IX_Whitelists_LicenseeId",
                schema: "dbo",
                table: "Whitelists",
                column: "LicenseeId");

            migrationBuilder.CreateIndex(
                "IX_Whitelists_ResellerId",
                schema: "dbo",
                table: "Whitelists",
                column: "ResellerId");

            InitAfterRun(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Announcements",
                "dbo");

            migrationBuilder.DropTable(
                "BrandBetLimits",
                "dbo");

            migrationBuilder.DropTable(
                "BrandCurrencies",
                "dbo");

            migrationBuilder.DropTable(
                "BrandGameProviderSchedules",
                "dbo");

            migrationBuilder.DropTable(
                "BrandGames",
                "dbo");

            migrationBuilder.DropTable(
                "BrandLanguages",
                "dbo");

            migrationBuilder.DropTable(
                "GameProviderBetLimits",
                "dbo");

            migrationBuilder.DropTable(
                "GameProviderCurrencies",
                "dbo");

            migrationBuilder.DropTable(
                "GameProviderLanguages",
                "dbo");

            migrationBuilder.DropTable(
                "LicenseeBetLimits",
                "dbo");

            migrationBuilder.DropTable(
                "LicenseeCurrencies",
                "dbo");

            migrationBuilder.DropTable(
                "LicenseeGameProviders",
                "dbo");

            migrationBuilder.DropTable(
                "LicenseeLanguages",
                "dbo");

            migrationBuilder.DropTable(
                "LocalizedStrings",
                "dbo");

            migrationBuilder.DropTable(
                "PaymentAuditLogs",
                "dbo");

            migrationBuilder.DropTable(
                "PaymentPlayerList",
                "dbo");

            migrationBuilder.DropTable(
                "PaymentVipLevels",
                "dbo");

            migrationBuilder.DropTable(
                "PlayerAuditLogs",
                "dbo");

            migrationBuilder.DropTable(
                "PlayerBanks",
                "dbo");

            migrationBuilder.DropTable(
                "PlayerLoginRecords",
                "dbo");

            migrationBuilder.DropTable(
                "ResellerBetLimits",
                "dbo");

            migrationBuilder.DropTable(
                "ResellerCurrencies",
                "dbo");

            migrationBuilder.DropTable(
                "ResellerGameProviders",
                "dbo");

            migrationBuilder.DropTable(
                "ResellerLanguages",
                "dbo");

            migrationBuilder.DropTable(
                "Settings",
                "dbo");

            migrationBuilder.DropTable(
                "StationLetterPlayers",
                "dbo");

            migrationBuilder.DropTable(
                "Users",
                "dbo");

            migrationBuilder.DropTable(
                "Wallets",
                "dbo");

            migrationBuilder.DropTable(
                "Whitelists",
                "dbo");

            migrationBuilder.DropTable(
                "BrandGameProviders",
                "dbo");

            migrationBuilder.DropTable(
                "Games",
                "dbo");

            migrationBuilder.DropTable(
                "BrandPayments",
                "dbo");

            migrationBuilder.DropTable(
                "StationLetters",
                "dbo");

            migrationBuilder.DropTable(
                "Departments",
                "dbo");

            migrationBuilder.DropTable(
                "Roles",
                "dbo");

            migrationBuilder.DropTable(
                "Currencies",
                "dbo");

            migrationBuilder.DropTable(
                "Players",
                "dbo");

            migrationBuilder.DropTable(
                "GameProviderConfigurations",
                "dbo");

            migrationBuilder.DropTable(
                "PaymentChannels",
                "dbo");

            migrationBuilder.DropTable(
                "BetLimitGroups",
                "dbo");

            migrationBuilder.DropTable(
                "Brands",
                "dbo");

            migrationBuilder.DropTable(
                "Languages",
                "dbo");

            migrationBuilder.DropTable(
                "VipLevels",
                "dbo");

            migrationBuilder.DropTable(
                "GameProviders",
                "dbo");

            migrationBuilder.DropTable(
                "PaymentMethods",
                "dbo");

            migrationBuilder.DropTable(
                "Licensees",
                "dbo");

            migrationBuilder.DropTable(
                "Payments",
                "dbo");

            migrationBuilder.DropTable(
                "Resellers",
                "dbo");
        }
    }
}