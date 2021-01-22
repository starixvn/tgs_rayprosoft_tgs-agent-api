using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Log.Migrations
{
    public partial class Initial
    {
        //InitAfterRun(migrationBuilder);
        public void InitAfterRun(MigrationBuilder migrationBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "log_get_admin_log"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "log_get_admin_logs"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "log_get_api_log"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "log_get_api_logs"));

            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.IndexSqlFormat, "adminLogs"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.IndexSqlFormat, "apiLogs"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.IndexSqlFormat, "LogRecords"));
        }
    }
}