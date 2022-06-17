using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class AddDepositWithdrawOrderNo : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "order_no",
            schema: "dbo",
            table: "withdraw_forms",
            type: "character varying(50)",
            maxLength: 50,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "order_no",
            schema: "dbo",
            table: "deposit_forms",
            type: "character varying(50)",
            maxLength: 50,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "order_no",
            schema: "dbo",
            table: "withdraw_forms");

        migrationBuilder.DropColumn(
            name: "order_no",
            schema: "dbo",
            table: "deposit_forms");
    }
}
