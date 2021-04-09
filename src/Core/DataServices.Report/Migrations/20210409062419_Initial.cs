using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TGS.Core.DataServices.Report.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "agent_dividends",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deposit_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    withdrawal_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    adjustment_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    bonus_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    bet_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    turnover = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    win_loss_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    net_profit = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    live_dealer_win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    slot_win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    fishing_win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    arcade_win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    table_game_win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    lottery_win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    card_game_win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    sports_win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    e_sports_win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    virtual_sports_win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    agent_dividend_percentage = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    agent_dividend_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agent_dividends_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "player_aggregates",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    hour = table.Column<int>(type: "integer", nullable: false),
                    minute = table.Column<int>(type: "integer", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    report_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    registered_count = table.Column<int>(type: "integer", nullable: false),
                    first_fund_in_count = table.Column<int>(type: "integer", nullable: false),
                    deposit_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    withdrawal_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    adjustment_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    bonus_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    bet_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    turnover = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    win_loss_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    net_profit = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_aggregates_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "player_game_type_rebate_aggregates",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    game_type = table.Column<short>(type: "smallint", nullable: false),
                    hour = table.Column<int>(type: "integer", nullable: false),
                    minute = table.Column<int>(type: "integer", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    report_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    turnover = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    vip_live_dealer_rebate_percentage = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    vip_slot_rebate_percentage = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    vip_card_game_rebate_percentage = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    vip_sports_rebate_percentage = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    vip_virtual_sports_rebate_percentage = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    vip_lottery_rebate_percentage = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    vip_rebate_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    agent_dividend_percentage = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_game_type_rebate_aggregates_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "player_rebates",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    report_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    sports_turnover = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    sports_rebate_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    virtual_sports_turnover = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    virtual_sports_rebate_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    live_dealer_turnover = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    live_dealer_rebate_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    card_game_turnover = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    card_game_rebate_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    slot_turnover = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    slot_rebate_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    lottery_turnover = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    lottery_rebate_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    vip_live_dealer_rebate_percentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    vip_slot_rebate_percentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    vip_card_game_rebate_percentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    vip_sports_rebate_percentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    vip_virtual_sports_rebate_percentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    vip_lottery_rebate_percentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    rebate_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_rebates_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "round_aggregate",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    player_code = table.Column<string>(type: "text", nullable: false),
                    game_type = table.Column<short>(type: "smallint", nullable: false),
                    hour = table.Column<int>(type: "integer", nullable: false),
                    minute = table.Column<int>(type: "integer", nullable: false),
                    bet_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    valid_bet_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    turnover = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    report_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_round_aggregate_id", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_agent_dividends_brand_id",
                schema: "dbo",
                table: "agent_dividends",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_agent_dividends_brand_id_created_on",
                schema: "dbo",
                table: "agent_dividends",
                columns: new[] { "brand_id", "created_on" });

            migrationBuilder.CreateIndex(
                name: "ix_agent_dividends_brand_id_created_on_agent_id",
                schema: "dbo",
                table: "agent_dividends",
                columns: new[] { "brand_id", "created_on", "agent_id" });

            migrationBuilder.CreateIndex(
                name: "ix_player_aggregates_brand_id_report_date",
                schema: "dbo",
                table: "player_aggregates",
                columns: new[] { "brand_id", "report_date" });

            migrationBuilder.CreateIndex(
                name: "ix_player_aggregates_brand_id_report_date_agent_id",
                schema: "dbo",
                table: "player_aggregates",
                columns: new[] { "brand_id", "report_date", "agent_id" });

            migrationBuilder.CreateIndex(
                name: "ix_player_aggregates_brand_id_report_date_player_code",
                schema: "dbo",
                table: "player_aggregates",
                columns: new[] { "brand_id", "report_date", "player_code" });

            migrationBuilder.CreateIndex(
                name: "ix_player_game_type_rebate_aggregates_brand_id_report_date",
                schema: "dbo",
                table: "player_game_type_rebate_aggregates",
                columns: new[] { "brand_id", "report_date" });

            migrationBuilder.CreateIndex(
                name: "ix_player_game_type_rebate_aggregates_brand_id_report_date_agent_id",
                schema: "dbo",
                table: "player_game_type_rebate_aggregates",
                columns: new[] { "brand_id", "report_date", "agent_id" });

            migrationBuilder.CreateIndex(
                name: "ix_player_game_type_rebate_aggregates_brand_id_report_date_player_code",
                schema: "dbo",
                table: "player_game_type_rebate_aggregates",
                columns: new[] { "brand_id", "report_date", "player_code" });

            migrationBuilder.CreateIndex(
                name: "ix_player_rebates_brand_id_report_date",
                schema: "dbo",
                table: "player_rebates",
                columns: new[] { "brand_id", "report_date" });

            migrationBuilder.CreateIndex(
                name: "ix_player_rebates_brand_id_report_date_agent_id",
                schema: "dbo",
                table: "player_rebates",
                columns: new[] { "brand_id", "report_date", "agent_id" });

            migrationBuilder.CreateIndex(
                name: "ix_player_rebates_brand_id_report_date_player_code",
                schema: "dbo",
                table: "player_rebates",
                columns: new[] { "brand_id", "report_date", "player_code" });

            migrationBuilder.CreateIndex(
                name: "ix_round_aggregate_multiple_keys",
                schema: "dbo",
                table: "round_aggregate",
                columns: new[] { "report_date", "hour", "minute" });

            migrationBuilder.CreateIndex(
                name: "ix_round_aggregate_player_code",
                schema: "dbo",
                table: "round_aggregate",
                column: "player_code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agent_dividends",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "player_aggregates",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "player_game_type_rebate_aggregates",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "player_rebates",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "round_aggregate",
                schema: "dbo");
        }
    }
}
