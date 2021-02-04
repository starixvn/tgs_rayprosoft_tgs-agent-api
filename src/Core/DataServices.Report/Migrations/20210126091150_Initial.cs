using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TGS.Core.DataServices.Report.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "markets",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    create_on_date_string = table.Column<string>(type: "text", nullable: true),
                    registered_count = table.Column<int>(type: "integer", nullable: false),
                    first_fund_in_count = table.Column<int>(type: "integer", nullable: false),
                    deposit_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    withdrawal_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    adjustment_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    bonus_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    bet_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    valid_bet_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    win_loss_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    net_profit = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_markets", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "markets",
                schema: "dbo");
        }
    }
}
