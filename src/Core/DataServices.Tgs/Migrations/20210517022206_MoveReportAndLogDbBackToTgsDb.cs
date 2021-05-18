using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class MoveReportAndLogDbBackToTgsDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "log");

            migrationBuilder.EnsureSchema(
                name: "report");

            migrationBuilder.CreateTable(
                name: "admin_logs",
                schema: "log",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tracking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    path = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    method = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    exception = table.Column<string>(type: "text", nullable: true),
                    stack_trace = table.Column<string>(type: "text", nullable: true),
                    server_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    request_headers = table.Column<string>(type: "text", nullable: false),
                    request_content = table.Column<string>(type: "text", nullable: false),
                    response_headers = table.Column<string>(type: "text", nullable: false),
                    response_content = table.Column<string>(type: "text", nullable: false),
                    response_code = table.Column<short>(type: "smallint", nullable: false),
                    execution_time_ms = table.Column<int>(type: "integer", nullable: false),
                    recorded_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_admin_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "api_logs",
                schema: "log",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tracking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    path = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    method = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    prefix = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    server_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    request_headers = table.Column<string>(type: "text", nullable: false),
                    request_content = table.Column<string>(type: "text", nullable: false),
                    response_headers = table.Column<string>(type: "text", nullable: false),
                    response_content = table.Column<string>(type: "text", nullable: false),
                    response_code = table.Column<short>(type: "smallint", nullable: false),
                    execution_time_ms = table.Column<int>(type: "integer", nullable: false),
                    recorded_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_api_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "log_records",
                schema: "log",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tracking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recorded_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    server_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    logger = table.Column<string>(type: "text", nullable: true),
                    message = table.Column<string>(type: "text", nullable: false),
                    exception = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_log_records", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "player_aggregates",
                schema: "report",
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
                schema: "report",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    vip_level = table.Column<int>(type: "integer", nullable: true),
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
                name: "round_aggregates",
                schema: "report",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    player_code = table.Column<string>(type: "text", nullable: false),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "ix_player_aggregates_brand_id_report_date",
                schema: "report",
                table: "player_aggregates",
                columns: new[] { "brand_id", "report_date" });

            migrationBuilder.CreateIndex(
                name: "ix_player_aggregates_brand_id_report_date_agent_id",
                schema: "report",
                table: "player_aggregates",
                columns: new[] { "brand_id", "report_date", "agent_id" });

            migrationBuilder.CreateIndex(
                name: "ix_player_aggregates_brand_id_report_date_player_code",
                schema: "report",
                table: "player_aggregates",
                columns: new[] { "brand_id", "report_date", "player_code" });

            migrationBuilder.CreateIndex(
                name: "ix_player_game_type_rebate_aggregates_brand_id_report_date",
                schema: "report",
                table: "player_game_type_rebate_aggregates",
                columns: new[] { "brand_id", "report_date" });

            migrationBuilder.CreateIndex(
                name: "ix_player_game_type_rebate_aggregates_brand_id_report_date_agent_id",
                schema: "report",
                table: "player_game_type_rebate_aggregates",
                columns: new[] { "brand_id", "report_date", "agent_id" });

            migrationBuilder.CreateIndex(
                name: "ix_player_game_type_rebate_aggregates_brand_id_report_date_player_code",
                schema: "report",
                table: "player_game_type_rebate_aggregates",
                columns: new[] { "brand_id", "report_date", "player_code" });

            migrationBuilder.CreateIndex(
                name: "ix_round_aggregate_multiple_keys",
                schema: "report",
                table: "round_aggregates",
                columns: new[] { "report_date", "hour", "minute" });

            migrationBuilder.CreateIndex(
                name: "ix_round_aggregate_player_code",
                schema: "report",
                table: "round_aggregates",
                column: "player_code");

            var assembly = Assembly.GetExecutingAssembly();

            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "log_get_admin_log"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "log_get_admin_logs"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "log_get_api_log"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "log_get_api_logs"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "log_get_log_records"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.IndexSqlFormat, "admin_logs"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.IndexSqlFormat, "api_logs"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.IndexSqlFormat, "log_records"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_logs",
                schema: "log");

            migrationBuilder.DropTable(
                name: "api_logs",
                schema: "log");

            migrationBuilder.DropTable(
                name: "log_records",
                schema: "log");

            migrationBuilder.DropTable(
                name: "player_aggregates",
                schema: "report");

            migrationBuilder.DropTable(
                name: "player_game_type_rebate_aggregates",
                schema: "report");

            migrationBuilder.DropTable(
                name: "round_aggregates",
                schema: "report");
        }
    }
}
