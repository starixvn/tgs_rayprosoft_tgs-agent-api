using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddPromotionContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "content",
                schema: "dbo",
                table: "promotions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "localization",
                schema: "dbo",
                table: "promotions",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sub_subject",
                schema: "dbo",
                table: "promotions",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "terms",
                schema: "dbo",
                table: "promotions",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "content",
                schema: "dbo",
                table: "promotions");

            migrationBuilder.DropColumn(
                name: "localization",
                schema: "dbo",
                table: "promotions");

            migrationBuilder.DropColumn(
                name: "sub_subject",
                schema: "dbo",
                table: "promotions");

            migrationBuilder.DropColumn(
                name: "terms",
                schema: "dbo",
                table: "promotions");
        }
    }
}
