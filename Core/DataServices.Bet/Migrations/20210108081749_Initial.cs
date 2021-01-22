using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Bet.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                "dbo");

            migrationBuilder.CreateTable(
                "game_action_batches",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    external_id = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                    error_type = table.Column<short>("smallint", nullable: false),
                    error_description =
                        table.Column<string>("character varying(1024)", maxLength: 1024, nullable: true),
                    transactions_count = table.Column<int>("integer", nullable: false),
                    errors_json = table.Column<string>("character varying(4000)", maxLength: 4000, nullable: true),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    game_provider_configuration_id = table.Column<Guid>("uuid", nullable: false)
                },
                constraints: table => { table.PrimaryKey("pk_game_action_batches", x => x.id); });

            migrationBuilder.CreateTable(
                "rounds",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    external_round_id = table.Column<string>("character varying(255)", maxLength: 255, nullable: false),
                    wallet_id = table.Column<Guid>("uuid", nullable: false),
                    game_id = table.Column<int>("integer", nullable: false),
                    game_name = table.Column<string>("character varying(255)", maxLength: 255, nullable: false),
                    game_type = table.Column<short>("smallint", nullable: false),
                    game_provider_id = table.Column<Guid>("uuid", nullable: false),
                    game_provider_name =
                        table.Column<string>("character varying(255)", maxLength: 255, nullable: false),
                    game_provider_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    player_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    player_type = table.Column<byte>("smallint", nullable: false),
                    currency_code = table.Column<string>("character varying(10)", maxLength: 10, nullable: false),
                    platform_type = table.Column<short>("smallint", nullable: false),
                    status = table.Column<short>("smallint", nullable: false),
                    game_provider_configuration_id = table.Column<Guid>("uuid", nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    closed_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true),
                    updated_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    reseller_id = table.Column<Guid>("uuid", nullable: false),
                    reseller_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    licensee_id = table.Column<Guid>("uuid", nullable: false),
                    licensee_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    brand_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    external_created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true),
                    external_closed_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true),
                    bet_amount = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    commission = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    win_loss = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    tie_bet_adjustment = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: true)
                },
                constraints: table => { table.PrimaryKey("pk_rounds", x => x.id); });

            migrationBuilder.CreateTable(
                "game_actions",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    game_provider_configuration_id = table.Column<Guid>("uuid", nullable: false),
                    wallet_id = table.Column<Guid>("uuid", nullable: false),
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
                    external_bet_id = table.Column<string>("text", nullable: false),
                    external_game_action_group_id =
                        table.Column<string>("character varying(128)", maxLength: 128, nullable: false),
                    referenced_game_action_id = table.Column<Guid>("uuid", nullable: true),
                    game_action_batch_id = table.Column<Guid>("uuid", nullable: true),
                    recorded_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    sent_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true),
                    is_closing_round = table.Column<bool>("boolean", nullable: false),
                    closing_round_explicit_request = table.Column<bool>("boolean", nullable: true),
                    player_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
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
                        principalSchema: "dbo",
                        principalTable: "game_action_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_game_actions_referenced_game_action_id",
                        x => x.referenced_game_action_id,
                        principalSchema: "dbo",
                        principalTable: "game_actions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_game_actions_rounds_round_id",
                        x => x.round_id,
                        principalSchema: "dbo",
                        principalTable: "rounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "round_details",
                schema: "dbo",
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
                        principalSchema: "dbo",
                        principalTable: "rounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "ix_game_actions_game_action_batch_id",
                schema: "dbo",
                table: "game_actions",
                column: "game_action_batch_id");

            migrationBuilder.CreateIndex(
                "ix_game_actions_referenced_game_action_id",
                schema: "dbo",
                table: "game_actions",
                column: "referenced_game_action_id");

            migrationBuilder.CreateIndex(
                "ix_game_actions_round_id",
                schema: "dbo",
                table: "game_actions",
                column: "round_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "game_actions",
                "dbo");

            migrationBuilder.DropTable(
                "round_details",
                "dbo");

            migrationBuilder.DropTable(
                "game_action_batches",
                "dbo");

            migrationBuilder.DropTable(
                "rounds",
                "dbo");
        }
    }
}