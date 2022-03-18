using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddNewPromotionDistributeTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "campaign_detail",
                schema: "dbo",
                table: "promotions",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.CreateTable(
                name: "promotion_distribute_plans",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    condition_hash = table.Column<long>(type: "bigint", nullable: false),
                    detail = table.Column<string>(type: "jsonb", nullable: true),
                    promotion_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_promotion_distribute_plans", x => x.id);
                    table.ForeignKey(
                        name: "fk_promotion_distribute_plans_promotions",
                        column: x => x.promotion_id,
                        principalSchema: "dbo",
                        principalTable: "promotions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "promotion_distributes",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bonus_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    plan_id = table.Column<Guid>(type: "uuid", nullable: true),
                    player_code = table.Column<string>(type: "text", nullable: false),
                    promotion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    promotion_type = table.Column<string>(type: "text", nullable: false),
                    distributor = table.Column<string>(type: "text", nullable: true),
                    sent_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_promotion_distributes", x => x.id);
                    table.ForeignKey(
                        name: "fk_promotion_distributes_promotions",
                        column: x => x.promotion_id,
                        principalSchema: "dbo",
                        principalTable: "promotions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_promotion_distribute_plans_promotion_id",
                schema: "dbo",
                table: "promotion_distribute_plans",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "ux_promotion_distrubute_plans",
                schema: "dbo",
                table: "promotion_distribute_plans",
                columns: new[] { "brand_id", "promotion_id", "condition_hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_promotion_distributes_promotion_id",
                schema: "dbo",
                table: "promotion_distributes",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "ux_promotion_distributes",
                schema: "dbo",
                table: "promotion_distributes",
                columns: new[] { "player_code", "plan_id" },
                unique: true,
                filter: "plan_id IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "promotion_distribute_plans",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "promotion_distributes",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "campaign_detail",
                schema: "dbo",
                table: "promotions");
        }
    }
}
