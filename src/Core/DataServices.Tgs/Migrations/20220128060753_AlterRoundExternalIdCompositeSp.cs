using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class AlterRoundExternalIdCompositeSp : Migration
{
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP INDEX IF EXISTS bet.ix_rounds_external_round_id_composite;");

        migrationBuilder.CreateIndex(
            name: "ux_rounds_external_round_id_composite",
            schema: "bet",
            table: "rounds",
            columns: new[] { "external_round_id", "player_code", "game_provider_id", "game_id" },
            unique: true)
            .Annotation("Npgsql:IndexInclude", new[] { "game_provider_configuration_id", "wallet_id" });
    }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP INDEX IF EXISTS bet.ux_rounds_external_round_id_composite;");

        var assembly = Assembly.GetExecutingAssembly();

        migrationBuilder.SqlResource(assembly,
            string.Format(Consts.IndexSqlFormat, "ix_rounds_external_round_id_composite"));
    }
}
