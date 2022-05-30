using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AdjustPromotionDistributeColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "promotion_type",
                schema: "dbo",
                table: "promotion_distributes");

            migrationBuilder.AddColumn<decimal>(
                name: "bucket",
                schema: "dbo",
                table: "promotion_distributes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bucket",
                schema: "dbo",
                table: "promotion_distributes");

            migrationBuilder.AddColumn<string>(
                name: "promotion_type",
                schema: "dbo",
                table: "promotion_distributes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
