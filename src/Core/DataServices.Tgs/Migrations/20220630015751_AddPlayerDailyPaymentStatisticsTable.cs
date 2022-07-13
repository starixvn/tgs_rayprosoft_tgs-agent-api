using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddPlayerDailyPaymentStatisticsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "player_daily_payment_statistics",
                schema: "dbo",
                columns: table => new
                {
                    player_code = table.Column<string>(type: "text", nullable: false),
                    report_date = table.Column<DateOnly>(type: "date", nullable: false),
                    daily_deposit_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    daily_deposit_count = table.Column<int>(type: "integer", nullable: true),
                    daily_withdraw_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    daily_withdraw_count = table.Column<int>(type: "integer", nullable: true),
                    total_deposit_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    total_deposit_count = table.Column<int>(type: "integer", nullable: true),
                    total_withdraw_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    total_withdraw_count = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_daily_payment_statistics", x => new { x.report_date, x.player_code });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_daily_payment_statistics",
                schema: "dbo");
        }
    }
}
