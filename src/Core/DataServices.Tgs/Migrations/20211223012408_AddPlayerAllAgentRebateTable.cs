using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class AddPlayerAllAgentRebateTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "dividend_percentage",
            schema: "dbo",
            table: "brand_agent_dividend_conditions",
            type: "numeric(9,6)",
            precision: 9,
            scale: 6,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric(18,6)",
            oldPrecision: 18,
            oldScale: 6);

        migrationBuilder.AlterColumn<decimal>(
            name: "dividend_percentage",
            schema: "dbo",
            table: "agent_dividend_conditions",
            type: "numeric(9,6)",
            precision: 9,
            scale: 6,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric(18,6)",
            oldPrecision: 18,
            oldScale: 6);

        migrationBuilder.CreateTable(
            name: "player_all_agent_rebates",
            schema: "dbo",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                report_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                sent_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                all_agent_game_type = table.Column<short>(type: "smallint", nullable: false),
                game_type = table.Column<short>(type: "smallint", nullable: false),
                all_agent_rebate_type = table.Column<short>(type: "smallint", nullable: false),
                rank = table.Column<int>(type: "integer", nullable: false),
                active_number = table.Column<int>(type: "integer", nullable: false),
                condition_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                rebate_percentage = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                actual_player_number = table.Column<int>(type: "integer", nullable: false),
                actual_condition_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                rebate_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_player_all_agent_rebates_id", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_player_all_agent_rebates_brand_id_report_date_player_code",
            schema: "dbo",
            table: "player_all_agent_rebates",
            columns: new[] { "brand_id", "report_date", "player_code" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "player_all_agent_rebates",
            schema: "dbo");

        migrationBuilder.AlterColumn<decimal>(
            name: "dividend_percentage",
            schema: "dbo",
            table: "brand_agent_dividend_conditions",
            type: "numeric(18,6)",
            precision: 18,
            scale: 6,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric(9,6)",
            oldPrecision: 9,
            oldScale: 6);

        migrationBuilder.AlterColumn<decimal>(
            name: "dividend_percentage",
            schema: "dbo",
            table: "agent_dividend_conditions",
            type: "numeric(18,6)",
            precision: 18,
            scale: 6,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric(9,6)",
            oldPrecision: 9,
            oldScale: 6);
    }
}
