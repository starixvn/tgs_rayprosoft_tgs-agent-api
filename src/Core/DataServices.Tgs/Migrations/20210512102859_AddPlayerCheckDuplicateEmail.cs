using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddPlayerCheckDuplicateEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.IndexSqlFormat, "players_email"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
