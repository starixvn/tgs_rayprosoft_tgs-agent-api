using System;
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
                name: "subject",
                schema: "dbo",
                table: "promotions",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "terms",
                schema: "dbo",
                table: "promotions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "promotion_images",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    platform_type = table.Column<int>(type: "integer", nullable: false),
                    promotion_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_promotion_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_promotion_image_promotion_id",
                        column: x => x.promotion_id,
                        principalSchema: "dbo",
                        principalTable: "promotions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_promotion_image_promotion_id",
                schema: "dbo",
                table: "promotion_images",
                column: "promotion_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "promotion_images",
                schema: "dbo");

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
                name: "subject",
                schema: "dbo",
                table: "promotions");

            migrationBuilder.DropColumn(
                name: "terms",
                schema: "dbo",
                table: "promotions");
        }
    }
}
