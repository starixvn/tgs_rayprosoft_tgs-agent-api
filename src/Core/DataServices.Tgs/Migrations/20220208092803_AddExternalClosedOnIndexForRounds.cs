using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class AddExternalClosedOnIndexForRounds : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        migrationBuilder.SqlResource(assembly,
            string.Format(Consts.IndexSqlFormat, "rounds_external_closed_on"));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
}
