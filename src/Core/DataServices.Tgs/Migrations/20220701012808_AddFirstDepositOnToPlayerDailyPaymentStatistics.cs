using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddFirstDepositOnToPlayerDailyPaymentStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "first_deposit_on",
                schema: "dbo",
                table: "player_daily_payment_statistics",
                type: "timestamptz",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "first_deposit_on",
                schema: "dbo",
                table: "player_daily_payment_statistics");
        }
    }
}
