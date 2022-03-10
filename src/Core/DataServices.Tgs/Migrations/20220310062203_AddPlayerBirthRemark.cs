using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddPlayerBirthRemark : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "birthday",
                schema: "dbo",
                table: "players",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "remark",
                schema: "dbo",
                table: "players",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "game_type",
                schema: "report",
                table: "brand_game_provider_aggregates",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "birthday",
                schema: "dbo",
                table: "players");

            migrationBuilder.DropColumn(
                name: "remark",
                schema: "dbo",
                table: "players");

            migrationBuilder.AlterColumn<int>(
                name: "game_type",
                schema: "report",
                table: "brand_game_provider_aggregates",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");
        }
    }
}
