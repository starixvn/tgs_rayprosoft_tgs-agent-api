using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddMarketAggregateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "market_aggregates",
                schema: "report",
                columns: table => new
                {
                    id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    report_date = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    registered_count = table.Column<int>("integer", nullable: false),
                    first_fund_in_count = table.Column<int>("integer", nullable: false),
                    deposit_amount = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    withdrawal_amount =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    adjustment_amount =
                        table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    bonus_amount = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    bet_amount = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    turnover = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    win_loss_amount = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    net_profit = table.Column<decimal>("numeric(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_market_aggregates_id", x => x.id);
                });

            migrationBuilder.CreateIndex(
                "ix_market_aggregates_brand_id_report_date",
                schema: "report",
                table: "market_aggregates",
                columns: new[] {"brand_id", "report_date"});
        }

        protected override void Down(MigrationBuilder migrationBuilder) =>
            migrationBuilder.DropTable(
                "market_aggregates",
                "report");
    }
}