using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AlterGetCachedBrandGamesSp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            migrationBuilder.Sql(@"
                drop function if exists dbo.brand_get_brand_games_for_cache;
            ");

            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "brand_get_brand_games_for_cache"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
