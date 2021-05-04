using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class add_player_dividned_player_rebate_promotion_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "fk_player_buckets_players_player_code",
                schema: "dbo",
                table: "player_buckets");

            migrationBuilder.DropIndex(
                "ix_player_buckets_player_code",
                schema: "dbo",
                table: "player_buckets");

            migrationBuilder.AlterColumn<string>(
                "player_code",
                schema: "dbo",
                table: "player_buckets",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                "is_full",
                schema: "dbo",
                table: "player_buckets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                "player_code1",
                schema: "dbo",
                table: "player_buckets",
                type: "character varying(16)",
                nullable: true);

            migrationBuilder.CreateTable(
                "agent_dividends",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    agent_id = table.Column<Guid>("uuid", nullable: false),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    from = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    to = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    deposit_amount = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    withdrawal_amount =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    adjustment_amount =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    bonus_amount = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    bet_amount = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    turnover = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    win_loss_amount = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    net_profit = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    live_dealer_win_loss =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    slot_win_loss = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    fishing_win_loss = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    arcade_win_loss = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    table_game_win_loss =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    lottery_win_loss = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    card_game_win_loss =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    sports_win_loss = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    e_sports_win_loss =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    virtual_sports_win_loss =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    agent_dividend_percentage =
                        table.Column<decimal>("numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    agent_dividend_amount = table.Column<decimal>("numeric", nullable: false),
                    sent_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agent_dividends_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                "player_rebates",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    agent_id = table.Column<Guid>("uuid", nullable: true),
                    player_code = table.Column<string>("character varying(16)", maxLength: 16, nullable: false),
                    sent_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    report_date = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    sports_turnover = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    sports_rebate_amount =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    virtual_sports_turnover =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    virtual_sports_rebate_amount =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    live_dealer_turnover =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    live_dealer_rebate_amount =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    card_game_turnover =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    card_game_rebate_amount =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    slot_turnover = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    slot_rebate_amount =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    lottery_turnover = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    lottery_rebate_amount =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    vip_live_dealer_rebate_percentage =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    vip_slot_rebate_percentage =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    vip_card_game_rebate_percentage =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    vip_sports_rebate_percentage =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    vip_virtual_sports_rebate_percentage =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    vip_lottery_rebate_percentage =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    rebate_amount = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_rebates_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                "promotion_bonuses",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    promotion_id = table.Column<Guid>("uuid", nullable: false),
                    from = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    to = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    promotion_type = table.Column<string>("text", nullable: false),
                    total_amount_limit =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    game_type = table.Column<short>("smallint", nullable: true),
                    percentage = table.Column<decimal>("numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    bonus_amount_limit =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    turnover_multiple = table.Column<decimal>("numeric", nullable: false),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_promotion_bonus_id", x => x.id);
                    table.ForeignKey(
                        "fk_promotion_bonus_promotion_id",
                        x => x.promotion_id,
                        principalSchema: "dbo",
                        principalTable: "promotions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "promotion_players",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_code = table.Column<string>("character varying(16)", nullable: false),
                    promotion_bonus_id = table.Column<Guid>("uuid", nullable: false),
                    bonus_amount = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    sent_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_promotion_player_id", x => x.id);
                    table.ForeignKey(
                        "fk_promotion_bonus_player_code",
                        x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "fk_promotion_bonus_promotion_brand_id",
                        x => x.promotion_bonus_id,
                        principalSchema: "dbo",
                        principalTable: "promotion_bonuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "ix_player_buckets_player_code1",
                schema: "dbo",
                table: "player_buckets",
                column: "player_code1");

            migrationBuilder.CreateIndex(
                "ix_agent_dividends_brand_id",
                schema: "dbo",
                table: "agent_dividends",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                "ix_agent_dividends_brand_id_created_on",
                schema: "dbo",
                table: "agent_dividends",
                columns: new[] {"brand_id", "created_on"});

            migrationBuilder.CreateIndex(
                "ix_agent_dividends_brand_id_created_on_agent_id",
                schema: "dbo",
                table: "agent_dividends",
                columns: new[] {"brand_id", "created_on", "agent_id"});

            migrationBuilder.CreateIndex(
                "ix_player_rebates_brand_id_report_date",
                schema: "dbo",
                table: "player_rebates",
                columns: new[] {"brand_id", "report_date"});

            migrationBuilder.CreateIndex(
                "ix_player_rebates_brand_id_report_date_agent_id",
                schema: "dbo",
                table: "player_rebates",
                columns: new[] {"brand_id", "report_date", "agent_id"});

            migrationBuilder.CreateIndex(
                "ix_player_rebates_brand_id_report_date_player_code",
                schema: "dbo",
                table: "player_rebates",
                columns: new[] {"brand_id", "report_date", "player_code"});

            migrationBuilder.CreateIndex(
                "ix_promotion_bonus_brand_id",
                schema: "dbo",
                table: "promotion_bonuses",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                "ix_promotion_bonus_brand_id_from_to",
                schema: "dbo",
                table: "promotion_bonuses",
                columns: new[] {"brand_id", "from", "to"});

            migrationBuilder.CreateIndex(
                "ix_promotion_bonus_promotion_id",
                schema: "dbo",
                table: "promotion_bonuses",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                "ix_promotion_bonus_promotion_id_from_to",
                schema: "dbo",
                table: "promotion_bonuses",
                columns: new[] {"promotion_id", "from", "to"});

            migrationBuilder.CreateIndex(
                "ix_promotion_player_multiple_keys",
                schema: "dbo",
                table: "promotion_players",
                columns: new[] {"player_code", "promotion_bonus_id"});

            migrationBuilder.CreateIndex(
                "ix_promotion_player_promotion_id",
                schema: "dbo",
                table: "promotion_players",
                column: "player_code");

            migrationBuilder.CreateIndex(
                "ix_promotion_players_promotion_bonus_id",
                schema: "dbo",
                table: "promotion_players",
                column: "promotion_bonus_id");

            migrationBuilder.AddForeignKey(
                "fk_player_buckets_players_player_code1",
                schema: "dbo",
                table: "player_buckets",
                column: "player_code1",
                principalSchema: "dbo",
                principalTable: "players",
                principalColumn: "player_code",
                onDelete: ReferentialAction.Restrict);

            var assembly = Assembly.GetExecutingAssembly();

            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "agent_wallet_update_wallet_balance"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "fk_player_buckets_players_player_code1",
                schema: "dbo",
                table: "player_buckets");

            migrationBuilder.DropTable(
                "agent_dividends",
                "dbo");

            migrationBuilder.DropTable(
                "player_rebates",
                "dbo");

            migrationBuilder.DropTable(
                "promotion_players",
                "dbo");

            migrationBuilder.DropTable(
                "promotion_bonuses",
                "dbo");

            migrationBuilder.DropIndex(
                "ix_player_buckets_player_code1",
                schema: "dbo",
                table: "player_buckets");

            migrationBuilder.DropColumn(
                "is_full",
                schema: "dbo",
                table: "player_buckets");

            migrationBuilder.DropColumn(
                "player_code1",
                schema: "dbo",
                table: "player_buckets");

            migrationBuilder.AlterColumn<string>(
                "player_code",
                schema: "dbo",
                table: "player_buckets",
                type: "character varying(16)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                "ix_player_buckets_player_code",
                schema: "dbo",
                table: "player_buckets",
                column: "player_code");

            migrationBuilder.AddForeignKey(
                "fk_player_buckets_players_player_code",
                schema: "dbo",
                table: "player_buckets",
                column: "player_code",
                principalSchema: "dbo",
                principalTable: "players",
                principalColumn: "player_code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}