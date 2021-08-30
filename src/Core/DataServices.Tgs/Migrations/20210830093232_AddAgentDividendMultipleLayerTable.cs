using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddAgentDividendMultipleLayerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "agent_dividend_percentage",
                schema: "report",
                table: "player_game_rebate_aggregates");

            migrationBuilder.DropColumn(
                name: "agent_dividend_percentage",
                schema: "report",
                table: "player_aggregates");

            migrationBuilder.AddColumn<decimal>(
                name: "promotion_amount",
                schema: "dbo",
                table: "agent_dividends",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "rebate_amount",
                schema: "dbo",
                table: "agent_dividends",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "agent_dividend_multiple_layers",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    agent_dividend_id = table.Column<int>(type: "integer", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dividend_provider_agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    layer = table.Column<string>(type: "text", nullable: false),
                    dividend = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agent_dividend_multiple_layers_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_agents_dividends_agent_dividend_multiple_layers_agent_dividend_id",
                        column: x => x.agent_dividend_id,
                        principalSchema: "dbo",
                        principalTable: "agent_dividends",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_agent_dividend_multiple_layers_agent_dividend_id",
                schema: "dbo",
                table: "agent_dividend_multiple_layers",
                column: "agent_dividend_id");

            migrationBuilder.CreateIndex(
                name: "ix_agent_dividend_multiple_layers_agent_id_dividend_provider_agent_id",
                schema: "dbo",
                table: "agent_dividend_multiple_layers",
                columns: new[] { "agent_id", "dividend_provider_agent_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agent_dividend_multiple_layers",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "promotion_amount",
                schema: "dbo",
                table: "agent_dividends");

            migrationBuilder.DropColumn(
                name: "rebate_amount",
                schema: "dbo",
                table: "agent_dividends");

            migrationBuilder.AddColumn<decimal>(
                name: "agent_dividend_percentage",
                schema: "report",
                table: "player_game_rebate_aggregates",
                type: "numeric(9,6)",
                precision: 9,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "agent_dividend_percentage",
                schema: "report",
                table: "player_aggregates",
                type: "numeric(9,6)",
                precision: 9,
                scale: 6,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
