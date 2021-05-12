using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AlterGetGameProviderCacheSp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "drop function if exists dbo.game_provider_get_game_provider_for_cache(uuid);");

            var assembly = Assembly.GetExecutingAssembly();
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "game_provider_get_game_provider_for_cache"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}