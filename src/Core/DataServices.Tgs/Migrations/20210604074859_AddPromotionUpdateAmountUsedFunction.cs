using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddPromotionUpdateAmountUsedFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "promotion_revert_amount_used"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.ProcedureSqlFormat, "promotion_update_amount_used"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}