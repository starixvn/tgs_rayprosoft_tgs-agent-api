using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class RefactorWithdrawForm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "user_name",
                schema: "dbo",
                table: "withdraw_forms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "user_type",
                schema: "dbo",
                table: "withdraw_forms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            var assembly = Assembly.GetExecutingAssembly();

            migrationBuilder.SqlResource(
                assembly,
                $"TGS.Core.DataServices.Tgs.Migrations.RefactorWithdrawForm.sql");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_name",
                schema: "dbo",
                table: "withdraw_forms");

            migrationBuilder.DropColumn(
                name: "user_type",
                schema: "dbo",
                table: "withdraw_forms");
        }
    }
}
