using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddInnerExceptionColumnForAdminLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "inner_exception",
                schema: "log",
                table: "admin_logs",
                type: "text",
                nullable: true);

            var assembly = Assembly.GetExecutingAssembly();

            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "log_get_admin_logs"));
        }

        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn(
            "inner_exception",
            schema: "log",
            table: "admin_logs");
    }
}