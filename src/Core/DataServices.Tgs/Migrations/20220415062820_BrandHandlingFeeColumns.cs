using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class BrandHandlingFeeColumns : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "deposit_handling_fee_rate_ten_percentile",
            schema: "dbo",
            table: "brands",
            type: "numeric",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<decimal>(
            name: "withdraw_handling_fee_rate_ten_percentile",
            schema: "dbo",
            table: "brands",
            type: "numeric",
            nullable: false,
            defaultValue: 0m);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "deposit_handling_fee_rate_ten_percentile",
            schema: "dbo",
            table: "brands");

        migrationBuilder.DropColumn(
            name: "withdraw_handling_fee_rate_ten_percentile",
            schema: "dbo",
            table: "brands");
    }
}
