using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddRecordedOnToAdjustPlayerRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "include_in_net_profit",
                schema: "dbo",
                table: "adjust_player_records",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "transaction_recorded_on",
                schema: "dbo",
                table: "adjust_player_records",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "include_in_net_profit",
                schema: "dbo",
                table: "adjust_player_records");

            migrationBuilder.DropColumn(
                name: "transaction_recorded_on",
                schema: "dbo",
                table: "adjust_player_records");
        }
    }
}
