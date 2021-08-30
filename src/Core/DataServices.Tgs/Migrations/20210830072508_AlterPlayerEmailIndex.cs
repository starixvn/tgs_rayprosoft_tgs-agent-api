using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AlterPlayerEmailIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            migrationBuilder.Sql(@"
                drop index if exists dbo.ux_players_email;
            ");

            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.IndexSqlFormat, "players_brand_id_email"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}