using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class FixPaymentMethodTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bank_type",
                schema: "dbo",
                table: "payment_methods",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bank_type",
                schema: "dbo",
                table: "payment_methods");
        }
    }
}
