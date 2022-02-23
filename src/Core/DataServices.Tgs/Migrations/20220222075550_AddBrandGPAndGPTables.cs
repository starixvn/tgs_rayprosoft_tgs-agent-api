using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddBrandGPAndGPTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "brand_game_provider_aggregates",
                schema: "report",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency_code = table.Column<string>(type: "text", nullable: false),
                    transaction_count = table.Column<int>(type: "integer", nullable: false),
                    bet_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    valid_bet_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    turnover = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    closed_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_game_provider_aggregate_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "game_provider_aggregates",
                schema: "report",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency_code = table.Column<string>(type: "text", nullable: false),
                    transaction_count = table.Column<int>(type: "integer", nullable: false),
                    bet_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    valid_bet_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    turnover = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    external_closed_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_provider_aggregate_id", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_brand_game_provider_aggregate_multiple_keys",
                schema: "report",
                table: "brand_game_provider_aggregates",
                columns: new[] { "closed_on", "brand_id", "game_provider_id" });

            migrationBuilder.CreateIndex(
                name: "ix_game_provider_aggregate_multiple_keys",
                schema: "report",
                table: "game_provider_aggregates",
                columns: new[] { "external_closed_on", "game_provider_id", "brand_id" });

            var assembly = Assembly.GetExecutingAssembly();

            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "report_generate_brand_game_provider_aggregate_by_closed_on"));

            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "report_generate_game_provider_aggregate_by_closed_on"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "brand_game_provider_aggregates",
                schema: "report");

            migrationBuilder.DropTable(
                name: "game_provider_aggregates",
                schema: "report");
        }
    }
}
