using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddServerNameFilterToAdminLogAndLogApi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "log_get_admin_logs"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "log_get_api_logs"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "log_get_log_records"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}