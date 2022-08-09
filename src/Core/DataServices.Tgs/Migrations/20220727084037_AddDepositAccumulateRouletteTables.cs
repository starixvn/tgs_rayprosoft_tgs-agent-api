using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddDepositAccumulateRouletteTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "deposit_accumulate_roulette_events",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    player_code = table.Column<string>(type: "character varying(16)", nullable: false),
                    promotion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    roulette_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    total_deposit_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_deposit_accumulate_roulette_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_deposit_accumulate_roulette_events_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_deposit_accumulate_roulette_events_players_player_code",
                        column: x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_deposit_accumulate_roulette_events_promotions_promotion_id",
                        column: x => x.promotion_id,
                        principalSchema: "dbo",
                        principalTable: "promotions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "deposit_accumulate_roulette_game_events",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bonus = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false, defaultValue: 0m),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "Now()"),
                    deposit_accumulate_roulette_event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_index = table.Column<int>(type: "integer", nullable: false),
                    player_code = table.Column<string>(type: "character varying(16)", nullable: false),
                    promotion_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_deposit_accumulate_roulette_game_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_events_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_game_events_players_brand_id",
                        column: x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_game_events_promotions_promotion_id",
                        column: x => x.promotion_id,
                        principalSchema: "dbo",
                        principalTable: "promotions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_deposit_accumulate_roulette_events_brand_id",
                schema: "dbo",
                table: "deposit_accumulate_roulette_events",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_accumulate_roulette_events_player_code",
                schema: "dbo",
                table: "deposit_accumulate_roulette_events",
                column: "player_code");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_accumulate_roulette_events_promotion_id",
                schema: "dbo",
                table: "deposit_accumulate_roulette_events",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_accumulate_roulette_game_events_brand_id",
                schema: "dbo",
                table: "deposit_accumulate_roulette_game_events",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_accumulate_roulette_game_events_player_code",
                schema: "dbo",
                table: "deposit_accumulate_roulette_game_events",
                column: "player_code");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_accumulate_roulette_game_events_promotion_id",
                schema: "dbo",
                table: "deposit_accumulate_roulette_game_events",
                column: "promotion_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deposit_accumulate_roulette_events",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "deposit_accumulate_roulette_game_events",
                schema: "dbo");
        }
    }
}
