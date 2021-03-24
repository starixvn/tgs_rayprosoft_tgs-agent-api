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
                "dbo");

            migrationBuilder.EnsureSchema(
                "bet");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateTable(
                "brand_payment_audit_logs",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    brand_payment_id = table.Column<Guid>("uuid", nullable: false),
                    before = table.Column<string>("jsonb", nullable: false),
                    after = table.Column<string>("jsonb", nullable: false),
                    reason = table.Column<string>("character varying(4000)", maxLength: 4000, nullable: true),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    operation = table.Column<short>("smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_payment_audit_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                "currencies",
                schema: "dbo",
                columns: table => new
                {
                    code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                    native_name = table.Column<string>("character varying(64)", maxLength: 64, nullable: true),
                    symbol = table.Column<string>("character varying(6)", maxLength: 6, nullable: true),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username =
                        table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username =
                        table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_currencies", x => x.code);
                });

            migrationBuilder.CreateTable(
                "game_providers",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    status = table.Column<short>("smallint", nullable: false),
                    description = table.Column<string>("character varying(128)", maxLength: 128, nullable: true),
                    auto_close_round = table.Column<bool>("boolean", nullable: false),
                    support_test_player = table.Column<bool>("boolean", nullable: false),
                    allow_kick_player = table.Column<bool>("boolean", nullable: false),
                    place_bet_timeout = table.Column<short>("smallint", nullable: false, defaultValue: (short)0),
                    settle_bet_timeout = table.Column<short>("smallint", nullable: false, defaultValue: (short)0),
                    unique_id = table.Column<short>("smallint", nullable: false),
                    code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>("character varying(255)", maxLength: 255, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_providers", x => x.id);
                });

            migrationBuilder.CreateTable(
                "languages",
                schema: "dbo",
                columns: table => new
                {
                    culture_code = table.Column<string>("character varying(6)", maxLength: 6, nullable: false),
                    language_code = table.Column<string>("character varying(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                    localized_name = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username =
                        table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username =
                        table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_languages", x => x.culture_code);
                });

            migrationBuilder.CreateTable(
                "payments",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    description = table.Column<string>("character varying(128)", maxLength: 128, nullable: true),
                    code = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>("text", nullable: true),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payments", x => x.id);
                });

            migrationBuilder.CreateTable(
                "player_audit_logs",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    module_name = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                    username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    operation = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_audit_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                "resellers",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    status = table.Column<short>("smallint", nullable: false),
                    description = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    operational_email = table.Column<string>("character varying(128)", maxLength: 128, nullable: true),
                    business_type = table.Column<short>("smallint", nullable: false),
                    code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>("character varying(255)", maxLength: 255, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_resellers", x => x.id);
                });

            migrationBuilder.CreateTable(
                "roles",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    role_type = table.Column<short>("smallint", nullable: false),
                    reseller_id = table.Column<Guid>("uuid", nullable: true),
                    licensee_id = table.Column<Guid>("uuid", nullable: true),
                    brand_id = table.Column<Guid>("uuid", nullable: true),
                    is_root = table.Column<bool>("boolean", nullable: false),
                    name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                    display_name = table.Column<string>("character varying(50)", maxLength: 50, nullable: true),
                    menu_setting = table.Column<string>("jsonb", nullable: false),
                    description = table.Column<string>("character varying(128)", maxLength: 128, nullable: true),
                    status = table.Column<short>("smallint", nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                "rounds",
                schema: "bet",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    external_round_id = table.Column<string>("character varying(255)", maxLength: 255, nullable: false),
                    game_provider_id = table.Column<Guid>("uuid", nullable: false),
                    player_code = table.Column<string>("text", nullable: false),
                    game_id = table.Column<int>("integer", nullable: false),
                    currency_code = table.Column<string>("character varying(10)", maxLength: 10, nullable: false),
                    status = table.Column<short>("smallint", nullable: false),
                    game_provider_configuration_id = table.Column<Guid>("uuid", nullable: false),
                    wallet_id = table.Column<Guid>("uuid", nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    closed_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    external_created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true),
                    external_closed_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true),
                    bet_amount = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    valid_bet_amount = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    settle_amount = table.Column<decimal>("numeric", nullable: true),
                    commission = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    turnover = table.Column<decimal>("numeric", nullable: true),
                    win_loss = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    tie_bet_adjustment =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    client_status = table.Column<short>("smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rounds", x => x.id);
                });

            migrationBuilder.CreateTable(
                "settings",
                schema: "dbo",
                columns: table => new
                {
                    key = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    value = table.Column<string>("text", nullable: false),
                    description = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username =
                        table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username =
                        table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_settings", x => x.key);
                });

            migrationBuilder.CreateTable(
                "bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    currency_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    min_bet = table.Column<int>("integer", nullable: false),
                    max_bet = table.Column<int>("integer", nullable: false),
                    max_bet_per_match = table.Column<int>("integer", nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bet_limits", x => x.id);
                    table.ForeignKey(
                        "fk_bet_limits_currencies_currency_code",
                        x => x.currency_code,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "game_action_batches",
                schema: "bet",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    external_id = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                    error = table.Column<short>("smallint", nullable: false),
                    error_message = table.Column<string>("character varying(1024)", maxLength: 1024, nullable: true),
                    transactions_count = table.Column<int>("integer", nullable: false),
                    errors_json = table.Column<string>("jsonb", nullable: true),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    game_provider_id = table.Column<Guid>("uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_action_batches", x => x.id);
                    table.ForeignKey(
                        "fk_game_action_batches_game_provider_configurations_game_provider_id",
                        x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "game_provider_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    game_provider_id = table.Column<Guid>("uuid", nullable: false),
                    currency_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: true),
                    min_bet = table.Column<int>("integer", nullable: false),
                    max_bet = table.Column<int>("integer", nullable: false),
                    max_bet_per_match = table.Column<int>("integer", nullable: true),
                    external_setting = table.Column<string>("character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_provider_bet_limits", x => x.id);
                    table.ForeignKey(
                        "fk_game_provider_bet_limits_currencies_currency_code",
                        x => x.currency_code,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_game_provider_bet_limits_game_providers_game_provider_id",
                        x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "game_provider_configurations",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    name = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                    description = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    client_id = table.Column<string>("character varying(128)", maxLength: 128, nullable: false),
                    secret_key = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<short>("smallint", nullable: false),
                    game_provider_id = table.Column<Guid>("uuid", nullable: false),
                    desktop_endpoint =
                        table.Column<string>("character varying(1024)", maxLength: 1024, nullable: false),
                    mobile_endpoint = table.Column<string>("character varying(1024)", maxLength: 1024, nullable: false),
                    configuration_data = table.Column<string>("text", nullable: false),
                    is_default = table.Column<bool>("boolean", nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_provider_configurations", x => x.id);
                    table.ForeignKey(
                        "fk_game_provider_configurations_game_providers_game_provider_id",
                        x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "game_provider_currencies",
                schema: "dbo",
                columns: table => new
                {
                    game_provider_id = table.Column<Guid>("uuid", nullable: false),
                    currency_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    game_provider_currency_code =
                        table.Column<string>("character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_provider_currencies", x => new
                    {
                        x.game_provider_id,
                        x.currency_code
                    });
                    table.ForeignKey(
                        "fk_game_provider_currencies_currencies_currency_code",
                        x => x.currency_code,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_game_provider_currencies_game_providers_game_provider_id",
                        x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "games",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>("integer", nullable: false),
                    status = table.Column<short>("smallint", nullable: false),
                    external_id = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    description = table.Column<string>("character varying(128)", maxLength: 128, nullable: true),
                    endpoint_path = table.Column<string>("character varying(2048)", maxLength: 2048, nullable: true),
                    rule_url = table.Column<string>("character varying(512)", maxLength: 512, nullable: true),
                    game_provider_id = table.Column<Guid>("uuid", nullable: false),
                    type = table.Column<short>("smallint", nullable: false),
                    platform_type = table.Column<byte>("smallint", nullable: false),
                    sub_game = table.Column<bool>("boolean", nullable: false),
                    available_to_brand = table.Column<bool>("boolean", nullable: false),
                    has_turnover_on_tie_bet = table.Column<bool>("boolean", nullable: false),
                    html5 = table.Column<bool>("boolean", nullable: false),
                    available_currencies =
                        table.Column<string>("character varying(1024)", maxLength: 1024, nullable: false,
                            defaultValue: ""),
                    code = table.Column<string>("character varying(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>("character varying(255)", maxLength: 255, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_games", x => x.id);
                    table.ForeignKey(
                        "fk_games_game_providers_game_provider_id",
                        x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "market_types",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    game_provider_id = table.Column<Guid>("uuid", nullable: false),
                    code = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                    external_id = table.Column<string>("character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_market_types", x => x.id);
                    table.UniqueConstraint("ak_market_types_game_provider_id_code", x => new
                    {
                        x.game_provider_id,
                        x.code
                    });
                    table.ForeignKey(
                        "fk_market_types_game_providers_game_provider_id",
                        x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "sport_types",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    game_provider_id = table.Column<Guid>("uuid", nullable: false),
                    code = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                    external_id = table.Column<string>("character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sport_types", x => x.id);
                    table.UniqueConstraint("ak_sport_types_game_provider_id_code", x => new
                    {
                        x.game_provider_id,
                        x.code
                    });
                    table.ForeignKey(
                        "fk_sport_types_game_providers_game_provider_id",
                        x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "game_provider_languages",
                schema: "dbo",
                columns: table => new
                {
                    game_provider_id = table.Column<Guid>("uuid", nullable: false),
                    culture_code = table.Column<string>("character varying(6)", maxLength: 6, nullable: false),
                    game_provider_culture_code =
                        table.Column<string>("character varying(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_provider_languages", x => new
                    {
                        x.game_provider_id,
                        x.culture_code
                    });
                    table.ForeignKey(
                        "fk_game_provider_languages_game_providers_game_provider_id",
                        x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_game_provider_languages_languages_culture_code",
                        x => x.culture_code,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "culture_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "localized_strings",
                schema: "dbo",
                columns: table => new
                {
                    context = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                    key = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                    culture_code = table.Column<string>("character varying(6)", maxLength: 6, nullable: false),
                    translation = table.Column<string>("character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_localized_strings", x => new
                    {
                        x.key,
                        x.culture_code,
                        x.context
                    });
                    table.ForeignKey(
                        "fk_localized_strings_languages_culture_code",
                        x => x.culture_code,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "culture_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "payment_methods",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    description = table.Column<string>("character varying(128)", maxLength: 128, nullable: true),
                    payment_id = table.Column<Guid>("uuid", nullable: false),
                    code = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>("text", nullable: true),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_methods", x => x.id);
                    table.ForeignKey(
                        "fk_payment_methods_payments_payment_id",
                        x => x.payment_id,
                        principalSchema: "dbo",
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "licensees",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    status = table.Column<short>("smallint", nullable: false),
                    description = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    operational_email = table.Column<string>("character varying(128)", maxLength: 128, nullable: true),
                    reseller_id = table.Column<Guid>("uuid", nullable: false),
                    code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>("character varying(255)", maxLength: 255, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_licensees", x => x.id);
                    table.ForeignKey(
                        "fk_licensees_resellers_reseller_id",
                        x => x.reseller_id,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "reseller_currencies",
                schema: "dbo",
                columns: table => new
                {
                    reseller_id = table.Column<Guid>("uuid", nullable: false),
                    currency_code = table.Column<string>("character varying(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reseller_currencies", x => new
                    {
                        x.reseller_id,
                        x.currency_code
                    });
                    table.ForeignKey(
                        "fk_reseller_currencies_currencies_currency_code",
                        x => x.currency_code,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_reseller_currencies_resellers_reseller_id",
                        x => x.reseller_id,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "reseller_game_providers",
                schema: "dbo",
                columns: table => new
                {
                    reseller_id = table.Column<Guid>("uuid", nullable: false),
                    game_provider_id = table.Column<Guid>("uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reseller_game_providers", x => new
                    {
                        x.reseller_id,
                        x.game_provider_id
                    });
                    table.ForeignKey(
                        "fk_reseller_game_providers_game_providers_game_provider_id",
                        x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_reseller_game_providers_game_resellers_reseller_id",
                        x => x.reseller_id,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "reseller_languages",
                schema: "dbo",
                columns: table => new
                {
                    reseller_id = table.Column<Guid>("uuid", nullable: false),
                    culture_code = table.Column<string>("character varying(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reseller_languages", x => new
                    {
                        x.reseller_id,
                        x.culture_code
                    });
                    table.ForeignKey(
                        "fk_reseller_languages_languages_culture_code",
                        x => x.culture_code,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "culture_code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_reseller_languages_resellers_reseller_id",
                        x => x.reseller_id,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "round_details",
                schema: "bet",
                columns: table => new
                {
                    round_id = table.Column<Guid>("uuid", nullable: false),
                    additional_data = table.Column<string>("text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_round_details", x => x.round_id);
                    table.ForeignKey(
                        "fk_round_details_rounds_round_id",
                        x => x.round_id,
                        principalSchema: "bet",
                        principalTable: "rounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "reseller_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    bet_limit_id = table.Column<int>("integer", nullable: false),
                    reseller_id = table.Column<Guid>("uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reseller_bet_limits", x => new
                    {
                        x.reseller_id,
                        x.bet_limit_id
                    });
                    table.ForeignKey(
                        "fk_reseller_bet_limits_resellers_bet_limit_id",
                        x => x.bet_limit_id,
                        principalSchema: "dbo",
                        principalTable: "bet_limits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_reseller_bet_limits_resellers_reseller_id",
                        x => x.reseller_id,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "game_actions",
                schema: "bet",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    round_id = table.Column<Guid>("uuid", nullable: false),
                    external_transaction_id =
                        table.Column<string>("character varying(128)", maxLength: 128, nullable: false),
                    type = table.Column<short>("smallint", nullable: false),
                    amount = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    turnover = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    win_loss = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    unsettled_bets = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    balance = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    bet_type = table.Column<string>("character varying(128)", maxLength: 128, nullable: true),
                    external_bet_id = table.Column<string>("character varying(128)", maxLength: 128, nullable: false),
                    external_game_action_group_id =
                        table.Column<string>("character varying(128)", maxLength: 128, nullable: false),
                    referenced_game_action_id = table.Column<Guid>("uuid", nullable: true),
                    game_action_batch_id = table.Column<Guid>("uuid", nullable: true),
                    recorded_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    sent_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true),
                    is_closing_round = table.Column<bool>("boolean", nullable: false),
                    closing_round_explicit_request = table.Column<bool>("boolean", nullable: true),
                    player_ip = table.Column<string>("character varying(128)", maxLength: 128, nullable: false),
                    description = table.Column<string>("text", nullable: true),
                    stored_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false,
                        defaultValueSql: "now() at time zone 'utc'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_actions", x => x.id);
                    table.ForeignKey(
                        "fk_game_actions_game_action_batches_game_action_batch_id",
                        x => x.game_action_batch_id,
                        principalSchema: "bet",
                        principalTable: "game_action_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_game_actions_referenced_game_action_id",
                        x => x.referenced_game_action_id,
                        principalSchema: "bet",
                        principalTable: "game_actions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_game_actions_rounds_round_id",
                        x => x.round_id,
                        principalSchema: "bet",
                        principalTable: "rounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "sport_market_types",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    sport_type_id = table.Column<Guid>("uuid", nullable: false),
                    market_type_id = table.Column<Guid>("uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sport_market_types", x => x.id);
                    table.UniqueConstraint("ak_sport_market_types_sport_type_id_market_type_id",
                        x => new
                        {
                            x.sport_type_id,
                            x.market_type_id
                        });
                    table.ForeignKey(
                        "fk_sport_market_types_market_types_market_type_id",
                        x => x.market_type_id,
                        principalSchema: "dbo",
                        principalTable: "market_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_sport_market_types_sport_types_sport_type_id",
                        x => x.sport_type_id,
                        principalSchema: "dbo",
                        principalTable: "sport_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "payment_channels",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    description = table.Column<string>("character varying(128)", maxLength: 128, nullable: true),
                    payment_id = table.Column<Guid>("uuid", nullable: false),
                    payment_method_id = table.Column<Guid>("uuid", nullable: false),
                    code = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>("text", nullable: true),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_channels", x => x.id);
                    table.ForeignKey(
                        "fk_payment_channels_payment_methods_payment_method_id",
                        x => x.payment_method_id,
                        principalSchema: "dbo",
                        principalTable: "payment_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_payment_channels_payments_payment_id",
                        x => x.payment_id,
                        principalSchema: "dbo",
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "brands",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    status = table.Column<short>("smallint", nullable: false),
                    description = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    operational_email = table.Column<string>("character varying(128)", maxLength: 128, nullable: true),
                    time_zone_id = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    licensee_id = table.Column<Guid>("uuid", nullable: false),
                    client_id = table.Column<string>("character varying(128)", maxLength: 128, nullable: false),
                    secret_key = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    api_key = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                    code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>("character varying(255)", maxLength: 255, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brands", x => x.id);
                    table.ForeignKey(
                        "fk_brands_licensees_licensee_id",
                        x => x.licensee_id,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "licensee_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    bet_limit_id = table.Column<int>("integer", nullable: false),
                    licensee_id = table.Column<Guid>("uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_licensee_bet_limits", x => new
                    {
                        x.licensee_id,
                        x.bet_limit_id
                    });
                    table.ForeignKey(
                        "fk_licensee_bet_limits_bet_limits_bet_limit_id",
                        x => x.bet_limit_id,
                        principalSchema: "dbo",
                        principalTable: "bet_limits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_licensee_bet_limits_licensees_licensee_id",
                        x => x.licensee_id,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "licensee_currencies",
                schema: "dbo",
                columns: table => new
                {
                    licensee_id = table.Column<Guid>("uuid", nullable: false),
                    currency_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_licensee_currencies", x => new
                    {
                        x.licensee_id,
                        x.currency_code
                    });
                    table.ForeignKey(
                        "fk_licensee_currencies_currencies_currency_code",
                        x => x.currency_code,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_licensee_currencies_licensees_licensee_id",
                        x => x.licensee_id,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "licensee_game_providers",
                schema: "dbo",
                columns: table => new
                {
                    licensee_id = table.Column<Guid>("uuid", nullable: false),
                    game_provider_id = table.Column<Guid>("uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_licensee_game_providers", x => new
                    {
                        x.licensee_id,
                        x.game_provider_id
                    });
                    table.ForeignKey(
                        "fk_licensee_game_providers_game_providers_game_provider_id",
                        x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_licensee_game_providers_licensees_licensee_id",
                        x => x.licensee_id,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "licensee_languages",
                schema: "dbo",
                columns: table => new
                {
                    licensee_id = table.Column<Guid>("uuid", nullable: false),
                    culture_code = table.Column<string>("character varying(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_licensee_languages", x => new
                    {
                        x.licensee_id,
                        x.culture_code
                    });
                    table.ForeignKey(
                        "fk_licensee_languages_languages_culture_code",
                        x => x.culture_code,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "culture_code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_licensee_languages_licensees_licensee_id",
                        x => x.licensee_id,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "agents",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    real_name = table.Column<string>("character varying(32)", maxLength: 32, nullable: true),
                    password = table.Column<byte[]>("bytea", nullable: false),
                    salt = table.Column<byte[]>("bytea", nullable: false),
                    withdraw_password = table.Column<byte[]>("bytea", nullable: false),
                    withdraw_salt = table.Column<byte[]>("bytea", nullable: false),
                    status = table.Column<short>("smallint", nullable: false),
                    agent_status = table.Column<int>("integer", nullable: false),
                    level_type = table.Column<int>("integer", nullable: false),
                    type = table.Column<short>("smallint", nullable: false),
                    invitation_code = table.Column<string>("character varying(6)", maxLength: 6, nullable: false),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    parent_agent_id = table.Column<Guid>("uuid", nullable: true),
                    registered_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    position_taking_percentage = table.Column<decimal>("numeric", nullable: false),
                    bonus_percentage = table.Column<decimal>("numeric", nullable: false),
                    rebate_percentage = table.Column<decimal>("numeric", nullable: false),
                    remarks = table.Column<string>("text", nullable: true),
                    email = table.Column<string>("character varying(64)", maxLength: 64, nullable: true),
                    phone = table.Column<string>("character varying(32)", maxLength: 32, nullable: true),
                    qq = table.Column<string>("character varying(64)", maxLength: 64, nullable: true),
                    wechat = table.Column<string>("character varying(64)", maxLength: 64, nullable: true),
                    created_by_username = table.Column<string>("text", nullable: true),
                    updated_by_username = table.Column<string>("text", nullable: true),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agents", x => x.id);
                    table.ForeignKey(
                        "fk_agents_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_agents_parent_agent_id",
                        x => x.parent_agent_id,
                        principalSchema: "dbo",
                        principalTable: "agents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "announcements",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    subject = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    content = table.Column<string>("character varying(4000)", maxLength: 4000, nullable: true),
                    remarks = table.Column<string>("character varying(512)", maxLength: 512, nullable: true),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    status = table.Column<int>("integer", nullable: false),
                    from = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    to = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    display_order = table.Column<short>("smallint", nullable: false),
                    verified_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: true),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_announcements", x => x.id);
                    table.ForeignKey(
                        "fk_announcements_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "brand_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    bet_limit_id = table.Column<int>("integer", nullable: false),
                    is_default = table.Column<bool>("boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_bet_limits", x => new
                    {
                        x.brand_id,
                        x.bet_limit_id
                    });
                    table.ForeignKey(
                        "fk_brand_bet_limits_bet_limits_bet_limit_id",
                        x => x.bet_limit_id,
                        principalSchema: "dbo",
                        principalTable: "bet_limits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_brand_bet_limits_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "brand_currencies",
                schema: "dbo",
                columns: table => new
                {
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    currency_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    brand_currency_code = table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_currencies", x => new
                    {
                        x.brand_id,
                        x.currency_code
                    });
                    table.ForeignKey(
                        "fk_brand_currencies_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_brand_currencies_currencies_currency_code",
                        x => x.currency_code,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "brand_game_providers",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    game_provider_id = table.Column<Guid>("uuid", nullable: false),
                    status = table.Column<short>("smallint", nullable: false),
                    game_provider_configuration_id = table.Column<Guid>("uuid", nullable: false),
                    default_language_code = table.Column<string>("character varying(6)", maxLength: 6, nullable: false),
                    desktop_query_string =
                        table.Column<string>("character varying(512)", maxLength: 512, nullable: false,
                            defaultValue: ""),
                    mobile_query_string =
                        table.Column<string>("character varying(512)", maxLength: 512, nullable: false,
                            defaultValue: ""),
                    is_manual_change_status = table.Column<bool>("boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_game_providers", x => x.id);
                    table.ForeignKey(
                        "fk_brand_game_providers_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_brand_game_providers_game_provider_configurations_game_provider_configuration_id",
                        x => x.game_provider_configuration_id,
                        principalSchema: "dbo",
                        principalTable: "game_provider_configurations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_brand_game_providers_game_providers_game_provider_id",
                        x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_brand_game_providers_languages_default_language_code",
                        x => x.default_language_code,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "culture_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "brand_games",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    game_id = table.Column<int>("integer", nullable: false),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    status = table.Column<short>("smallint", nullable: false),
                    tag = table.Column<byte>("smallint", nullable: false),
                    display_order = table.Column<int>("integer", nullable: false),
                    rebate = table.Column<bool>("boolean", nullable: false),
                    effective_betting_percentage =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_games", x => x.id);
                    table.UniqueConstraint("ak_brand_games_brand_id_game_id", x => new
                    {
                        x.brand_id,
                        x.game_id
                    });
                    table.ForeignKey(
                        "fk_brand_games_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_brand_games_games_game_id",
                        x => x.game_id,
                        principalSchema: "dbo",
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "brand_groups",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    name = table.Column<string>("character varying(255)", maxLength: 255, nullable: false),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    description = table.Column<string>("text", nullable: true),
                    is_sports = table.Column<bool>("boolean", nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_groups", x => x.id);
                    table.ForeignKey(
                        "fk_brand_groups_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "brand_languages",
                schema: "dbo",
                columns: table => new
                {
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    culture_code = table.Column<string>("character varying(6)", maxLength: 6, nullable: false),
                    brand_culture_code = table.Column<string>("character varying(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_languages", x => new
                    {
                        x.brand_id,
                        x.culture_code
                    });
                    table.ForeignKey(
                        "fk_brand_languages_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_brand_languages_languages_culture_code",
                        x => x.culture_code,
                        principalSchema: "dbo",
                        principalTable: "languages",
                        principalColumn: "culture_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "brand_payments",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    display_order = table.Column<int>("integer", nullable: false),
                    payment_id = table.Column<Guid>("uuid", nullable: false),
                    payment_method_id = table.Column<Guid>("uuid", nullable: false),
                    payment_channel_id = table.Column<Guid>("uuid", nullable: false),
                    transfer_type = table.Column<int>("integer", nullable: false),
                    channel_name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                    payment_status = table.Column<int>("integer", nullable: false),
                    transfer_limit_from =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    transfer_limit_to =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    player_list_condition = table.Column<int>("integer", nullable: false),
                    all_players = table.Column<bool>("boolean", nullable: false),
                    identity_authentication = table.Column<bool>("boolean", nullable: false),
                    phone_authentication = table.Column<bool>("boolean", nullable: false),
                    deposited_amount_unlimited = table.Column<bool>("boolean", nullable: false),
                    deposited_amount_min =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    deposited_amount_max =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    reason = table.Column<string>("character varying(4000)", maxLength: 4000, nullable: true),
                    fee_percentage = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    lowest_fee = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    highest_fee = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_payments", x => x.id);
                    table.ForeignKey(
                        "fk_brand_payments_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_brand_payments_payment_channels_payment_channel_id",
                        x => x.payment_channel_id,
                        principalSchema: "dbo",
                        principalTable: "payment_channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_brand_payments_payment_methods_payment_method_id",
                        x => x.payment_method_id,
                        principalSchema: "dbo",
                        principalTable: "payment_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_brand_payments_payments_payment_id",
                        x => x.payment_id,
                        principalSchema: "dbo",
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "departments",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>("character varying(128)", maxLength: 128, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    reseller_id = table.Column<Guid>("uuid", nullable: true),
                    licensee_id = table.Column<Guid>("uuid", nullable: true),
                    brand_id = table.Column<Guid>("uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_departments", x => x.id);
                    table.ForeignKey(
                        "fk_departments_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_departments_licensees_licensee_id",
                        x => x.licensee_id,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_departments_resellers_reseller_id",
                        x => x.reseller_id,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "station_letters",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    subject = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    content = table.Column<string>("character varying(4000)", maxLength: 4000, nullable: true),
                    remarks = table.Column<string>("character varying(512)", maxLength: 512, nullable: true),
                    assigned_all_players = table.Column<bool>("boolean", nullable: false),
                    status = table.Column<int>("integer", nullable: false),
                    verified_by_username =
                        table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_station_letters", x => x.id);
                    table.ForeignKey(
                        "fk_station_letters_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "vip_levels",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    level = table.Column<int>("integer", nullable: false),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    level_up_accumulate_bet_amount =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    level_up_accumulate_deposit =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    level_keep_accumulate_bet_amount =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    level_keep_accumulate_deposit =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    level_keep_condition = table.Column<bool>("boolean", nullable: false),
                    birth_reward = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    promotion_reward = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    quarterly_reward = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    live_dealer_rebate =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    slot_rebate = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    card_game_rebate = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    sports_rebate = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    virtual_sports_rebate =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vip_levels", x => x.id);
                    table.ForeignKey(
                        "fk_vip_levels_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "whitelists",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    reseller_id = table.Column<Guid>("uuid", nullable: true),
                    licensee_id = table.Column<Guid>("uuid", nullable: true),
                    brand_id = table.Column<Guid>("uuid", nullable: true),
                    ip_address = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                    description = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                    internal_note = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    type = table.Column<short>("smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_whitelists", x => x.id);
                    table.ForeignKey(
                        "fk_whitelists_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_whitelists_licensees_licensee_id",
                        x => x.licensee_id,
                        principalSchema: "dbo",
                        principalTable: "licensees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_whitelists_resellers_reseller_id",
                        x => x.reseller_id,
                        principalSchema: "dbo",
                        principalTable: "resellers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "agent_audit_logs",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    agent_id = table.Column<Guid>("uuid", nullable: false),
                    operation = table.Column<short>("smallint", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agent_audit_logs", x => x.id);
                    table.ForeignKey(
                        "fk_agent_audit_logs_agent_id",
                        x => x.agent_id,
                        principalSchema: "dbo",
                        principalTable: "agents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "agent_banks",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    real_name = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    bank_name = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    bank_account = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    agent_id = table.Column<Guid>("uuid", nullable: false),
                    created_by_username = table.Column<string>("text", nullable: true),
                    updated_by_username = table.Column<string>("text", nullable: true),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agent_banks", x => x.id);
                    table.ForeignKey(
                        "fk_agent_banks_agents_agent_id",
                        x => x.agent_id,
                        principalSchema: "dbo",
                        principalTable: "agents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "agent_wallets",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    agent_id = table.Column<Guid>("uuid", nullable: false),
                    currency_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    balance = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agent_wallets", x => x.id);
                    table.ForeignKey(
                        "fk_agent_wallet_agent_id",
                        x => x.agent_id,
                        principalSchema: "dbo",
                        principalTable: "agents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_agent_wallets_currencies_currency_code",
                        x => x.currency_code,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "brand_game_provider_schedules",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    brand_game_provider_id = table.Column<Guid>("uuid", nullable: false),
                    from = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    to = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    status = table.Column<short>("smallint", nullable: false),
                    day_of_week = table.Column<int>("integer", nullable: true),
                    is_default = table.Column<bool>("boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_game_provider_schedule", x => x.id);
                    table.ForeignKey(
                        "fk_brand_game_provider_schedule_brand_game_providers_id",
                        x => x.brand_game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "brand_game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "brand_group_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    bet_limit_id = table.Column<int>("integer", nullable: false),
                    brand_group_id = table.Column<Guid>("uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_group_bet_limits", x => x.id);
                    table.UniqueConstraint("ak_brand_group_bet_limits_brand_group_id_bet_limit_id",
                        x => new
                        {
                            x.brand_group_id,
                            x.bet_limit_id
                        });
                    table.ForeignKey(
                        "fk_brand_group_bet_limits_bet_limits_bet_limit_id",
                        x => x.bet_limit_id,
                        principalSchema: "dbo",
                        principalTable: "bet_limits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_brand_group_bet_limits_brand_groups_brand_group_id",
                        x => x.brand_group_id,
                        principalSchema: "dbo",
                        principalTable: "brand_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "brand_group_games",
                schema: "dbo",
                columns: table => new
                {
                    brand_group_id = table.Column<Guid>("uuid", nullable: false),
                    brand_game_id = table.Column<Guid>("uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_group_games", x => new
                    {
                        x.brand_group_id,
                        x.brand_game_id
                    });
                    table.ForeignKey(
                        "fk_brand_group_games_brand_games_brand_game_id",
                        x => x.brand_game_id,
                        principalSchema: "dbo",
                        principalTable: "brand_games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_brand_group_games_brand_groups_brand_group_id",
                        x => x.brand_group_id,
                        principalSchema: "dbo",
                        principalTable: "brand_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "users",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    status = table.Column<short>("smallint", nullable: false),
                    role_id = table.Column<Guid>("uuid", nullable: false),
                    department_id = table.Column<Guid>("uuid", nullable: false),
                    username = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                    last_login_ip = table.Column<string>("character varying(128)", maxLength: 128, nullable: true),
                    last_login_time = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true),
                    name = table.Column<string>("character varying(30)", maxLength: 30, nullable: false),
                    time_zone = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                    password = table.Column<byte[]>("bytea", nullable: false),
                    salt = table.Column<byte[]>("bytea", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        "fk_users_departments_department_id",
                        x => x.department_id,
                        principalSchema: "dbo",
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_users_roles_role_id",
                        x => x.role_id,
                        principalSchema: "dbo",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "brand_payment_vip_levels",
                schema: "dbo",
                columns: table => new
                {
                    brand_payment_id = table.Column<Guid>("uuid", nullable: false),
                    vip_level_id = table.Column<Guid>("uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_payment_vip_levels", x => new
                    {
                        x.brand_payment_id,
                        x.vip_level_id
                    });
                    table.ForeignKey(
                        "fk_brand_payment_vip_levels_brand_payments_brand_payment_id",
                        x => x.brand_payment_id,
                        principalSchema: "dbo",
                        principalTable: "brand_payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_brand_payment_vip_levels_vip_levels_vip_level_id",
                        x => x.vip_level_id,
                        principalSchema: "dbo",
                        principalTable: "vip_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "players",
                schema: "dbo",
                columns: table => new
                {
                    player_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    id_number = table.Column<string>("character varying(50)", maxLength: 50, nullable: true),
                    username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    real_name = table.Column<string>("character varying(32)", maxLength: 32, nullable: true),
                    invitation_code = table.Column<string>("character varying(6)", maxLength: 6, nullable: false),
                    password = table.Column<byte[]>("bytea", nullable: false),
                    salt = table.Column<byte[]>("bytea", nullable: false),
                    withdraw_password = table.Column<byte[]>("bytea", nullable: false),
                    withdraw_salt = table.Column<byte[]>("bytea", nullable: false),
                    default_wallet_id = table.Column<Guid>("uuid", nullable: false),
                    default_wallet_currency_code =
                        table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    registered_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    last_active_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    status = table.Column<short>("smallint", nullable: false),
                    bet_limit_id = table.Column<int>("integer", nullable: false),
                    type = table.Column<byte>("smallint", nullable: false),
                    email = table.Column<string>("character varying(64)", maxLength: 64, nullable: true),
                    phone = table.Column<string>("character varying(32)", maxLength: 32, nullable: true),
                    qq = table.Column<string>("character varying(64)", maxLength: 64, nullable: true),
                    wechat = table.Column<string>("character varying(64)", maxLength: 64, nullable: true),
                    has_agent = table.Column<bool>("boolean", nullable: false),
                    agent_id = table.Column<Guid>("uuid", nullable: true),
                    identity_authentication = table.Column<bool>("boolean", nullable: false),
                    phone_authentication = table.Column<bool>("boolean", nullable: false),
                    accumulated_deposit =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false,
                            defaultValue: 0m),
                    parent_player_code =
                        table.Column<string>("character varying(16)", maxLength: 16, nullable: true),
                    parent_invitation_code =
                        table.Column<string>("character varying(6)", maxLength: 6, nullable: true),
                    vip_id = table.Column<Guid>("uuid", nullable: true),
                    updated_by_username = table.Column<string>("text", nullable: true),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    first_deposit_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_code", x => x.player_code);
                    table.ForeignKey(
                        "fk_players_agents_agent_id",
                        x => x.agent_id,
                        principalSchema: "dbo",
                        principalTable: "agents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_players_bet_limits_bet_limit_id",
                        x => x.bet_limit_id,
                        principalSchema: "dbo",
                        principalTable: "bet_limits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_players_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_players_parent_player_code",
                        x => x.parent_player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_players_vip_levels_vip_id",
                        x => x.vip_id,
                        principalSchema: "dbo",
                        principalTable: "vip_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "brand_group_game_provider_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    brand_group_bet_limit_id = table.Column<Guid>("uuid", nullable: false),
                    game_provider_bet_limit_id = table.Column<Guid>("uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_group_game_provider_bet_limits", x => new
                    {
                        x.brand_group_bet_limit_id,
                        x.game_provider_bet_limit_id
                    });
                    table.ForeignKey(
                        "fk_brand_group_game_provider_bet_limits_brand_group_bet_limits_brand_group_bet_limit_id",
                        x => x.brand_group_bet_limit_id,
                        principalSchema: "dbo",
                        principalTable: "brand_group_bet_limits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_brand_group_game_provider_bet_limits_game_provider_bet_limits_game_provider_bet_limit_id",
                        x => x.game_provider_bet_limit_id,
                        principalSchema: "dbo",
                        principalTable: "game_provider_bet_limits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "brand_payment_players",
                schema: "dbo",
                columns: table => new
                {
                    player_code = table.Column<string>("character varying(16)", nullable: false),
                    brand_payment_id = table.Column<Guid>("uuid", nullable: false),
                    list_type = table.Column<short>("smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_payment_players", x => new
                    {
                        x.brand_payment_id,
                        x.player_code
                    });
                    table.ForeignKey(
                        "fk_brand_payment_players_brand_payments_brand_payment_id",
                        x => x.brand_payment_id,
                        principalSchema: "dbo",
                        principalTable: "brand_payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_brand_payment_players_players_player_code",
                        x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "player_banks",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    player_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    real_name = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    bank_name = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    bank_account = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>("character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_banks", x => x.id);
                    table.ForeignKey(
                        "fk_player_banks_players_player_code",
                        x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "player_login_records",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    player_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    status = table.Column<short>("smallint", nullable: false),
                    platform_type = table.Column<short>("smallint", nullable: false),
                    ip_address = table.Column<string>("text", nullable: true),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    expired_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_login_records", x => x.id);
                    table.ForeignKey(
                        "fk_player_login_records_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_player_login_records_players_player_code",
                        x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "player_sports_bet_limits",
                schema: "dbo",
                columns: table => new
                {
                    player_code = table.Column<string>("character varying(16)", nullable: false),
                    sport_market_type_id = table.Column<Guid>("uuid", nullable: false),
                    min_bet = table.Column<int>("integer", nullable: false),
                    max_bet = table.Column<int>("integer", nullable: false),
                    max_bet_per_match = table.Column<int>("integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_sports_bet_limits", x => new
                    {
                        x.player_code,
                        x.sport_market_type_id
                    });
                    table.ForeignKey(
                        "fk_player_sports_bet_limits_players_player_code",
                        x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_player_sports_bet_limits_sport_market_types_sport_market_type_id",
                        x => x.sport_market_type_id,
                        principalSchema: "dbo",
                        principalTable: "sport_market_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "station_letter_players",
                schema: "dbo",
                columns: table => new
                {
                    station_letter_id = table.Column<Guid>("uuid", nullable: false),
                    player_code = table.Column<string>("character varying(16)", nullable: false),
                    is_read = table.Column<bool>("boolean", nullable: false),
                    read_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_station_letter_players", x => new
                    {
                        x.station_letter_id,
                        x.player_code
                    });
                    table.ForeignKey(
                        "fk_station_letter_players_players_player_code",
                        x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_station_letter_players_station_letters_station_letter_id",
                        x => x.station_letter_id,
                        principalSchema: "dbo",
                        principalTable: "station_letters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "wallets",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    player_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    currency_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    external_currency_code =
                        table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    balance = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wallets", x => x.id);
                    table.ForeignKey(
                        "fk_wallets_currencies_currency_code",
                        x => x.currency_code,
                        principalSchema: "dbo",
                        principalTable: "currencies",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_wallets_players_player_code",
                        x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "transactions",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    wallet_id = table.Column<Guid>("uuid", nullable: false),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    external_transaction_id = table.Column<string>("text", nullable: true),
                    amount = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    balance = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    description = table.Column<string>("text", nullable: true),
                    type = table.Column<int>("integer", nullable: false),
                    recorded_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    sent_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    confirmed_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transactions", x => x.id);
                    table.ForeignKey(
                        "fk_transactions_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_transactions_wallets_wallet_id",
                        x => x.wallet_id,
                        principalSchema: "dbo",
                        principalTable: "wallets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "ix_agent_audit_logs_agent_id",
                schema: "dbo",
                table: "agent_audit_logs",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                "ix_agent_bankers_agent_id",
                schema: "dbo",
                table: "agent_banks",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                "ix_agent_wallets_agent_id",
                schema: "dbo",
                table: "agent_wallets",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                "ix_agent_wallets_currency_code",
                schema: "dbo",
                table: "agent_wallets",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                "ix_agents_brand_id",
                schema: "dbo",
                table: "agents",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                "ix_agents_parent_agent_id",
                schema: "dbo",
                table: "agents",
                column: "parent_agent_id");

            migrationBuilder.CreateIndex(
                "ix_announcements_brand_id",
                schema: "dbo",
                table: "announcements",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                "ix_bet_limits_currency_code",
                schema: "dbo",
                table: "bet_limits",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                "ix_brand_bet_limits_bet_limit_id",
                schema: "dbo",
                table: "brand_bet_limits",
                column: "bet_limit_id");

            migrationBuilder.CreateIndex(
                "ix_brand_currencies_currency_code",
                schema: "dbo",
                table: "brand_currencies",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                "ix_brand_game_provider_schedule_brand_game_provider_id",
                schema: "dbo",
                table: "brand_game_provider_schedules",
                column: "brand_game_provider_id");

            migrationBuilder.CreateIndex(
                "ix_brand_game_providers_default_language_code",
                schema: "dbo",
                table: "brand_game_providers",
                column: "default_language_code");

            migrationBuilder.CreateIndex(
                "ix_brand_game_providers_game_provider_configuration_id",
                schema: "dbo",
                table: "brand_game_providers",
                column: "game_provider_configuration_id");

            migrationBuilder.CreateIndex(
                "ix_brand_game_providers_game_provider_id",
                schema: "dbo",
                table: "brand_game_providers",
                column: "game_provider_id");

            migrationBuilder.CreateIndex(
                    "ux_brand_game_providers_brand_id_game_provider_id",
                    schema: "dbo",
                    table: "brand_game_providers",
                    columns: new[] {"brand_id", "game_provider_id"},
                    unique: true)
                .Annotation("Npgsql:IndexInclude",
                    new[]
                    {
                        "game_provider_configuration_id", "status", "default_language_code", "desktop_query_string",
                        "mobile_query_string"
                    });

            migrationBuilder.CreateIndex(
                "ix_brand_games_game_id",
                schema: "dbo",
                table: "brand_games",
                column: "game_id");

            migrationBuilder.CreateIndex(
                "ix_brand_group_bet_limits_bet_limit_id",
                schema: "dbo",
                table: "brand_group_bet_limits",
                column: "bet_limit_id");

            migrationBuilder.CreateIndex(
                "ix_brand_group_bet_limits_brand_group_id",
                schema: "dbo",
                table: "brand_group_bet_limits",
                column: "brand_group_id");

            migrationBuilder.CreateIndex(
                "ix_brand_group_game_provider_bet_limits_game_provider_bet_limit_id",
                schema: "dbo",
                table: "brand_group_game_provider_bet_limits",
                column: "game_provider_bet_limit_id");

            migrationBuilder.CreateIndex(
                "ix_brand_group_games_brand_game_id",
                schema: "dbo",
                table: "brand_group_games",
                column: "brand_game_id");

            migrationBuilder.CreateIndex(
                "ix_brand_groups_brand_id",
                schema: "dbo",
                table: "brand_groups",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                "ix_brand_languages_culture_code",
                schema: "dbo",
                table: "brand_languages",
                column: "culture_code");

            migrationBuilder.CreateIndex(
                "ix_brand_payment_audit_logs_brand_payment_id",
                schema: "dbo",
                table: "brand_payment_audit_logs",
                column: "brand_payment_id");

            migrationBuilder.CreateIndex(
                "ix_brand_payment_players_player_code",
                schema: "dbo",
                table: "brand_payment_players",
                column: "player_code");

            migrationBuilder.CreateIndex(
                "ix_brand_payment_vip_levels_vip_level_id",
                schema: "dbo",
                table: "brand_payment_vip_levels",
                column: "vip_level_id");

            migrationBuilder.CreateIndex(
                "ix_brand_payments_brand_id",
                schema: "dbo",
                table: "brand_payments",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                "ix_brand_payments_payment_channel_id",
                schema: "dbo",
                table: "brand_payments",
                column: "payment_channel_id");

            migrationBuilder.CreateIndex(
                "ix_brand_payments_payment_id",
                schema: "dbo",
                table: "brand_payments",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                "ix_brand_payments_payment_method_id",
                schema: "dbo",
                table: "brand_payments",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                "ix_brands_licensee_id",
                schema: "dbo",
                table: "brands",
                column: "licensee_id");

            migrationBuilder.CreateIndex(
                "ix_departments_brand_id",
                schema: "dbo",
                table: "departments",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                "ix_departments_licensee_id",
                schema: "dbo",
                table: "departments",
                column: "licensee_id");

            migrationBuilder.CreateIndex(
                "ix_departments_reseller_id",
                schema: "dbo",
                table: "departments",
                column: "reseller_id");

            migrationBuilder.CreateIndex(
                "ix_game_action_batches_game_provider_id",
                schema: "bet",
                table: "game_action_batches",
                column: "game_provider_id");

            migrationBuilder.CreateIndex(
                "ix_game_actions_game_action_batch_id",
                schema: "bet",
                table: "game_actions",
                column: "game_action_batch_id");

            migrationBuilder.CreateIndex(
                "ix_game_actions_referenced_game_action_id",
                schema: "bet",
                table: "game_actions",
                column: "referenced_game_action_id");

            migrationBuilder.CreateIndex(
                "ix_game_actions_round_id",
                schema: "bet",
                table: "game_actions",
                column: "round_id");

            migrationBuilder.CreateIndex(
                "ix_game_provider_bet_limits_currency_code",
                schema: "dbo",
                table: "game_provider_bet_limits",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                "ix_game_provider_bet_limits_game_provider_id",
                schema: "dbo",
                table: "game_provider_bet_limits",
                column: "game_provider_id");

            migrationBuilder.CreateIndex(
                "ix_game_provider_configurations_game_provider_id",
                schema: "dbo",
                table: "game_provider_configurations",
                column: "game_provider_id");

            migrationBuilder.CreateIndex(
                "ix_game_provider_currencies_currency_code",
                schema: "dbo",
                table: "game_provider_currencies",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                "ix_game_provider_languages_culture_code",
                schema: "dbo",
                table: "game_provider_languages",
                column: "culture_code");

            migrationBuilder.CreateIndex(
                    "ix_games_game_provider_id",
                    schema: "dbo",
                    table: "games",
                    column: "game_provider_id")
                .Annotation("Npgsql:IndexInclude",
                    new[]
                    {
                        "id", "code", "name", "external_id", "platform_type", "type", "status",
                        "available_currencies", "endpoint_path", "sub_game", "has_turnover_on_tie_bet", "rule_url",
                        "description"
                    });

            migrationBuilder.CreateIndex(
                "ix_licensee_bet_limits_bet_limit_id",
                schema: "dbo",
                table: "licensee_bet_limits",
                column: "bet_limit_id");

            migrationBuilder.CreateIndex(
                "ix_licensee_currencies_currency_code",
                schema: "dbo",
                table: "licensee_currencies",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                "ix_licensee_game_providers_game_provider_id",
                schema: "dbo",
                table: "licensee_game_providers",
                column: "game_provider_id");

            migrationBuilder.CreateIndex(
                "ix_licensee_languages_culture_code",
                schema: "dbo",
                table: "licensee_languages",
                column: "culture_code");

            migrationBuilder.CreateIndex(
                "ix_licensees_reseller_id",
                schema: "dbo",
                table: "licensees",
                column: "reseller_id");

            migrationBuilder.CreateIndex(
                "ix_localized_strings_culture_code",
                schema: "dbo",
                table: "localized_strings",
                column: "culture_code");

            migrationBuilder.CreateIndex(
                "ix_payment_channels_payment_id",
                schema: "dbo",
                table: "payment_channels",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                "ix_payment_channels_payment_method_id",
                schema: "dbo",
                table: "payment_channels",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                "ix_payment_methods_payment_id",
                schema: "dbo",
                table: "payment_methods",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                "ix_player_bankers_player_code",
                schema: "dbo",
                table: "player_banks",
                column: "player_code");

            migrationBuilder.CreateIndex(
                "ix_player_login_records_brand_id",
                schema: "dbo",
                table: "player_login_records",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                "ix_player_login_records_player_code",
                schema: "dbo",
                table: "player_login_records",
                column: "player_code");

            migrationBuilder.CreateIndex(
                "ix_player_sports_bet_limits_sport_market_id",
                schema: "dbo",
                table: "player_sports_bet_limits",
                column: "sport_market_type_id");

            migrationBuilder.CreateIndex(
                "ix_players_agent_id",
                schema: "dbo",
                table: "players",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                "ix_players_bet_limit_id",
                schema: "dbo",
                table: "players",
                column: "bet_limit_id");

            migrationBuilder.CreateIndex(
                "ix_players_brand_id",
                schema: "dbo",
                table: "players",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                "ix_players_parent_player_code",
                schema: "dbo",
                table: "players",
                column: "parent_player_code");

            migrationBuilder.CreateIndex(
                "ix_players_vip_id",
                schema: "dbo",
                table: "players",
                column: "vip_id");

            migrationBuilder.CreateIndex(
                "ix_reseller_bet_limits_bet_limit_id",
                schema: "dbo",
                table: "reseller_bet_limits",
                column: "bet_limit_id");

            migrationBuilder.CreateIndex(
                "ix_reseller_currencies_currency_code",
                schema: "dbo",
                table: "reseller_currencies",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                "ix_reseller_game_providers_game_provider_id",
                schema: "dbo",
                table: "reseller_game_providers",
                column: "game_provider_id");

            migrationBuilder.CreateIndex(
                "ix_reseller_languages_culture_code",
                schema: "dbo",
                table: "reseller_languages",
                column: "culture_code");

            migrationBuilder.CreateIndex(
                "ix_rounds_brand_id",
                schema: "bet",
                table: "rounds",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                    "ix_rounds_closed_on",
                    schema: "bet",
                    table: "rounds",
                    column: "closed_on")
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                "ix_rounds_game_id",
                schema: "bet",
                table: "rounds",
                column: "game_id");

            migrationBuilder.CreateIndex(
                "ix_rounds_game_provider_id",
                schema: "bet",
                table: "rounds",
                column: "game_provider_id");

            migrationBuilder.CreateIndex(
                "ix_rounds_player_code",
                schema: "bet",
                table: "rounds",
                column: "player_code");

            migrationBuilder.CreateIndex(
                    "ux_rounds_external_round_id_composite",
                    schema: "bet",
                    table: "rounds",
                    columns: new[] {"external_round_id", "player_code", "game_provider_id", "game_id"},
                    unique: true)
                .Annotation("Npgsql:IndexInclude", new[] {"game_provider_configuration_id", "wallet_id"});

            migrationBuilder.CreateIndex(
                "ix_sport_market_types_market_type_id",
                schema: "dbo",
                table: "sport_market_types",
                column: "market_type_id");

            migrationBuilder.CreateIndex(
                "ix_sport_types_game_provider_id",
                schema: "dbo",
                table: "sport_types",
                column: "game_provider_id");

            migrationBuilder.CreateIndex(
                "ix_station_letter_players_player_code",
                schema: "dbo",
                table: "station_letter_players",
                column: "player_code");

            migrationBuilder.CreateIndex(
                "ix_station_letters_brand_id",
                schema: "dbo",
                table: "station_letters",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                "ix_transactions_brand_id",
                schema: "dbo",
                table: "transactions",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                "ix_transactions_wallet_id",
                schema: "dbo",
                table: "transactions",
                column: "wallet_id");

            migrationBuilder.CreateIndex(
                "ix_users_department_id",
                schema: "dbo",
                table: "users",
                column: "department_id");

            migrationBuilder.CreateIndex(
                "ix_users_role_id",
                schema: "dbo",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                "ix_vip_levels_brand_id",
                schema: "dbo",
                table: "vip_levels",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                "ix_wallets_currency_code",
                schema: "dbo",
                table: "wallets",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                "ix_wallets_player_code",
                schema: "dbo",
                table: "wallets",
                column: "player_code");

            migrationBuilder.CreateIndex(
                "ix_whitelists_brand_id",
                schema: "dbo",
                table: "whitelists",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                "ix_whitelists_licensee_id",
                schema: "dbo",
                table: "whitelists",
                column: "licensee_id");

            migrationBuilder.CreateIndex(
                "ix_whitelists_reseller_id",
                schema: "dbo",
                table: "whitelists",
                column: "reseller_id");

            InitAfterRun(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "agent_audit_logs",
                "dbo");

            migrationBuilder.DropTable(
                "agent_banks",
                "dbo");

            migrationBuilder.DropTable(
                "agent_wallets",
                "dbo");

            migrationBuilder.DropTable(
                "announcements",
                "dbo");

            migrationBuilder.DropTable(
                "brand_bet_limits",
                "dbo");

            migrationBuilder.DropTable(
                "brand_currencies",
                "dbo");

            migrationBuilder.DropTable(
                "brand_game_provider_schedules",
                "dbo");

            migrationBuilder.DropTable(
                "brand_group_game_provider_bet_limits",
                "dbo");

            migrationBuilder.DropTable(
                "brand_group_games",
                "dbo");

            migrationBuilder.DropTable(
                "brand_languages",
                "dbo");

            migrationBuilder.DropTable(
                "brand_payment_audit_logs",
                "dbo");

            migrationBuilder.DropTable(
                "brand_payment_players",
                "dbo");

            migrationBuilder.DropTable(
                "brand_payment_vip_levels",
                "dbo");

            migrationBuilder.DropTable(
                "game_actions",
                "bet");

            migrationBuilder.DropTable(
                "game_provider_currencies",
                "dbo");

            migrationBuilder.DropTable(
                "game_provider_languages",
                "dbo");

            migrationBuilder.DropTable(
                "licensee_bet_limits",
                "dbo");

            migrationBuilder.DropTable(
                "licensee_currencies",
                "dbo");

            migrationBuilder.DropTable(
                "licensee_game_providers",
                "dbo");

            migrationBuilder.DropTable(
                "licensee_languages",
                "dbo");

            migrationBuilder.DropTable(
                "localized_strings",
                "dbo");

            migrationBuilder.DropTable(
                "player_audit_logs",
                "dbo");

            migrationBuilder.DropTable(
                "player_banks",
                "dbo");

            migrationBuilder.DropTable(
                "player_login_records",
                "dbo");

            migrationBuilder.DropTable(
                "player_sports_bet_limits",
                "dbo");

            migrationBuilder.DropTable(
                "reseller_bet_limits",
                "dbo");

            migrationBuilder.DropTable(
                "reseller_currencies",
                "dbo");

            migrationBuilder.DropTable(
                "reseller_game_providers",
                "dbo");

            migrationBuilder.DropTable(
                "reseller_languages",
                "dbo");

            migrationBuilder.DropTable(
                "round_details",
                "bet");

            migrationBuilder.DropTable(
                "settings",
                "dbo");

            migrationBuilder.DropTable(
                "station_letter_players",
                "dbo");

            migrationBuilder.DropTable(
                "transactions",
                "dbo");

            migrationBuilder.DropTable(
                "users",
                "dbo");

            migrationBuilder.DropTable(
                "whitelists",
                "dbo");

            migrationBuilder.DropTable(
                "brand_game_providers",
                "dbo");

            migrationBuilder.DropTable(
                "brand_group_bet_limits",
                "dbo");

            migrationBuilder.DropTable(
                "game_provider_bet_limits",
                "dbo");

            migrationBuilder.DropTable(
                "brand_games",
                "dbo");

            migrationBuilder.DropTable(
                "brand_payments",
                "dbo");

            migrationBuilder.DropTable(
                "game_action_batches",
                "bet");

            migrationBuilder.DropTable(
                "sport_market_types",
                "dbo");

            migrationBuilder.DropTable(
                "rounds",
                "bet");

            migrationBuilder.DropTable(
                "station_letters",
                "dbo");

            migrationBuilder.DropTable(
                "wallets",
                "dbo");

            migrationBuilder.DropTable(
                "departments",
                "dbo");

            migrationBuilder.DropTable(
                "roles",
                "dbo");

            migrationBuilder.DropTable(
                "game_provider_configurations",
                "dbo");

            migrationBuilder.DropTable(
                "languages",
                "dbo");

            migrationBuilder.DropTable(
                "brand_groups",
                "dbo");

            migrationBuilder.DropTable(
                "games",
                "dbo");

            migrationBuilder.DropTable(
                "payment_channels",
                "dbo");

            migrationBuilder.DropTable(
                "market_types",
                "dbo");

            migrationBuilder.DropTable(
                "sport_types",
                "dbo");

            migrationBuilder.DropTable(
                "players",
                "dbo");

            migrationBuilder.DropTable(
                "payment_methods",
                "dbo");

            migrationBuilder.DropTable(
                "game_providers",
                "dbo");

            migrationBuilder.DropTable(
                "agents",
                "dbo");

            migrationBuilder.DropTable(
                "bet_limits",
                "dbo");

            migrationBuilder.DropTable(
                "vip_levels",
                "dbo");

            migrationBuilder.DropTable(
                "payments",
                "dbo");

            migrationBuilder.DropTable(
                "currencies",
                "dbo");

            migrationBuilder.DropTable(
                "brands",
                "dbo");

            migrationBuilder.DropTable(
                "licensees",
                "dbo");

            migrationBuilder.DropTable(
                "resellers",
                "dbo");
        }
    }
}