using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System.Reflection;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class AddBrandGPAndGPTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "brand_game_provider_aggregate_audit_logs",
            schema: "dbo",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                brand_id = table.Column<Guid>(type: "uuid", nullable: true),
                created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_brand_game_provider_aggregate_audit_log_id", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "brand_game_provider_aggregate_records",
            schema: "report",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                audit_log_id = table.Column<int>(type: "integer", nullable: false),
                brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                data = table.Column<string>(type: "jsonb", nullable: false),
                created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_brand_game_provider_aggregate_record_id", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "brand_game_provider_aggregates",
            schema: "report",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                audit_log_id = table.Column<int>(type: "integer", nullable: false),
                brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                game_type = table.Column<int>(type: "integer", nullable: false),
                currency_code = table.Column<string>(type: "text", nullable: false),
                transaction_count = table.Column<int>(type: "integer", nullable: false),
                bet_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                valid_bet_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                turnover = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                settle_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                commission = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                report_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_brand_game_provider_aggregate_id", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "game_provider_aggregate_audit_logs",
            schema: "dbo",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                game_provider_id = table.Column<Guid>(type: "uuid", nullable: true),
                created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_game_provider_aggregate_audit_log_id", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "game_provider_aggregate_records",
            schema: "report",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                audit_log_id = table.Column<int>(type: "integer", nullable: false),
                game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                data = table.Column<string>(type: "jsonb", nullable: false),
                created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_game_provider_aggregate_record_id", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "game_provider_aggregates",
            schema: "report",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                audit_log_id = table.Column<int>(type: "integer", nullable: false),
                game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                currency_code = table.Column<string>(type: "text", nullable: false),
                transaction_count = table.Column<int>(type: "integer", nullable: false),
                bet_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                valid_bet_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                turnover = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                win_loss = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                settle_amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                commission = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                report_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_game_provider_aggregate_id", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_brand_game_provider_aggregate_audit_log_multiple_keys",
            schema: "dbo",
            table: "brand_game_provider_aggregate_audit_logs",
            columns: new[] { "created_on", "created_by_user_id", "brand_id" });

        migrationBuilder.CreateIndex(
            name: "ix_brand_game_provider_aggregate_record_audit_log_id",
            schema: "report",
            table: "brand_game_provider_aggregate_records",
            column: "audit_log_id");

        migrationBuilder.CreateIndex(
            name: "ix_brand_game_provider_aggregate_audit_log_id",
            schema: "report",
            table: "brand_game_provider_aggregates",
            column: "audit_log_id");

        migrationBuilder.CreateIndex(
            name: "ix_brand_game_provider_aggregate_multiple_keys",
            schema: "report",
            table: "brand_game_provider_aggregates",
            columns: new[] { "report_date", "brand_id", "game_provider_id" });

        migrationBuilder.CreateIndex(
            name: "ix_game_provider_aggregate_audit_log_created_on",
            schema: "dbo",
            table: "game_provider_aggregate_audit_logs",
            column: "created_on");

        migrationBuilder.CreateIndex(
            name: "ix_game_provider_aggregate_record_audit_log_id",
            schema: "report",
            table: "game_provider_aggregate_records",
            column: "audit_log_id");

        migrationBuilder.CreateIndex(
            name: "ix_game_provider_aggregate_audit_log_id",
            schema: "report",
            table: "game_provider_aggregates",
            column: "audit_log_id");

        migrationBuilder.CreateIndex(
            name: "ix_game_provider_aggregate_multiple_keys",
            schema: "report",
            table: "game_provider_aggregates",
            columns: new[] { "report_date", "game_provider_id" });

        var assembly = Assembly.GetExecutingAssembly();

        migrationBuilder.SqlResource(assembly,
            string.Format(Consts.FunctionSqlFormat, "report_generate_brand_game_provider_aggregate_by_closed_on"));

        migrationBuilder.SqlResource(assembly,
            string.Format(Consts.FunctionSqlFormat, "report_generate_game_provider_aggregate_by_closed_on"));

        migrationBuilder.Sql(
            "create index ix_rounds_closed_on_brand_id on bet.rounds (closed_on,brand_id);");

        migrationBuilder.Sql(
            "create index ix_rounds_external_closed_on_game_provider_id on bet.rounds (external_closed_on,game_provider_id);");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "brand_game_provider_aggregate_audit_logs",
            schema: "dbo");

        migrationBuilder.DropTable(
            name: "brand_game_provider_aggregate_records",
            schema: "report");

        migrationBuilder.DropTable(
            name: "brand_game_provider_aggregates",
            schema: "report");

        migrationBuilder.DropTable(
            name: "game_provider_aggregate_audit_logs",
            schema: "dbo");

        migrationBuilder.DropTable(
            name: "game_provider_aggregate_records",
            schema: "report");

        migrationBuilder.DropTable(
            name: "game_provider_aggregates",
            schema: "report");
    }
}
