using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class RemoveBrandPaymentNonUseColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fee_percentage",
                schema: "dbo",
                table: "brand_payments");

            migrationBuilder.DropColumn(
                name: "highest_fee",
                schema: "dbo",
                table: "brand_payments");

            migrationBuilder.DropColumn(
                name: "lowest_fee",
                schema: "dbo",
                table: "brand_payments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "fee_percentage",
                schema: "dbo",
                table: "brand_payments",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "highest_fee",
                schema: "dbo",
                table: "brand_payments",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "lowest_fee",
                schema: "dbo",
                table: "brand_payments",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);
        }
    }
}
