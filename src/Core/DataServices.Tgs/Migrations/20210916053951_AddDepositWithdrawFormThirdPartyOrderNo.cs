using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddDepositWithdrawFormThirdPartyOrderNo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "third_party_order_no",
                schema: "dbo",
                table: "withdraw_forms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "third_party_order_no",
                schema: "dbo",
                table: "deposit_forms",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "third_party_order_no",
                schema: "dbo",
                table: "withdraw_forms");

            migrationBuilder.DropColumn(
                name: "third_party_order_no",
                schema: "dbo",
                table: "deposit_forms");
        }
    }
}
