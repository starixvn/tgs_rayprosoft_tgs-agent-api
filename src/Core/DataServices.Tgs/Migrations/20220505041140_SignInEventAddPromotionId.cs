using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class SignInEventAddPromotionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "promotion_id",
                schema: "dbo",
                table: "sign_in_events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_sign_in_events_promotion_id",
                schema: "dbo",
                table: "sign_in_events",
                column: "promotion_id");

            migrationBuilder.AddForeignKey(
                name: "fk_sign_in_events_promotions_promotion_id",
                schema: "dbo",
                table: "sign_in_events",
                column: "promotion_id",
                principalSchema: "dbo",
                principalTable: "promotions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sign_in_events_promotions_promotion_id",
                schema: "dbo",
                table: "sign_in_events");

            migrationBuilder.DropIndex(
                name: "ix_sign_in_events_promotion_id",
                schema: "dbo",
                table: "sign_in_events");

            migrationBuilder.DropColumn(
                name: "promotion_id",
                schema: "dbo",
                table: "sign_in_events");
        }
    }
}
