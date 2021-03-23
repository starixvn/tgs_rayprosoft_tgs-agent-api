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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    before = table.Column<string>(type: "jsonb", nullable: false),
                    after = table.Column<string>(type: "jsonb", nullable: false),
                    reason = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    operation = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_payment_audit_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "currencies",
                schema: "dbo",
                columns: table => new
                {
                    code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    native_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    symbol = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_currencies", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "game_providers",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    auto_close_round = table.Column<bool>(type: "boolean", nullable: false),
                    support_test_player = table.Column<bool>(type: "boolean", nullable: false),
                    allow_kick_player = table.Column<bool>(type: "boolean", nullable: false),
                    place_bet_timeout = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    settle_bet_timeout = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    unique_id = table.Column<short>(type: "smallint", nullable: false),
                    code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_providers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "languages",
                schema: "dbo",
                columns: table => new
                {
                    culture_code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    language_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    localized_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_languages", x => x.culture_code);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "player_audit_logs",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    module_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    operation = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_audit_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "resellers",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    operational_email = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    business_type = table.Column<short>(type: "smallint", nullable: false),
                    code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_resellers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_type = table.Column<short>(type: "smallint", nullable: false),
                    reseller_id = table.Column<Guid>(type: "uuid", nullable: true),
                    licensee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_root = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    display_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    menu_setting = table.Column<string>(type: "jsonb", nullable: false),
                    description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rounds",
                schema: "bet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_round_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_code = table.Column<string>(type: "text", nullable: false),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    currency_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    platform_type = table.Column<short>(type: "smallint", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    game_provider_configuration_id = table.Column<Guid>(type: "uuid", nullable: false),
                    wallet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    closed_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    external_closed_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    bet_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    valid_bet_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    settle_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    commission = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    turnover = table.Column<decimal>(type: "numeric", nullable: true),
                    win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    tie_bet_adjustment = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    client_status = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rounds", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                schema: "dbo",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_settings", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    currency_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    min_bet = table.Column<int>(type: "integer", nullable: false),
                    max_bet = table.Column<int>(type: "integer", nullable: false),
                    max_bet_per_match = table.Column<int>(type: "integer", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bet_limits", x => x.id);
                    table.ForeignKey(
                        name: "fk_bet_limits_currencies_currency_code",
                        column: x => x.currency_code,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_action_batches",
                schema: "bet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    error = table.Column<short>(type: "smallint", nullable: false),
                    error_message = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    transactions_count = table.Column<int>(type: "integer", nullable: false),
                    errors_json = table.Column<string>(type: "jsonb", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_action_batches", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_action_batches_game_provider_configurations_game_provider_id",
                        column: x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_provider_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    min_bet = table.Column<int>(type: "integer", nullable: false),
                    max_bet = table.Column<int>(type: "integer", nullable: false),
                    max_bet_per_match = table.Column<int>(type: "integer", nullable: true),
                    external_setting = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_provider_bet_limits", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_provider_bet_limits_currencies_currency_code",
                        column: x => x.currency_code,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_game_provider_bet_limits_game_providers_game_provider_id",
                        column: x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_provider_configurations",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    client_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    secret_key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                    desktop_endpoint = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    mobile_endpoint = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    configuration_data = table.Column<string>(type: "text", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_provider_configurations", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_provider_configurations_game_providers_game_provider_id",
                        column: x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_provider_currencies",
                schema: "dbo",
                columns: table => new
                {
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    game_provider_currency_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_provider_currencies", x => new { x.game_provider_id, x.currency_code });
                    table.ForeignKey(
                        name: "fk_game_provider_currencies_currencies_currency_code",
                        column: x => x.currency_code,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_game_provider_currencies_game_providers_game_provider_id",
                        column: x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "games",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    external_id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    endpoint_path = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    rule_url = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    platform_type = table.Column<short>(type: "smallint", nullable: false),
                    sub_game = table.Column<bool>(type: "boolean", nullable: false),
                    available_to_brand = table.Column<bool>(type: "boolean", nullable: false),
                    has_turnover_on_tie_bet = table.Column<bool>(type: "boolean", nullable: false),
                    html5 = table.Column<bool>(type: "boolean", nullable: false),
                    available_currencies = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false, defaultValue: ""),
                    code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_games", x => x.id);
                    table.ForeignKey(
                        name: "fk_games_game_providers_game_provider_id",
                        column: x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "market_types",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    external_id = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_market_types", x => x.id);
                    table.UniqueConstraint("ak_market_types_game_provider_id_code", x => new { x.game_provider_id, x.code });
                    table.ForeignKey(
                        name: "fk_market_types_game_providers_game_provider_id",
                        column: x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sport_types",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    external_id = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sport_types", x => x.id);
                    table.UniqueConstraint("ak_sport_types_game_provider_id_code", x => new { x.game_provider_id, x.code });
                    table.ForeignKey(
                        name: "fk_sport_types_game_providers_game_provider_id",
                        column: x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_provider_languages",
                schema: "dbo",
                columns: table => new
                {
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                    culture_code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    game_provider_culture_code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_provider_languages", x => new { x.game_provider_id, x.culture_code });
                    table.ForeignKey(
                        name: "fk_game_provider_languages_game_providers_game_provider_id",
                        column: x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_game_provider_languages_languages_culture_code",
                        column: x => x.culture_code,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "culture_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "localized_strings",
                schema: "dbo",
                columns: table => new
                {
                    context = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    culture_code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    translation = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_localized_strings", x => new { x.key, x.culture_code, x.context });
                    table.ForeignKey(
                        name: "fk_localized_strings_languages_culture_code",
                        column: x => x.culture_code,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "culture_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment_methods",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_methods", x => x.id);
                    table.ForeignKey(
                        name: "fk_payment_methods_payments_payment_id",
                        column: x => x.payment_id,
                        principalSchema: "dbo",
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "licensees",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    operational_email = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    reseller_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_licensees", x => x.id);
                    table.ForeignKey(
                        name: "fk_licensees_resellers_reseller_id",
                        column: x => x.reseller_id,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reseller_currencies",
                schema: "dbo",
                columns: table => new
                {
                    reseller_id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency_code = table.Column<string>(type: "character varying(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reseller_currencies", x => new { x.reseller_id, x.currency_code });
                    table.ForeignKey(
                        name: "fk_reseller_currencies_currencies_currency_code",
                        column: x => x.currency_code,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reseller_currencies_resellers_reseller_id",
                        column: x => x.reseller_id,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reseller_game_providers",
                schema: "dbo",
                columns: table => new
                {
                    reseller_id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reseller_game_providers", x => new { x.reseller_id, x.game_provider_id });
                    table.ForeignKey(
                        name: "fk_reseller_game_providers_game_providers_game_provider_id",
                        column: x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reseller_game_providers_game_resellers_reseller_id",
                        column: x => x.reseller_id,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reseller_languages",
                schema: "dbo",
                columns: table => new
                {
                    reseller_id = table.Column<Guid>(type: "uuid", nullable: false),
                    culture_code = table.Column<string>(type: "character varying(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reseller_languages", x => new { x.reseller_id, x.culture_code });
                    table.ForeignKey(
                        name: "fk_reseller_languages_languages_culture_code",
                        column: x => x.culture_code,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "culture_code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reseller_languages_resellers_reseller_id",
                        column: x => x.reseller_id,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "round_details",
                schema: "bet",
                columns: table => new
                {
                    round_id = table.Column<Guid>(type: "uuid", nullable: false),
                    additional_data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_round_details", x => x.round_id);
                    table.ForeignKey(
                        name: "fk_round_details_rounds_round_id",
                        column: x => x.round_id,
                        principalSchema: "bet",
                        principalTable: "rounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reseller_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    bet_limit_id = table.Column<int>(type: "integer", nullable: false),
                    reseller_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reseller_bet_limits", x => new { x.reseller_id, x.bet_limit_id });
                    table.ForeignKey(
                        name: "fk_reseller_bet_limits_resellers_bet_limit_id",
                        column: x => x.bet_limit_id,
                        principalSchema: "dbo",
                        principalTable: "bet_limits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reseller_bet_limits_resellers_reseller_id",
                        column: x => x.reseller_id,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_actions",
                schema: "bet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    round_id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_transaction_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    turnover = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    unsettled_bets = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    balance = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    bet_type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    external_bet_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    external_game_action_group_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    referenced_game_action_id = table.Column<Guid>(type: "uuid", nullable: true),
                    game_action_batch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    recorded_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    sent_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_closing_round = table.Column<bool>(type: "boolean", nullable: false),
                    closing_round_explicit_request = table.Column<bool>(type: "boolean", nullable: true),
                    player_ip = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    stored_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_actions", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_actions_game_action_batches_game_action_batch_id",
                        column: x => x.game_action_batch_id,
                        principalSchema: "bet",
                        principalTable: "game_action_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_game_actions_referenced_game_action_id",
                        column: x => x.referenced_game_action_id,
                        principalSchema: "bet",
                        principalTable: "game_actions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_game_actions_rounds_round_id",
                        column: x => x.round_id,
                        principalSchema: "bet",
                        principalTable: "rounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sport_market_types",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sport_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    market_type_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sport_market_types", x => x.id);
                    table.UniqueConstraint("ak_sport_market_types_sport_type_id_market_type_id", x => new { x.sport_type_id, x.market_type_id });
                    table.ForeignKey(
                        name: "fk_sport_market_types_market_types_market_type_id",
                        column: x => x.market_type_id,
                        principalSchema: "dbo",
                        principalTable: "market_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_sport_market_types_sport_types_sport_type_id",
                        column: x => x.sport_type_id,
                        principalSchema: "dbo",
                        principalTable: "sport_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payment_channels",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_method_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_channels", x => x.id);
                    table.ForeignKey(
                        name: "fk_payment_channels_payment_methods_payment_method_id",
                        column: x => x.payment_method_id,
                        principalSchema: "dbo",
                        principalTable: "payment_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_payment_channels_payments_payment_id",
                        column: x => x.payment_id,
                        principalSchema: "dbo",
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "brands",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    operational_email = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    time_zone_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    licensee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    secret_key = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    api_key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brands", x => x.id);
                    table.ForeignKey(
                        name: "fk_brands_licensees_licensee_id",
                        column: x => x.licensee_id,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "licensee_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    bet_limit_id = table.Column<int>(type: "integer", nullable: false),
                    licensee_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_licensee_bet_limits", x => new { x.licensee_id, x.bet_limit_id });
                    table.ForeignKey(
                        name: "fk_licensee_bet_limits_bet_limits_bet_limit_id",
                        column: x => x.bet_limit_id,
                        principalSchema: "dbo",
                        principalTable: "bet_limits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_licensee_bet_limits_licensees_licensee_id",
                        column: x => x.licensee_id,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "licensee_currencies",
                schema: "dbo",
                columns: table => new
                {
                    licensee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_licensee_currencies", x => new { x.licensee_id, x.currency_code });
                    table.ForeignKey(
                        name: "fk_licensee_currencies_currencies_currency_code",
                        column: x => x.currency_code,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_licensee_currencies_licensees_licensee_id",
                        column: x => x.licensee_id,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "licensee_game_providers",
                schema: "dbo",
                columns: table => new
                {
                    licensee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_licensee_game_providers", x => new { x.licensee_id, x.game_provider_id });
                    table.ForeignKey(
                        name: "fk_licensee_game_providers_game_providers_game_provider_id",
                        column: x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_licensee_game_providers_licensees_licensee_id",
                        column: x => x.licensee_id,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "licensee_languages",
                schema: "dbo",
                columns: table => new
                {
                    licensee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    culture_code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_licensee_languages", x => new { x.licensee_id, x.culture_code });
                    table.ForeignKey(
                        name: "fk_licensee_languages_languages_culture_code",
                        column: x => x.culture_code,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "culture_code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_licensee_languages_licensees_licensee_id",
                        column: x => x.licensee_id,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "announcements",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    content = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    remarks = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    display_order = table.Column<short>(type: "smallint", nullable: false),
                    verified_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_announcements", x => x.id);
                    table.ForeignKey(
                        name: "fk_announcements_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bet_limit_id = table.Column<int>(type: "integer", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_bet_limits", x => new { x.brand_id, x.bet_limit_id });
                    table.ForeignKey(
                        name: "fk_brand_bet_limits_bet_limits_bet_limit_id",
                        column: x => x.bet_limit_id,
                        principalSchema: "dbo",
                        principalTable: "bet_limits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_bet_limits_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_currencies",
                schema: "dbo",
                columns: table => new
                {
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    brand_currency_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_currencies", x => new { x.brand_id, x.currency_code });
                    table.ForeignKey(
                        name: "fk_brand_currencies_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_currencies_currencies_currency_code",
                        column: x => x.currency_code,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_game_providers",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    game_provider_configuration_id = table.Column<Guid>(type: "uuid", nullable: false),
                    default_language_code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    desktop_query_string = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false, defaultValue: ""),
                    mobile_query_string = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false, defaultValue: ""),
                    is_manual_change_status = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_game_providers", x => x.id);
                    table.ForeignKey(
                        name: "fk_brand_game_providers_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_game_providers_game_provider_configurations_game_provider_configuration_id",
                        column: x => x.game_provider_configuration_id,
                        principalSchema: "dbo",
                        principalTable: "game_provider_configurations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_brand_game_providers_game_providers_game_provider_id",
                        column: x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_brand_game_providers_languages_default_language_code",
                        column: x => x.default_language_code,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "culture_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_games",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    tag = table.Column<byte>(type: "smallint", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    rebate = table.Column<bool>(type: "boolean", nullable: false),
                    effective_betting_percentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_games", x => x.id);
                    table.UniqueConstraint("ak_brand_games_brand_id_game_id", x => new { x.brand_id, x.game_id });
                    table.ForeignKey(
                        name: "fk_brand_games_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_games_games_game_id",
                        column: x => x.game_id,
                        principalSchema: "dbo",
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_groups",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_sports = table.Column<bool>(type: "boolean", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_brand_groups_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_languages",
                schema: "dbo",
                columns: table => new
                {
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    culture_code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    brand_culture_code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_languages", x => new { x.brand_id, x.culture_code });
                    table.ForeignKey(
                        name: "fk_brand_languages_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_languages_languages_culture_code",
                        column: x => x.culture_code,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "culture_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_payments",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_method_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transfer_type = table.Column<int>(type: "integer", nullable: false),
                    channel_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    payment_status = table.Column<int>(type: "integer", nullable: false),
                    transfer_limit_from = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    transfer_limit_to = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    player_list_condition = table.Column<int>(type: "integer", nullable: false),
                    all_players = table.Column<bool>(type: "boolean", nullable: false),
                    identity_authentication = table.Column<bool>(type: "boolean", nullable: false),
                    phone_authentication = table.Column<bool>(type: "boolean", nullable: false),
                    deposited_amount_unlimited = table.Column<bool>(type: "boolean", nullable: false),
                    deposited_amount_min = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    deposited_amount_max = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    reason = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    fee_percentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    lowest_fee = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    highest_fee = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_payments", x => x.id);
                    table.ForeignKey(
                        name: "fk_brand_payments_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_payments_payment_channels_payment_channel_id",
                        column: x => x.payment_channel_id,
                        principalSchema: "dbo",
                        principalTable: "payment_channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_payments_payment_methods_payment_method_id",
                        column: x => x.payment_method_id,
                        principalSchema: "dbo",
                        principalTable: "payment_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_payments_payments_payment_id",
                        column: x => x.payment_id,
                        principalSchema: "dbo",
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "departments",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    reseller_id = table.Column<Guid>(type: "uuid", nullable: true),
                    licensee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_departments", x => x.id);
                    table.ForeignKey(
                        name: "fk_departments_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_departments_licensees_licensee_id",
                        column: x => x.licensee_id,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_departments_resellers_reseller_id",
                        column: x => x.reseller_id,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "station_letters",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    content = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    remarks = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    assigned_all_players = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    verified_by_username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_station_letters", x => x.id);
                    table.ForeignKey(
                        name: "fk_station_letters_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vip_levels",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    level_up_accumulate_bet_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    level_up_accumulate_deposit = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    level_keep_accumulate_bet_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    level_keep_accumulate_deposit = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    level_keep_condition = table.Column<bool>(type: "boolean", nullable: false),
                    birth_reward = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    promotion_reward = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    quarterly_reward = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    live_dealer_rebate = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    slot_rebate = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    card_game_rebate = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    sports_rebate = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    virtual_sports_rebate = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vip_levels", x => x.id);
                    table.ForeignKey(
                        name: "fk_vip_levels_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "whitelists",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    reseller_id = table.Column<Guid>(type: "uuid", nullable: true),
                    licensee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    internal_note = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_whitelists", x => x.id);
                    table.ForeignKey(
                        name: "fk_whitelists_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_whitelists_licensees_licensee_id",
                        column: x => x.licensee_id,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_whitelists_resellers_reseller_id",
                        column: x => x.reseller_id,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "brand_game_provider_schedules",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    day_of_week = table.Column<int>(type: "integer", nullable: true),
                    is_default = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_game_provider_schedule", x => x.id);
                    table.ForeignKey(
                        name: "fk_brand_game_provider_schedule_brand_game_providers_id",
                        column: x => x.brand_game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "brand_game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_group_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bet_limit_id = table.Column<int>(type: "integer", nullable: false),
                    brand_group_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_group_bet_limits", x => x.id);
                    table.UniqueConstraint("ak_brand_group_bet_limits_brand_group_id_bet_limit_id", x => new { x.brand_group_id, x.bet_limit_id });
                    table.ForeignKey(
                        name: "fk_brand_group_bet_limits_bet_limits_bet_limit_id",
                        column: x => x.bet_limit_id,
                        principalSchema: "dbo",
                        principalTable: "bet_limits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_group_bet_limits_brand_groups_brand_group_id",
                        column: x => x.brand_group_id,
                        principalSchema: "dbo",
                        principalTable: "brand_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_group_games",
                schema: "dbo",
                columns: table => new
                {
                    brand_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_game_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_group_games", x => new { x.brand_group_id, x.brand_game_id });
                    table.ForeignKey(
                        name: "fk_brand_group_games_brand_games_brand_game_id",
                        column: x => x.brand_game_id,
                        principalSchema: "dbo",
                        principalTable: "brand_games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_group_games_brand_groups_brand_group_id",
                        column: x => x.brand_group_id,
                        principalSchema: "dbo",
                        principalTable: "brand_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    department_id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_login_ip = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    last_login_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    time_zone = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    password = table.Column<byte[]>(type: "bytea", nullable: false),
                    salt = table.Column<byte[]>(type: "bytea", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "dbo",
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_users_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "dbo",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_payment_vip_levels",
                schema: "dbo",
                columns: table => new
                {
                    brand_payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vip_level_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_payment_vip_levels", x => new { x.brand_payment_id, x.vip_level_id });
                    table.ForeignKey(
                        name: "fk_brand_payment_vip_levels_brand_payments_brand_payment_id",
                        column: x => x.brand_payment_id,
                        principalSchema: "dbo",
                        principalTable: "brand_payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_payment_vip_levels_vip_levels_vip_level_id",
                        column: x => x.vip_level_id,
                        principalSchema: "dbo",
                        principalTable: "vip_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "players",
                schema: "dbo",
                columns: table => new
                {
                    player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    id_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    real_name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    invitation_code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    password = table.Column<byte[]>(type: "bytea", nullable: false),
                    salt = table.Column<byte[]>(type: "bytea", nullable: false),
                    withdraw_password = table.Column<byte[]>(type: "bytea", nullable: false),
                    withdraw_salt = table.Column<byte[]>(type: "bytea", nullable: false),
                    default_wallet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    default_wallet_currency_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    registered_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_active_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    bet_limit_id = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<byte>(type: "smallint", nullable: false),
                    email = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    qq = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    wechat = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    has_agent = table.Column<bool>(type: "boolean", nullable: false),
                    identity_authentication = table.Column<bool>(type: "boolean", nullable: false),
                    phone_authentication = table.Column<bool>(type: "boolean", nullable: false),
                    accumulated_deposit = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false, defaultValue: 0m),
                    parent_player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    parent_invitation_code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    vip_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by_username = table.Column<string>(type: "text", nullable: true),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    first_deposit_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_code", x => x.player_code);
                    table.ForeignKey(
                        name: "fk_players_bet_limits_bet_limit_id",
                        column: x => x.bet_limit_id,
                        principalSchema: "dbo",
                        principalTable: "bet_limits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_players_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_players_parent_player_code",
                        column: x => x.parent_player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_players_vip_levels_vip_id",
                        column: x => x.vip_id,
                        principalSchema: "dbo",
                        principalTable: "vip_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "brand_group_game_provider_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    brand_group_bet_limit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_provider_bet_limit_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_group_game_provider_bet_limits", x => new { x.brand_group_bet_limit_id, x.game_provider_bet_limit_id });
                    table.ForeignKey(
                        name: "fk_brand_group_game_provider_bet_limits_brand_group_bet_limits_brand_group_bet_limit_id",
                        column: x => x.brand_group_bet_limit_id,
                        principalSchema: "dbo",
                        principalTable: "brand_group_bet_limits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_group_game_provider_bet_limits_game_provider_bet_limits_game_provider_bet_limit_id",
                        column: x => x.game_provider_bet_limit_id,
                        principalSchema: "dbo",
                        principalTable: "game_provider_bet_limits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand_payment_players",
                schema: "dbo",
                columns: table => new
                {
                    player_code = table.Column<string>(type: "character varying(16)", nullable: false),
                    brand_payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    list_type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_payment_players", x => new { x.brand_payment_id, x.player_code });
                    table.ForeignKey(
                        name: "fk_brand_payment_players_brand_payments_brand_payment_id",
                        column: x => x.brand_payment_id,
                        principalSchema: "dbo",
                        principalTable: "brand_payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_brand_payment_players_players_player_code",
                        column: x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "deposit_forms",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_account = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    brand_payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "Now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_deposit_forms", x => x.id);
                    table.ForeignKey(
                        name: "fk_deposit_forms_brand_payments_id",
                        column: x => x.brand_payment_id,
                        principalSchema: "dbo",
                        principalTable: "brand_payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_deposit_forms_players_playerCode",
                        column: x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_deposit_froms_brands_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_banks",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    real_name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    bank_name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    bank_account = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_banks", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_banks_players_player_code",
                        column: x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_login_records",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    platform_type = table.Column<short>(type: "smallint", nullable: false),
                    ip_address = table.Column<string>(type: "text", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expired_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_login_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_login_records_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_player_login_records_players_player_code",
                        column: x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "player_sports_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    player_code = table.Column<string>(type: "character varying(16)", nullable: false),
                    sport_market_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    min_bet = table.Column<int>(type: "integer", nullable: false),
                    max_bet = table.Column<int>(type: "integer", nullable: false),
                    max_bet_per_match = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_sports_bet_limits", x => new { x.player_code, x.sport_market_type_id });
                    table.ForeignKey(
                        name: "fk_player_sports_bet_limits_players_player_code",
                        column: x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_player_sports_bet_limits_sport_market_types_sport_market_type_id",
                        column: x => x.sport_market_type_id,
                        principalSchema: "dbo",
                        principalTable: "sport_market_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "station_letter_players",
                schema: "dbo",
                columns: table => new
                {
                    station_letter_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_code = table.Column<string>(type: "character varying(16)", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    read_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_station_letter_players", x => new { x.station_letter_id, x.player_code });
                    table.ForeignKey(
                        name: "fk_station_letter_players_players_player_code",
                        column: x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_station_letter_players_station_letters_station_letter_id",
                        column: x => x.station_letter_id,
                        principalSchema: "dbo",
                        principalTable: "station_letters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wallets",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    currency_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    external_currency_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    balance = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wallets", x => x.id);
                    table.ForeignKey(
                        name: "fk_wallets_currencies_currency_code",
                        column: x => x.currency_code,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_wallets_players_player_code",
                        column: x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    wallet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_transaction_id = table.Column<string>(type: "text", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    balance = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    recorded_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    sent_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    confirmed_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_transactions_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_transactions_wallets_wallet_id",
                        column: x => x.wallet_id,
                        principalSchema: "dbo",
                        principalTable: "wallets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_announcements_brand_id",
                schema: "dbo",
                table: "announcements",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_bet_limits_currency_code",
                schema: "dbo",
                table: "bet_limits",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                name: "ix_brand_bet_limits_bet_limit_id",
                schema: "dbo",
                table: "brand_bet_limits",
                column: "bet_limit_id");

            migrationBuilder.CreateIndex(
                name: "ix_brand_currencies_currency_code",
                schema: "dbo",
                table: "brand_currencies",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                name: "ix_brand_game_provider_schedule_brand_game_provider_id",
                schema: "dbo",
                table: "brand_game_provider_schedules",
                column: "brand_game_provider_id");

            migrationBuilder.CreateIndex(
                name: "ix_brand_game_providers_default_language_code",
                schema: "dbo",
                table: "brand_game_providers",
                column: "default_language_code");

            migrationBuilder.CreateIndex(
                name: "ix_brand_game_providers_game_provider_configuration_id",
                schema: "dbo",
                table: "brand_game_providers",
                column: "game_provider_configuration_id");

            migrationBuilder.CreateIndex(
                name: "ix_brand_game_providers_game_provider_id",
                schema: "dbo",
                table: "brand_game_providers",
                column: "game_provider_id");

            migrationBuilder.CreateIndex(
                name: "ux_brand_game_providers_brand_id_game_provider_id",
                schema: "dbo",
                table: "brand_game_providers",
                columns: new[] { "brand_id", "game_provider_id" },
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "game_provider_configuration_id", "status", "default_language_code", "desktop_query_string", "mobile_query_string" });

            migrationBuilder.CreateIndex(
                name: "ix_brand_games_game_id",
                schema: "dbo",
                table: "brand_games",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "ix_brand_group_bet_limits_bet_limit_id",
                schema: "dbo",
                table: "brand_group_bet_limits",
                column: "bet_limit_id");

            migrationBuilder.CreateIndex(
                name: "ix_brand_group_bet_limits_brand_group_id",
                schema: "dbo",
                table: "brand_group_bet_limits",
                column: "brand_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_brand_group_game_provider_bet_limits_game_provider_bet_limit_id",
                schema: "dbo",
                table: "brand_group_game_provider_bet_limits",
                column: "game_provider_bet_limit_id");

            migrationBuilder.CreateIndex(
                name: "ix_brand_group_games_brand_game_id",
                schema: "dbo",
                table: "brand_group_games",
                column: "brand_game_id");

            migrationBuilder.CreateIndex(
                name: "ix_brand_groups_brand_id",
                schema: "dbo",
                table: "brand_groups",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_brand_languages_culture_code",
                schema: "dbo",
                table: "brand_languages",
                column: "culture_code");

            migrationBuilder.CreateIndex(
                name: "ix_brand_payment_audit_logs_brand_payment_id",
                schema: "dbo",
                table: "brand_payment_audit_logs",
                column: "brand_payment_id");

            migrationBuilder.CreateIndex(
                name: "ix_brand_payment_players_player_code",
                schema: "dbo",
                table: "brand_payment_players",
                column: "player_code");

            migrationBuilder.CreateIndex(
                name: "ix_brand_payment_vip_levels_vip_level_id",
                schema: "dbo",
                table: "brand_payment_vip_levels",
                column: "vip_level_id");

            migrationBuilder.CreateIndex(
                name: "ix_brand_payments_brand_id",
                schema: "dbo",
                table: "brand_payments",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_brand_payments_payment_channel_id",
                schema: "dbo",
                table: "brand_payments",
                column: "payment_channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_brand_payments_payment_id",
                schema: "dbo",
                table: "brand_payments",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "ix_brand_payments_payment_method_id",
                schema: "dbo",
                table: "brand_payments",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "ix_brands_licensee_id",
                schema: "dbo",
                table: "brands",
                column: "licensee_id");

            migrationBuilder.CreateIndex(
                name: "ix_departments_brand_id",
                schema: "dbo",
                table: "departments",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_departments_licensee_id",
                schema: "dbo",
                table: "departments",
                column: "licensee_id");

            migrationBuilder.CreateIndex(
                name: "ix_departments_reseller_id",
                schema: "dbo",
                table: "departments",
                column: "reseller_id");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_forms_brandId",
                schema: "dbo",
                table: "deposit_forms",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_forms_brandPaymentId",
                schema: "dbo",
                table: "deposit_forms",
                column: "brand_payment_id");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_forms_playerCode",
                schema: "dbo",
                table: "deposit_forms",
                column: "player_code");

            migrationBuilder.CreateIndex(
                name: "ix_game_action_batches_game_provider_id",
                schema: "bet",
                table: "game_action_batches",
                column: "game_provider_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_actions_game_action_batch_id",
                schema: "bet",
                table: "game_actions",
                column: "game_action_batch_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_actions_referenced_game_action_id",
                schema: "bet",
                table: "game_actions",
                column: "referenced_game_action_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_actions_round_id",
                schema: "bet",
                table: "game_actions",
                column: "round_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_provider_bet_limits_currency_code",
                schema: "dbo",
                table: "game_provider_bet_limits",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                name: "ix_game_provider_bet_limits_game_provider_id",
                schema: "dbo",
                table: "game_provider_bet_limits",
                column: "game_provider_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_provider_configurations_game_provider_id",
                schema: "dbo",
                table: "game_provider_configurations",
                column: "game_provider_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_provider_currencies_currency_code",
                schema: "dbo",
                table: "game_provider_currencies",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                name: "ix_game_provider_languages_culture_code",
                schema: "dbo",
                table: "game_provider_languages",
                column: "culture_code");

            migrationBuilder.CreateIndex(
                name: "ix_games_game_provider_id",
                schema: "dbo",
                table: "games",
                column: "game_provider_id")
                .Annotation("Npgsql:IndexInclude", new[] { "id", "code", "name", "external_id", "platform_type", "type", "status", "available_currencies", "endpoint_path", "sub_game", "has_turnover_on_tie_bet", "rule_url", "description" });

            migrationBuilder.CreateIndex(
                name: "ix_licensee_bet_limits_bet_limit_id",
                schema: "dbo",
                table: "licensee_bet_limits",
                column: "bet_limit_id");

            migrationBuilder.CreateIndex(
                name: "ix_licensee_currencies_currency_code",
                schema: "dbo",
                table: "licensee_currencies",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                name: "ix_licensee_game_providers_game_provider_id",
                schema: "dbo",
                table: "licensee_game_providers",
                column: "game_provider_id");

            migrationBuilder.CreateIndex(
                name: "ix_licensee_languages_culture_code",
                schema: "dbo",
                table: "licensee_languages",
                column: "culture_code");

            migrationBuilder.CreateIndex(
                name: "ix_licensees_reseller_id",
                schema: "dbo",
                table: "licensees",
                column: "reseller_id");

            migrationBuilder.CreateIndex(
                name: "ix_localized_strings_culture_code",
                schema: "dbo",
                table: "localized_strings",
                column: "culture_code");

            migrationBuilder.CreateIndex(
                name: "ix_payment_channels_payment_id",
                schema: "dbo",
                table: "payment_channels",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_channels_payment_method_id",
                schema: "dbo",
                table: "payment_channels",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_methods_payment_id",
                schema: "dbo",
                table: "payment_methods",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_bankers_player_code",
                schema: "dbo",
                table: "player_banks",
                column: "player_code");

            migrationBuilder.CreateIndex(
                name: "ix_player_login_records_brand_id",
                schema: "dbo",
                table: "player_login_records",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_login_records_player_code",
                schema: "dbo",
                table: "player_login_records",
                column: "player_code");

            migrationBuilder.CreateIndex(
                name: "ix_player_sports_bet_limits_sport_market_id",
                schema: "dbo",
                table: "player_sports_bet_limits",
                column: "sport_market_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_players_bet_limit_id",
                schema: "dbo",
                table: "players",
                column: "bet_limit_id");

            migrationBuilder.CreateIndex(
                name: "ix_players_brand_id",
                schema: "dbo",
                table: "players",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_players_parent_player_code",
                schema: "dbo",
                table: "players",
                column: "parent_player_code");

            migrationBuilder.CreateIndex(
                name: "ix_players_vip_id",
                schema: "dbo",
                table: "players",
                column: "vip_id");

            migrationBuilder.CreateIndex(
                name: "ix_reseller_bet_limits_bet_limit_id",
                schema: "dbo",
                table: "reseller_bet_limits",
                column: "bet_limit_id");

            migrationBuilder.CreateIndex(
                name: "ix_reseller_currencies_currency_code",
                schema: "dbo",
                table: "reseller_currencies",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                name: "ix_reseller_game_providers_game_provider_id",
                schema: "dbo",
                table: "reseller_game_providers",
                column: "game_provider_id");

            migrationBuilder.CreateIndex(
                name: "ix_reseller_languages_culture_code",
                schema: "dbo",
                table: "reseller_languages",
                column: "culture_code");

            migrationBuilder.CreateIndex(
                name: "ix_rounds_brand_id",
                schema: "bet",
                table: "rounds",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_rounds_game_id",
                schema: "bet",
                table: "rounds",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "ix_rounds_game_provider_id",
                schema: "bet",
                table: "rounds",
                column: "game_provider_id");

            migrationBuilder.CreateIndex(
                name: "ix_rounds_player_code",
                schema: "bet",
                table: "rounds",
                column: "player_code");

            migrationBuilder.CreateIndex(
                name: "ux_rounds_external_round_id_composite",
                schema: "bet",
                table: "rounds",
                columns: new[] { "external_round_id", "player_code", "game_provider_id", "game_id" },
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "game_provider_configuration_id", "wallet_id" });

            migrationBuilder.CreateIndex(
                name: "ix_sport_market_types_market_type_id",
                schema: "dbo",
                table: "sport_market_types",
                column: "market_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_sport_types_game_provider_id",
                schema: "dbo",
                table: "sport_types",
                column: "game_provider_id");

            migrationBuilder.CreateIndex(
                name: "ix_station_letter_players_player_code",
                schema: "dbo",
                table: "station_letter_players",
                column: "player_code");

            migrationBuilder.CreateIndex(
                name: "ix_station_letters_brand_id",
                schema: "dbo",
                table: "station_letters",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_brand_id",
                schema: "dbo",
                table: "transactions",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_wallet_id",
                schema: "dbo",
                table: "transactions",
                column: "wallet_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_department_id",
                schema: "dbo",
                table: "users",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_role_id",
                schema: "dbo",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_vip_levels_brand_id",
                schema: "dbo",
                table: "vip_levels",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_wallets_currency_code",
                schema: "dbo",
                table: "wallets",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                name: "ix_wallets_player_code",
                schema: "dbo",
                table: "wallets",
                column: "player_code");

            migrationBuilder.CreateIndex(
                name: "ix_whitelists_brand_id",
                schema: "dbo",
                table: "whitelists",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_whitelists_licensee_id",
                schema: "dbo",
                table: "whitelists",
                column: "licensee_id");

            migrationBuilder.CreateIndex(
                name: "ix_whitelists_reseller_id",
                schema: "dbo",
                table: "whitelists",
                column: "reseller_id");

            InitAfterRun(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "payment_channels",
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
                name: "payment_methods",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "game_providers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "bet_limits",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "vip_levels",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "payments",
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
