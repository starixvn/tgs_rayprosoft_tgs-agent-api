using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class UpdateBrandMaintenanceForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_brand_maintenance_brand_id",
                schema: "dbo",
                table: "brand_maintenance");

            migrationBuilder.CreateIndex(
                name: "ix_brand_maintenance_brand_id",
                schema: "dbo",
                table: "brand_maintenance",
                column: "brand_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_brand_maintenance_brand_id",
                schema: "dbo",
                table: "brand_maintenance");

            migrationBuilder.CreateIndex(
                name: "ix_brand_maintenance_brand_id",
                schema: "dbo",
                table: "brand_maintenance",
                column: "brand_id");
        }
    }
}
