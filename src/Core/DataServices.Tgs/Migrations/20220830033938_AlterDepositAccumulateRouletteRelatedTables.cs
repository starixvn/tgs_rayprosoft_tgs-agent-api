using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AlterDepositAccumulateRouletteRelatedTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_deposit_accumulate_roulette_events",
                schema: "dbo",
                table: "deposit_accumulate_roulette_events");

            migrationBuilder.DropIndex(
                name: "ix_deposit_accumulate_roulette_events_player_code",
                schema: "dbo",
                table: "deposit_accumulate_roulette_events");

            migrationBuilder.DropColumn(
                name: "deposit_accumulate_roulette_event_id",
                schema: "dbo",
                table: "deposit_accumulate_roulette_game_events");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "dbo",
                table: "deposit_accumulate_roulette_events");

            migrationBuilder.AddColumn<DateOnly>(
                name: "date",
                schema: "dbo",
                table: "deposit_accumulate_roulette_game_events",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddPrimaryKey(
                name: "pk_deposit_accumulate_roulette_events",
                schema: "dbo",
                table: "deposit_accumulate_roulette_events",
                columns: new[] { "player_code", "promotion_id", "date" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_deposit_accumulate_roulette_events",
                schema: "dbo",
                table: "deposit_accumulate_roulette_events");

            migrationBuilder.DropColumn(
                name: "date",
                schema: "dbo",
                table: "deposit_accumulate_roulette_game_events");

            migrationBuilder.AddColumn<Guid>(
                name: "deposit_accumulate_roulette_event_id",
                schema: "dbo",
                table: "deposit_accumulate_roulette_game_events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                schema: "dbo",
                table: "deposit_accumulate_roulette_events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "pk_deposit_accumulate_roulette_events",
                schema: "dbo",
                table: "deposit_accumulate_roulette_events",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_deposit_accumulate_roulette_events_player_code",
                schema: "dbo",
                table: "deposit_accumulate_roulette_events",
                column: "player_code");
        }
    }
}
