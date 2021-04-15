using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.EnsureSchema(
                name: "bet");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateTable(
                name: "brand_payment_audit_logs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Before = table.Column<string>(type: "jsonb", nullable: false),
                    After = table.Column<string>(type: "jsonb", nullable: false),
                    Reason = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Operation = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_payment_audit_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "currencies",
                schema: "dbo",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    NativeName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Symbol = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_currencies", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "game_providers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    AutoCloseRound = table.Column<bool>(type: "boolean", nullable: false),
                    SupportTestPlayer = table.Column<bool>(type: "boolean", nullable: false),
                    AllowKickPlayer = table.Column<bool>(type: "boolean", nullable: false),
                    PlaceBetTimeout = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    SettleBetTimeout = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    UniqueId = table.Column<short>(type: "smallint", nullable: false),
                    Code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_providers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "languages",
                schema: "dbo",
                columns: table => new
                {
                    CultureCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LocalizedName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_languages", x => x.CultureCode);
                });

            migrationBuilder.CreateTable(
                name: "payment_channels",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_channels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "payment_methods",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_methods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ExtendData = table.Column<string>(type: "jsonb", nullable: true),
                    Status = table.Column<short>(type: "smallint", nullable: false, defaultValueSql: "0"),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "Now()"),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "Now()"),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "player_audit_logs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Operation = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_audit_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "resellers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    OperationalEmail = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    BusinessType = table.Column<short>(type: "smallint", nullable: false),
                    Code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_resellers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleType = table.Column<short>(type: "smallint", nullable: false),
                    ResellerId = table.Column<Guid>(type: "uuid", nullable: true),
                    LicenseeId = table.Column<Guid>(type: "uuid", nullable: true),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsRoot = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MenuSetting = table.Column<string>(type: "jsonb", nullable: false),
                    Description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "rounds",
                schema: "bet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalRoundId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerCode = table.Column<string>(type: "text", nullable: false),
                    GameId = table.Column<int>(type: "integer", nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    GameProviderConfigurationId = table.Column<Guid>(type: "uuid", nullable: false),
                    WalletId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ClosedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalCreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExternalClosedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    BetAmount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    ValidBetAmount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    SettleAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    Commission = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    Turnover = table.Column<decimal>(type: "numeric", nullable: true),
                    WinLoss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    TieBetAdjustment = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    ClientStatus = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rounds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                schema: "dbo",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_settings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurrencyCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    MinBet = table.Column<int>(type: "integer", nullable: false),
                    MaxBet = table.Column<int>(type: "integer", nullable: false),
                    MaxBetPerMatch = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bet_limits", x => x.Id);
                    table.ForeignKey(
                        name: "fk_bet_limits_currencies_currency_code",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_action_batches",
                schema: "bet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Error = table.Column<short>(type: "smallint", nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TransactionsCount = table.Column<int>(type: "integer", nullable: false),
                    ErrorsJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_action_batches", x => x.Id);
                    table.ForeignKey(
                        name: "fk_game_action_batches_game_provider_configurations_game_provider_id",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_provider_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    MinBet = table.Column<int>(type: "integer", nullable: false),
                    MaxBet = table.Column<int>(type: "integer", nullable: false),
                    MaxBetPerMatch = table.Column<int>(type: "integer", nullable: true),
                    ExternalSetting = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_provider_bet_limits", x => x.Id);
                    table.ForeignKey(
                        name: "fk_game_provider_bet_limits_currencies_currency_code",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_game_provider_bet_limits_game_providers_game_provider_id",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_provider_configurations",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    SecretKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    DesktopEndpoint = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    MobileEndpoint = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    ConfigurationData = table.Column<string>(type: "text", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_provider_configurations", x => x.Id);
                    table.ForeignKey(
                        name: "fk_game_provider_configurations_game_providers_game_provider_id",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_provider_currencies",
                schema: "dbo",
                columns: table => new
                {
                    GameProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    GameProviderCurrencyCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_provider_currencies", x => new { x.GameProviderId, x.CurrencyCode });
                    table.ForeignKey(
                        name: "fk_game_provider_currencies_currencies_currency_code",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_game_provider_currencies_game_providers_game_provider_id",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "games",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    EndpointPath = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    RuleUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    GameProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    PlatformType = table.Column<byte>(type: "smallint", nullable: false),
                    SubGame = table.Column<bool>(type: "boolean", nullable: false),
                    AvailableToBrand = table.Column<bool>(type: "boolean", nullable: false),
                    HasTurnoverOnTieBet = table.Column<bool>(type: "boolean", nullable: false),
                    Html5 = table.Column<bool>(type: "boolean", nullable: false),
                    AvailableCurrencies = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false, defaultValue: ""),
                    Code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_games", x => x.Id);
                    table.ForeignKey(
                        name: "fk_games_game_providers_game_provider_id",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "market_types",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_market_types", x => x.Id);
                    table.UniqueConstraint("ak_market_types_game_provider_id_code", x => new { x.GameProviderId, x.Code });
                    table.ForeignKey(
                        name: "fk_market_types_game_providers_game_provider_id",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sport_types",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sport_types", x => x.Id);
                    table.UniqueConstraint("ak_sport_types_game_provider_id_code", x => new { x.GameProviderId, x.Code });
                    table.ForeignKey(
                        name: "fk_sport_types_game_providers_game_provider_id",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_provider_languages",
                schema: "dbo",
                columns: table => new
                {
                    GameProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CultureCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    GameProviderCultureCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_provider_languages", x => new { x.GameProviderId, x.CultureCode });
                    table.ForeignKey(
                        name: "fk_game_provider_languages_game_providers_game_provider_id",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_game_provider_languages_languages_culture_code",
                        column: x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "localized_strings",
                schema: "dbo",
                columns: table => new
                {
                    Context = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CultureCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    Translation = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_localized_strings", x => new { x.Key, x.CultureCode, x.Context });
                    table.ForeignKey(
                        name: "fk_localized_strings_languages_culture_code",
                        column: x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment_method_channels",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    MethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false, defaultValueSql: "0"),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "Now()"),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "Now()"),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_method_channels", x => x.Id);
                    table.UniqueConstraint("ak_payment_method_channels_payment_id_method_id_channed_id", x => new { x.PaymentId, x.MethodId, x.ChannelId });
                    table.ForeignKey(
                        name: "fk_payment_method_channels_payment_channels_id",
                        column: x => x.ChannelId,
                        principalSchema: "dbo",
                        principalTable: "payment_channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_payment_method_channels_payment_methods_id",
                        column: x => x.MethodId,
                        principalSchema: "dbo",
                        principalTable: "payment_methods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_payment_method_channels_payments_id",
                        column: x => x.PaymentId,
                        principalSchema: "dbo",
                        principalTable: "payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "licensees",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    OperationalEmail = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ResellerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_licensees", x => x.Id);
                    table.ForeignKey(
                        name: "fk_licensees_resellers_reseller_id",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reseller_currencies",
                schema: "dbo",
                columns: table => new
                {
                    ResellerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reseller_currencies", x => new { x.ResellerId, x.CurrencyCode });
                    table.ForeignKey(
                        name: "fk_reseller_currencies_currencies_currency_code",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reseller_currencies_resellers_reseller_id",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reseller_game_providers",
                schema: "dbo",
                columns: table => new
                {
                    ResellerId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reseller_game_providers", x => new { x.ResellerId, x.GameProviderId });
                    table.ForeignKey(
                        name: "fk_reseller_game_providers_game_providers_game_provider_id",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reseller_game_providers_game_resellers_reseller_id",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reseller_languages",
                schema: "dbo",
                columns: table => new
                {
                    ResellerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CultureCode = table.Column<string>(type: "character varying(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reseller_languages", x => new { x.ResellerId, x.CultureCode });
                    table.ForeignKey(
                        name: "fk_reseller_languages_languages_culture_code",
                        column: x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reseller_languages_resellers_reseller_id",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "round_details",
                schema: "bet",
                columns: table => new
                {
                    RoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdditionalData = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_round_details", x => x.RoundId);
                    table.ForeignKey(
                        name: "fk_round_details_rounds_round_id",
                        column: x => x.RoundId,
                        principalSchema: "bet",
                        principalTable: "rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reseller_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    BetLimitId = table.Column<int>(type: "integer", nullable: false),
                    ResellerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reseller_bet_limits", x => new { x.ResellerId, x.BetLimitId });
                    table.ForeignKey(
                        name: "fk_reseller_bet_limits_resellers_bet_limit_id",
                        column: x => x.BetLimitId,
                        principalSchema: "dbo",
                        principalTable: "bet_limits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reseller_bet_limits_resellers_reseller_id",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_actions",
                schema: "bet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalTransactionId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Turnover = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    WinLoss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    UnsettledBets = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    BetType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ExternalBetId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExternalGameActionGroupId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ReferencedGameActionId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameActionBatchId = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SentOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsClosingRound = table.Column<bool>(type: "boolean", nullable: false),
                    ClosingRoundExplicitRequest = table.Column<bool>(type: "boolean", nullable: true),
                    PlayerIp = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExtraData = table.Column<string>(type: "text", nullable: true),
                    JackpotContribution = table.Column<string>(type: "jsonb", nullable: true),
                    StoredOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_actions", x => x.Id);
                    table.ForeignKey(
                        name: "fk_game_actions_game_action_batches_game_action_batch_id",
                        column: x => x.GameActionBatchId,
                        principalSchema: "bet",
                        principalTable: "game_action_batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_game_actions_referenced_game_action_id",
                        column: x => x.ReferencedGameActionId,
                        principalSchema: "bet",
                        principalTable: "game_actions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_game_actions_rounds_round_id",
                        column: x => x.RoundId,
                        principalSchema: "bet",
                        principalTable: "rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sport_market_types",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SportTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    MarketTypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sport_market_types", x => x.Id);
                    table.UniqueConstraint("ak_sport_market_types_sport_type_id_market_type_id", x => new { x.SportTypeId, x.MarketTypeId });
                    table.ForeignKey(
                        name: "fk_sport_market_types_market_types_market_type_id",
                        column: x => x.MarketTypeId,
                        principalSchema: "dbo",
                        principalTable: "market_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_sport_market_types_sport_types_sport_type_id",
                        column: x => x.SportTypeId,
                        principalSchema: "dbo",
                        principalTable: "sport_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "brands",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    OperationalEmail = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    TimeZoneId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    LicenseeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    SecretKey = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ApiKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SettlementIntervalType = table.Column<int>(type: "integer", nullable: false),
                    BeforeFromDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BeforeToDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AfterFromDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AfterToDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brands", x => x.Id);
                    table.ForeignKey(
                        name: "fk_brands_licensees_licensee_id",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "licensee_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    BetLimitId = table.Column<int>(type: "integer", nullable: false),
                    LicenseeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_licensee_bet_limits", x => new { x.LicenseeId, x.BetLimitId });
                    table.ForeignKey(
                        name: "fk_licensee_bet_limits_bet_limits_bet_limit_id",
                        column: x => x.BetLimitId,
                        principalSchema: "dbo",
                        principalTable: "bet_limits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_licensee_bet_limits_licensees_licensee_id",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "licensee_currencies",
                schema: "dbo",
                columns: table => new
                {
                    LicenseeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_licensee_currencies", x => new { x.LicenseeId, x.CurrencyCode });
                    table.ForeignKey(
                        name: "fk_licensee_currencies_currencies_currency_code",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_licensee_currencies_licensees_licensee_id",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "licensee_game_providers",
                schema: "dbo",
                columns: table => new
                {
                    LicenseeId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_licensee_game_providers", x => new { x.LicenseeId, x.GameProviderId });
                    table.ForeignKey(
                        name: "fk_licensee_game_providers_game_providers_game_provider_id",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_licensee_game_providers_licensees_licensee_id",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "licensee_languages",
                schema: "dbo",
                columns: table => new
                {
                    LicenseeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CultureCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_licensee_languages", x => new { x.LicenseeId, x.CultureCode });
                    table.ForeignKey(
                        name: "fk_licensee_languages_languages_culture_code",
                        column: x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_licensee_languages_licensees_licensee_id",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "agents",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    RealName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Password = table.Column<byte[]>(type: "bytea", nullable: false),
                    Salt = table.Column<byte[]>(type: "bytea", nullable: false),
                    WithdrawPassword = table.Column<byte[]>(type: "bytea", nullable: false),
                    WithdrawSalt = table.Column<byte[]>(type: "bytea", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    AgentStatus = table.Column<int>(type: "integer", nullable: false),
                    LevelType = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    InvitationCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentAgentId = table.Column<Guid>(type: "uuid", nullable: true),
                    RegisteredOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DefaultWalletId = table.Column<Guid>(type: "uuid", nullable: false),
                    CommissionPercentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    DividendPercentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    LiveDealerRebatePercentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    SlotRebatePercentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    LotteryRebatePercentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    CardGameRebatePercentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    SportsRebatePercentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    VirtualSportsRebatePercentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    QQ = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Wechat = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CreatedByUsername = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUsername = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agents", x => x.Id);
                    table.ForeignKey(
                        name: "fk_agents_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_agents_parent_agent_id",
                        column: x => x.ParentAgentId,
                        principalSchema: "dbo",
                        principalTable: "agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "announcements",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Content = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    From = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    To = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DisplayOrder = table.Column<short>(type: "smallint", nullable: false),
                    VerifiedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_announcements", x => x.Id);
                    table.ForeignKey(
                        name: "fk_announcements_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    BetLimitId = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_bet_limits", x => new { x.BrandId, x.BetLimitId });
                    table.ForeignKey(
                        name: "fk_brand_bet_limits_bet_limits_bet_limit_id",
                        column: x => x.BetLimitId,
                        principalSchema: "dbo",
                        principalTable: "bet_limits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_bet_limits_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_currencies",
                schema: "dbo",
                columns: table => new
                {
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    BrandCurrencyCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brand_currencies", x => new { x.BrandId, x.CurrencyCode });
                    table.ForeignKey(
                        name: "fk_brand_currencies_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_currencies_currencies_currency_code",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_game_providers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    GameProviderConfigurationId = table.Column<Guid>(type: "uuid", nullable: false),
                    DefaultLanguageCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    DesktopQueryString = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false, defaultValue: ""),
                    MobileQueryString = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false, defaultValue: ""),
                    IsManualChangeStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_game_providers", x => x.Id);
                    table.ForeignKey(
                        name: "fk_brand_game_providers_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_game_providers_game_provider_configurations_game_provider_configuration_id",
                        column: x => x.GameProviderConfigurationId,
                        principalSchema: "dbo",
                        principalTable: "game_provider_configurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_brand_game_providers_game_providers_game_provider_id",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_brand_game_providers_languages_default_language_code",
                        column: x => x.DefaultLanguageCode,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_games",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<int>(type: "integer", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    Tag = table.Column<byte>(type: "smallint", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    Rebate = table.Column<bool>(type: "boolean", nullable: false),
                    EffectiveBettingPercentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_games", x => x.Id);
                    table.UniqueConstraint("ak_brand_games_brand_id_game_id", x => new { x.BrandId, x.GameId });
                    table.ForeignKey(
                        name: "fk_brand_games_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_games_games_game_id",
                        column: x => x.GameId,
                        principalSchema: "dbo",
                        principalTable: "games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_groups",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsSports = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_groups", x => x.Id);
                    table.ForeignKey(
                        name: "fk_brand_groups_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_languages",
                schema: "dbo",
                columns: table => new
                {
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    CultureCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    BrandCultureCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_languages", x => new { x.BrandId, x.CultureCode });
                    table.ForeignKey(
                        name: "fk_brand_languages_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_languages_languages_culture_code",
                        column: x => x.CultureCode,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "CultureCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_payments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethodChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransferType = table.Column<int>(type: "integer", nullable: false),
                    ChannelName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    TransferLimitFrom = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    TransferLimitTo = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    PlayerListCondition = table.Column<int>(type: "integer", nullable: false),
                    AllPlayers = table.Column<bool>(type: "boolean", nullable: false),
                    IdentityAuthentication = table.Column<bool>(type: "boolean", nullable: false),
                    PhoneAuthentication = table.Column<bool>(type: "boolean", nullable: false),
                    DepositedAmountUnlimited = table.Column<bool>(type: "boolean", nullable: false),
                    DepositedAmountMin = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    DepositedAmountMax = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Reason = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    FeePercentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    LowestFee = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    HighestFee = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "Now()"),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "Now()"),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ExtendedData = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_payments", x => x.Id);
                    table.ForeignKey(
                        name: "fk_brand_payments_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_payments_payment_method_channels_id",
                        column: x => x.PaymentMethodChannelId,
                        principalSchema: "dbo",
                        principalTable: "payment_method_channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "departments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ResellerId = table.Column<Guid>(type: "uuid", nullable: true),
                    LicenseeId = table.Column<Guid>(type: "uuid", nullable: true),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_departments", x => x.Id);
                    table.ForeignKey(
                        name: "fk_departments_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_departments_licensees_licensee_id",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_departments_resellers_reseller_id",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "station_letters",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Content = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Remarks = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    AssignedAllPlayers = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    VerifiedByUsername = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_station_letters", x => x.Id);
                    table.ForeignKey(
                        name: "fk_station_letters_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vip_levels",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    LevelUpAccumulateBetAmount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    LevelUpAccumulateDeposit = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    LevelKeepAccumulateBetAmount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    LevelKeepAccumulateDeposit = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    LevelKeepCondition = table.Column<bool>(type: "boolean", nullable: false),
                    BirthReward = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    PromotionReward = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    QuarterlyReward = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    LiveDealerRebatePercentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    SlotRebatePercentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    LotteryRebatePercentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    CardGameRebatePercentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    SportsRebatePercentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    VirtualSportsRebatePercentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vip_levels", x => x.Id);
                    table.ForeignKey(
                        name: "fk_vip_levels_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "whitelists",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResellerId = table.Column<Guid>(type: "uuid", nullable: true),
                    LicenseeId = table.Column<Guid>(type: "uuid", nullable: true),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    InternalNote = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_whitelists", x => x.Id);
                    table.ForeignKey(
                        name: "fk_whitelists_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_whitelists_licensees_licensee_id",
                        column: x => x.LicenseeId,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_whitelists_resellers_reseller_id",
                        column: x => x.ResellerId,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "agent_audit_logs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Operation = table.Column<short>(type: "smallint", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agent_audit_logs", x => x.Id);
                    table.ForeignKey(
                        name: "fk_agent_audit_logs_agent_id",
                        column: x => x.AgentId,
                        principalSchema: "dbo",
                        principalTable: "agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "agent_banks",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RealName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    BankName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    BankAccount = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    AgentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUsername = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agent_banks", x => x.Id);
                    table.ForeignKey(
                        name: "fk_agent_banks_agents_agent_id",
                        column: x => x.AgentId,
                        principalSchema: "dbo",
                        principalTable: "agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "agent_wallets",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agent_wallets", x => x.Id);
                    table.ForeignKey(
                        name: "fk_agent_wallet_agent_id",
                        column: x => x.AgentId,
                        principalSchema: "dbo",
                        principalTable: "agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_agent_wallets_currencies_currency_code",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "brand_game_provider_schedules",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandGameProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    From = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    To = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_game_provider_schedule", x => x.Id);
                    table.ForeignKey(
                        name: "fk_brand_game_provider_schedule_brand_game_providers_id",
                        column: x => x.BrandGameProviderId,
                        principalSchema: "dbo",
                        principalTable: "brand_game_providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_group_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BetLimitId = table.Column<int>(type: "integer", nullable: false),
                    BrandGroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_group_bet_limits", x => x.Id);
                    table.UniqueConstraint("ak_brand_group_bet_limits_brand_group_id_bet_limit_id", x => new { x.BrandGroupId, x.BetLimitId });
                    table.ForeignKey(
                        name: "fk_brand_group_bet_limits_bet_limits_bet_limit_id",
                        column: x => x.BetLimitId,
                        principalSchema: "dbo",
                        principalTable: "bet_limits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_group_bet_limits_brand_groups_brand_group_id",
                        column: x => x.BrandGroupId,
                        principalSchema: "dbo",
                        principalTable: "brand_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_group_games",
                schema: "dbo",
                columns: table => new
                {
                    BrandGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandGameId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_group_games", x => new { x.BrandGroupId, x.BrandGameId });
                    table.ForeignKey(
                        name: "fk_brand_group_games_brand_games_brand_game_id",
                        column: x => x.BrandGameId,
                        principalSchema: "dbo",
                        principalTable: "brand_games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_group_games_brand_groups_brand_group_id",
                        column: x => x.BrandGroupId,
                        principalSchema: "dbo",
                        principalTable: "brand_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastLoginIp = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LastLoginTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TimeZone = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Password = table.Column<byte[]>(type: "bytea", nullable: false),
                    Salt = table.Column<byte[]>(type: "bytea", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.Id);
                    table.ForeignKey(
                        name: "fk_users_departments_department_id",
                        column: x => x.DepartmentId,
                        principalSchema: "dbo",
                        principalTable: "departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_users_roles_role_id",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_payment_vip_levels",
                schema: "dbo",
                columns: table => new
                {
                    BrandPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    VipLevelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_payment_vip_levels", x => new { x.BrandPaymentId, x.VipLevelId });
                    table.ForeignKey(
                        name: "fk_brand_payment_vip_levels_brand_payments_brand_payment_id",
                        column: x => x.BrandPaymentId,
                        principalSchema: "dbo",
                        principalTable: "brand_payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_payment_vip_levels_vip_levels_vip_level_id",
                        column: x => x.VipLevelId,
                        principalSchema: "dbo",
                        principalTable: "vip_levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "players",
                schema: "dbo",
                columns: table => new
                {
                    PlayerCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    IdNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    RealName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Password = table.Column<byte[]>(type: "bytea", nullable: false),
                    Salt = table.Column<byte[]>(type: "bytea", nullable: false),
                    WithdrawPassword = table.Column<byte[]>(type: "bytea", nullable: false),
                    WithdrawSalt = table.Column<byte[]>(type: "bytea", nullable: false),
                    DefaultWalletId = table.Column<Guid>(type: "uuid", nullable: false),
                    DefaultWalletCurrencyCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegisteredOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastActiveOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    BetLimitId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Email = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    QQ = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Wechat = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    AgentId = table.Column<Guid>(type: "uuid", nullable: true),
                    IdentityAuthentication = table.Column<bool>(type: "boolean", nullable: false),
                    PhoneAuthentication = table.Column<bool>(type: "boolean", nullable: false),
                    AccumulatedDeposit = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false, defaultValue: 0m),
                    VipId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUsername = table.Column<string>(type: "text", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FirstDepositOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_code", x => x.PlayerCode);
                    table.ForeignKey(
                        name: "fk_players_agents_agent_id",
                        column: x => x.AgentId,
                        principalSchema: "dbo",
                        principalTable: "agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_players_bet_limits_bet_limit_id",
                        column: x => x.BetLimitId,
                        principalSchema: "dbo",
                        principalTable: "bet_limits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_players_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_players_vip_levels_vip_id",
                        column: x => x.VipId,
                        principalSchema: "dbo",
                        principalTable: "vip_levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "promotions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    From = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    To = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    PromotionType = table.Column<string>(type: "text", nullable: false),
                    PromotionAmount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    AmountUsed = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Unlimited = table.Column<short>(type: "smallint", nullable: false),
                    GameType = table.Column<short>(type: "smallint", nullable: true),
                    Percentage = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    TotalAmountLimit = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    TurnoverMultiple = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUsername = table.Column<string>(type: "text", nullable: true),
                    VipLevelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_promotions_id", x => x.Id);
                    table.ForeignKey(
                        name: "fk_promotions_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_promotions_vip_levels_VipLevelId",
                        column: x => x.VipLevelId,
                        principalSchema: "dbo",
                        principalTable: "vip_levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgentTransactions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WalletId = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalTransactionId = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    RecordedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SentOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ConfirmedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentTransactions_agent_wallets_WalletId",
                        column: x => x.WalletId,
                        principalSchema: "dbo",
                        principalTable: "agent_wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgentTransactions_brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_group_game_provider_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    BrandGroupBetLimitId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameProviderBetLimitId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_group_game_provider_bet_limits", x => new { x.BrandGroupBetLimitId, x.GameProviderBetLimitId });
                    table.ForeignKey(
                        name: "fk_brand_group_game_provider_bet_limits_brand_group_bet_limits_brand_group_bet_limit_id",
                        column: x => x.BrandGroupBetLimitId,
                        principalSchema: "dbo",
                        principalTable: "brand_group_bet_limits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_group_game_provider_bet_limits_game_provider_bet_limits_game_provider_bet_limit_id",
                        column: x => x.GameProviderBetLimitId,
                        principalSchema: "dbo",
                        principalTable: "game_provider_bet_limits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_payment_players",
                schema: "dbo",
                columns: table => new
                {
                    PlayerCode = table.Column<string>(type: "character varying(16)", nullable: false),
                    BrandPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ListType = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_payment_players", x => new { x.BrandPaymentId, x.PlayerCode });
                    table.ForeignKey(
                        name: "fk_brand_payment_players_brand_payments_brand_payment_id",
                        column: x => x.BrandPaymentId,
                        principalSchema: "dbo",
                        principalTable: "brand_payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_payment_players_players_player_code",
                        column: x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "deposit_forms",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    BankAccount = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    BrandPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Remark = table.Column<string>(type: "text", nullable: true),
                    ManualReceiptFileId = table.Column<string>(type: "text", nullable: true),
                    ManualReceiptExternalUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "Now()"),
                    UpdateOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "Now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_deposit_forms", x => x.Id);
                    table.ForeignKey(
                        name: "fk_deposit_forms_brand_payments_id",
                        column: x => x.BrandPaymentId,
                        principalSchema: "dbo",
                        principalTable: "brand_payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_deposit_forms_players_playerCode",
                        column: x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_deposit_froms_brands_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_banks",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    RealName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    BankName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    BankAccount = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UpdatedByUsername = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_banks", x => x.Id);
                    table.ForeignKey(
                        name: "fk_player_banks_players_player_code",
                        column: x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_login_records",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    PlatformType = table.Column<short>(type: "smallint", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiredOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_login_records", x => x.Id);
                    table.ForeignKey(
                        name: "fk_player_login_records_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_player_login_records_players_player_code",
                        column: x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "player_sports_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    PlayerCode = table.Column<string>(type: "character varying(16)", nullable: false),
                    SportMarketTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    MinBet = table.Column<int>(type: "integer", nullable: false),
                    MaxBet = table.Column<int>(type: "integer", nullable: false),
                    MaxBetPerMatch = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_sports_bet_limits", x => new { x.PlayerCode, x.SportMarketTypeId });
                    table.ForeignKey(
                        name: "fk_player_sports_bet_limits_players_player_code",
                        column: x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_player_sports_bet_limits_sport_market_types_sport_market_type_id",
                        column: x => x.SportMarketTypeId,
                        principalSchema: "dbo",
                        principalTable: "sport_market_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "station_letter_players",
                schema: "dbo",
                columns: table => new
                {
                    StationLetterId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerCode = table.Column<string>(type: "character varying(16)", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ReadOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_station_letter_players", x => new { x.StationLetterId, x.PlayerCode });
                    table.ForeignKey(
                        name: "fk_station_letter_players_players_player_code",
                        column: x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_station_letter_players_station_letters_station_letter_id",
                        column: x => x.StationLetterId,
                        principalSchema: "dbo",
                        principalTable: "station_letters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wallets",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    ExternalCurrencyCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wallets", x => x.Id);
                    table.ForeignKey(
                        name: "fk_wallets_currencies_currency_code",
                        column: x => x.CurrencyCode,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_wallets_players_player_code",
                        column: x => x.PlayerCode,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "PlayerCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "promotion_game_providers",
                schema: "dbo",
                columns: table => new
                {
                    PromotionId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_promotion_game_providers", x => new { x.PromotionId, x.GameProviderId });
                    table.ForeignKey(
                        name: "fk_promotion_game_provider_game_provider_id",
                        column: x => x.GameProviderId,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_promotion_game_provider_promotion_id",
                        column: x => x.PromotionId,
                        principalSchema: "dbo",
                        principalTable: "promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "promotion_vip_levels",
                schema: "dbo",
                columns: table => new
                {
                    PromotionId = table.Column<Guid>(type: "uuid", nullable: false),
                    VipLevelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_promotion_vip_levels", x => new { x.PromotionId, x.VipLevelId });
                    table.ForeignKey(
                        name: "fk_promotion_vip_level_promotion_id",
                        column: x => x.PromotionId,
                        principalSchema: "dbo",
                        principalTable: "promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_promotion_vip_level_vip_level_id",
                        column: x => x.VipLevelId,
                        principalSchema: "dbo",
                        principalTable: "vip_levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WalletId = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalTransactionId = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    RecordedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SentOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ConfirmedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transactions", x => x.Id);
                    table.ForeignKey(
                        name: "fk_transactions_brands_brand_id",
                        column: x => x.BrandId,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_transactions_wallets_wallet_id",
                        column: x => x.WalletId,
                        principalSchema: "dbo",
                        principalTable: "wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_agent_audit_logs_AgentId",
                schema: "dbo",
                table: "agent_audit_logs",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "ix_agent_bankers_agent_id",
                schema: "dbo",
                table: "agent_banks",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_agent_wallets_AgentId",
                schema: "dbo",
                table: "agent_wallets",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "ix_agent_wallets_currency_code",
                schema: "dbo",
                table: "agent_wallets",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "ix_agents_brand_id",
                schema: "dbo",
                table: "agents",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_agents_ParentAgentId",
                schema: "dbo",
                table: "agents",
                column: "ParentAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentTransactions_BrandId",
                schema: "dbo",
                table: "AgentTransactions",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentTransactions_WalletId",
                schema: "dbo",
                table: "AgentTransactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "ix_announcements_brand_id",
                schema: "dbo",
                table: "announcements",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "ix_bet_limits_currency_code",
                schema: "dbo",
                table: "bet_limits",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "ix_brand_bet_limits_bet_limit_id",
                schema: "dbo",
                table: "brand_bet_limits",
                column: "BetLimitId");

            migrationBuilder.CreateIndex(
                name: "ix_brand_currencies_currency_code",
                schema: "dbo",
                table: "brand_currencies",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "ix_brand_game_provider_schedule_brand_game_provider_id",
                schema: "dbo",
                table: "brand_game_provider_schedules",
                column: "BrandGameProviderId");

            migrationBuilder.CreateIndex(
                name: "ix_brand_game_providers_default_language_code",
                schema: "dbo",
                table: "brand_game_providers",
                column: "DefaultLanguageCode");

            migrationBuilder.CreateIndex(
                name: "ix_brand_game_providers_game_provider_configuration_id",
                schema: "dbo",
                table: "brand_game_providers",
                column: "GameProviderConfigurationId");

            migrationBuilder.CreateIndex(
                name: "ix_brand_game_providers_game_provider_id",
                schema: "dbo",
                table: "brand_game_providers",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "ux_brand_game_providers_brand_id_game_provider_id",
                schema: "dbo",
                table: "brand_game_providers",
                columns: new[] { "BrandId", "GameProviderId" },
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "GameProviderConfigurationId", "Status", "DefaultLanguageCode", "DesktopQueryString", "MobileQueryString" });

            migrationBuilder.CreateIndex(
                name: "ix_brand_games_game_id",
                schema: "dbo",
                table: "brand_games",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "ix_brand_group_bet_limits_bet_limit_id",
                schema: "dbo",
                table: "brand_group_bet_limits",
                column: "BetLimitId");

            migrationBuilder.CreateIndex(
                name: "ix_brand_group_bet_limits_brand_group_id",
                schema: "dbo",
                table: "brand_group_bet_limits",
                column: "BrandGroupId");

            migrationBuilder.CreateIndex(
                name: "ix_brand_group_game_provider_bet_limits_game_provider_bet_limit_id",
                schema: "dbo",
                table: "brand_group_game_provider_bet_limits",
                column: "GameProviderBetLimitId");

            migrationBuilder.CreateIndex(
                name: "ix_brand_group_games_brand_game_id",
                schema: "dbo",
                table: "brand_group_games",
                column: "BrandGameId");

            migrationBuilder.CreateIndex(
                name: "ix_brand_groups_brand_id",
                schema: "dbo",
                table: "brand_groups",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "ix_brand_languages_culture_code",
                schema: "dbo",
                table: "brand_languages",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                name: "IX_brand_payment_audit_logs_BrandPaymentId",
                schema: "dbo",
                table: "brand_payment_audit_logs",
                column: "BrandPaymentId");

            migrationBuilder.CreateIndex(
                name: "ix_brand_payment_players_player_code",
                schema: "dbo",
                table: "brand_payment_players",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                name: "ix_brand_payment_vip_levels_vip_level_id",
                schema: "dbo",
                table: "brand_payment_vip_levels",
                column: "VipLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_brand_payments_BrandId",
                schema: "dbo",
                table: "brand_payments",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "ix_brand_payments_payment_method_channel_id",
                schema: "dbo",
                table: "brand_payments",
                column: "PaymentMethodChannelId");

            migrationBuilder.CreateIndex(
                name: "ix_brands_licensee_id",
                schema: "dbo",
                table: "brands",
                column: "LicenseeId");

            migrationBuilder.CreateIndex(
                name: "ix_departments_brand_id",
                schema: "dbo",
                table: "departments",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "ix_departments_licensee_id",
                schema: "dbo",
                table: "departments",
                column: "LicenseeId");

            migrationBuilder.CreateIndex(
                name: "ix_departments_reseller_id",
                schema: "dbo",
                table: "departments",
                column: "ResellerId");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_forms_brandId",
                schema: "dbo",
                table: "deposit_forms",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_forms_brandPaymentId",
                schema: "dbo",
                table: "deposit_forms",
                column: "BrandPaymentId");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_forms_playerCode",
                schema: "dbo",
                table: "deposit_forms",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                name: "ix_game_action_batches_game_provider_id",
                schema: "bet",
                table: "game_action_batches",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "ix_game_actions_game_action_batch_id",
                schema: "bet",
                table: "game_actions",
                column: "GameActionBatchId");

            migrationBuilder.CreateIndex(
                name: "ix_game_actions_referenced_game_action_id",
                schema: "bet",
                table: "game_actions",
                column: "ReferencedGameActionId");

            migrationBuilder.CreateIndex(
                name: "ix_game_actions_round_id",
                schema: "bet",
                table: "game_actions",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "ix_game_provider_bet_limits_currency_code",
                schema: "dbo",
                table: "game_provider_bet_limits",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "ix_game_provider_bet_limits_game_provider_id",
                schema: "dbo",
                table: "game_provider_bet_limits",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "ix_game_provider_configurations_game_provider_id",
                schema: "dbo",
                table: "game_provider_configurations",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "ix_game_provider_currencies_currency_code",
                schema: "dbo",
                table: "game_provider_currencies",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "ix_game_provider_languages_culture_code",
                schema: "dbo",
                table: "game_provider_languages",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                name: "ix_games_game_provider_id",
                schema: "dbo",
                table: "games",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "ix_licensee_bet_limits_bet_limit_id",
                schema: "dbo",
                table: "licensee_bet_limits",
                column: "BetLimitId");

            migrationBuilder.CreateIndex(
                name: "ix_licensee_currencies_currency_code",
                schema: "dbo",
                table: "licensee_currencies",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "ix_licensee_game_providers_game_provider_id",
                schema: "dbo",
                table: "licensee_game_providers",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_licensee_languages_CultureCode",
                schema: "dbo",
                table: "licensee_languages",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                name: "ix_licensees_reseller_id",
                schema: "dbo",
                table: "licensees",
                column: "ResellerId");

            migrationBuilder.CreateIndex(
                name: "ix_localized_strings_culture_code",
                schema: "dbo",
                table: "localized_strings",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                name: "ix_payment_method_channels_channel_id",
                schema: "dbo",
                table: "payment_method_channels",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "ix_payment_method_channels_method_id",
                schema: "dbo",
                table: "payment_method_channels",
                column: "MethodId");

            migrationBuilder.CreateIndex(
                name: "ix_payment_method_channels_payment_id",
                schema: "dbo",
                table: "payment_method_channels",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "ix_player_bankers_player_code",
                schema: "dbo",
                table: "player_banks",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                name: "ix_player_login_records_brand_id",
                schema: "dbo",
                table: "player_login_records",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "ix_player_login_records_player_code",
                schema: "dbo",
                table: "player_login_records",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                name: "ix_player_sports_bet_limits_sport_market_id",
                schema: "dbo",
                table: "player_sports_bet_limits",
                column: "SportMarketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_players_AgentId",
                schema: "dbo",
                table: "players",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "ix_players_bet_limit_id",
                schema: "dbo",
                table: "players",
                column: "BetLimitId");

            migrationBuilder.CreateIndex(
                name: "IX_players_BrandId",
                schema: "dbo",
                table: "players",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "ix_players_vip_id",
                schema: "dbo",
                table: "players",
                column: "VipId");

            migrationBuilder.CreateIndex(
                name: "ix_promotion_game_provider_game_provider_id",
                schema: "dbo",
                table: "promotion_game_providers",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "ix_promotion_vip_level_vip_level_id",
                schema: "dbo",
                table: "promotion_vip_levels",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_promotion_vip_levels_VipLevelId",
                schema: "dbo",
                table: "promotion_vip_levels",
                column: "VipLevelId");

            migrationBuilder.CreateIndex(
                name: "ix_promotions_brand_id",
                schema: "dbo",
                table: "promotions",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_promotions_VipLevelId",
                schema: "dbo",
                table: "promotions",
                column: "VipLevelId");

            migrationBuilder.CreateIndex(
                name: "ix_reseller_bet_limits_bet_limit_id",
                schema: "dbo",
                table: "reseller_bet_limits",
                column: "BetLimitId");

            migrationBuilder.CreateIndex(
                name: "ix_reseller_currencies_currency_code",
                schema: "dbo",
                table: "reseller_currencies",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "ix_reseller_game_providers_game_provider_id",
                schema: "dbo",
                table: "reseller_game_providers",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "ix_reseller_languages_culture_code",
                schema: "dbo",
                table: "reseller_languages",
                column: "CultureCode");

            migrationBuilder.CreateIndex(
                name: "ix_rounds_brand_id",
                schema: "bet",
                table: "rounds",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "ix_rounds_closed_on",
                schema: "bet",
                table: "rounds",
                column: "ClosedOn")
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "ix_rounds_game_id",
                schema: "bet",
                table: "rounds",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "ix_rounds_game_provider_id",
                schema: "bet",
                table: "rounds",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "ix_rounds_player_code",
                schema: "bet",
                table: "rounds",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                name: "ux_rounds_external_round_id_composite",
                schema: "bet",
                table: "rounds",
                columns: new[] { "ExternalRoundId", "PlayerCode", "GameProviderId", "GameId" },
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "GameProviderConfigurationId", "WalletId" });

            migrationBuilder.CreateIndex(
                name: "ix_sport_market_types_market_type_id",
                schema: "dbo",
                table: "sport_market_types",
                column: "MarketTypeId");

            migrationBuilder.CreateIndex(
                name: "ix_sport_types_game_provider_id",
                schema: "dbo",
                table: "sport_types",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "ix_station_letter_players_player_code",
                schema: "dbo",
                table: "station_letter_players",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                name: "ix_station_letters_brand_id",
                schema: "dbo",
                table: "station_letters",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_brand_id",
                schema: "dbo",
                table: "transactions",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_wallet_id",
                schema: "dbo",
                table: "transactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "ix_users_department_id",
                schema: "dbo",
                table: "users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "ix_users_role_id",
                schema: "dbo",
                table: "users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "ix_vip_levels_brand_id",
                schema: "dbo",
                table: "vip_levels",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "ix_wallets_currency_code",
                schema: "dbo",
                table: "wallets",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_wallets_PlayerCode",
                schema: "dbo",
                table: "wallets",
                column: "PlayerCode");

            migrationBuilder.CreateIndex(
                name: "ix_whitelists_brand_id",
                schema: "dbo",
                table: "whitelists",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "ix_whitelists_licensee_id",
                schema: "dbo",
                table: "whitelists",
                column: "LicenseeId");

            migrationBuilder.CreateIndex(
                name: "ix_whitelists_reseller_id",
                schema: "dbo",
                table: "whitelists",
                column: "ResellerId");

            InitAfterRun(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agent_audit_logs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "agent_banks",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AgentTransactions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "announcements",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brand_bet_limits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brand_currencies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brand_game_provider_schedules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brand_group_game_provider_bet_limits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brand_group_games",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brand_languages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brand_payment_audit_logs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brand_payment_players",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brand_payment_vip_levels",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "deposit_forms",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "game_actions",
                schema: "bet");

            migrationBuilder.DropTable(
                name: "game_provider_currencies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "game_provider_languages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "licensee_bet_limits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "licensee_currencies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "licensee_game_providers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "licensee_languages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "localized_strings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "player_audit_logs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "player_banks",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "player_login_records",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "player_sports_bet_limits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "promotion_game_providers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "promotion_vip_levels",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "reseller_bet_limits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "reseller_currencies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "reseller_game_providers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "reseller_languages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "round_details",
                schema: "bet");

            migrationBuilder.DropTable(
                name: "settings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "station_letter_players",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "transactions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "users",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "whitelists",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "agent_wallets",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brand_game_providers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brand_group_bet_limits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "game_provider_bet_limits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brand_games",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brand_payments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "game_action_batches",
                schema: "bet");

            migrationBuilder.DropTable(
                name: "sport_market_types",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "promotions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rounds",
                schema: "bet");

            migrationBuilder.DropTable(
                name: "station_letters",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "wallets",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "departments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "game_provider_configurations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "languages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brand_groups",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "games",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "payment_method_channels",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "market_types",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "sport_types",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "players",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "payment_channels",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "payment_methods",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "payments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "game_providers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "agents",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "bet_limits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "vip_levels",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "currencies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brands",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "licensees",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "resellers",
                schema: "dbo");
        }
    }
}
