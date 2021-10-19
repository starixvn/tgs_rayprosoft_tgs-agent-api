using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddPaidUserColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "paid_on",
                schema: "dbo",
                table: "withdraw_forms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "paid_user_id",
                schema: "dbo",
                table: "withdraw_forms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "paid_user_name",
                schema: "dbo",
                table: "withdraw_forms",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "paid_on",
                schema: "dbo",
                table: "withdraw_forms");

            migrationBuilder.DropColumn(
                name: "paid_user_id",
                schema: "dbo",
                table: "withdraw_forms");

            migrationBuilder.DropColumn(
                name: "paid_user_name",
                schema: "dbo",
                table: "withdraw_forms");
        }
    }
}
