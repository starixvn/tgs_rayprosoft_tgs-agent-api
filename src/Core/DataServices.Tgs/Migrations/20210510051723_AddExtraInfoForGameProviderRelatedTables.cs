using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddExtraInfoForGameProviderRelatedTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "reopen_turnover_adjustment",
                schema: "bet",
                table: "rounds",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "reopened_on",
                schema: "bet",
                table: "rounds",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "transfer_wallet",
                schema: "dbo",
                table: "game_providers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "player_code",
                schema: "bet",
                table: "game_actions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reopen_turnover_adjustment",
                schema: "bet",
                table: "rounds");

            migrationBuilder.DropColumn(
                name: "reopened_on",
                schema: "bet",
                table: "rounds");

            migrationBuilder.DropColumn(
                name: "transfer_wallet",
                schema: "dbo",
                table: "game_providers");

            migrationBuilder.DropColumn(
                name: "player_code",
                schema: "bet",
                table: "game_actions");
        }
    }
}
