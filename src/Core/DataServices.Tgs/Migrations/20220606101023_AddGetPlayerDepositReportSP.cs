using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class AddGetPlayerDepositReportSP : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        migrationBuilder.SqlResource(assembly,
            string.Format(Consts.FunctionSqlFormat, "report_get_player_first_deposit_details"));
        migrationBuilder.SqlResource(assembly,
            string.Format(Consts.FunctionSqlFormat, "report_get_player_no_second_deposit_details"));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
}
