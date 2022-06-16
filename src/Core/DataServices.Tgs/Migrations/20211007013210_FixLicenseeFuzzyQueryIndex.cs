using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class FixLicenseeFuzzyQueryIndex : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        migrationBuilder.SqlResource(assembly,
            string.Format(Consts.IndexSqlFormat, "licensee_fuzzy_query_trgm"));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
}
