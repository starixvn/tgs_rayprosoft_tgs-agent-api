using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class RemoveTransactionPromotionFK : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_transaction_promotion_transaction_id",
            schema: "dbo",
            table: "transaction_promotions");

        migrationBuilder.DropIndex(
            name: "ix_transactions_transaction_id",
            schema: "dbo",
            table: "transaction_promotions");

        migrationBuilder.CreateIndex(
            name: "ix_transactions_transaction_id",
            schema: "dbo",
            table: "transaction_promotions",
            column: "transaction_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_transactions_transaction_id",
            schema: "dbo",
            table: "transaction_promotions");

        migrationBuilder.CreateIndex(
            name: "ix_transactions_transaction_id",
            schema: "dbo",
            table: "transaction_promotions",
            column: "transaction_id",
            unique: true);

        migrationBuilder.AddForeignKey(
            name: "fk_transaction_promotion_transaction_id",
            schema: "dbo",
            table: "transaction_promotions",
            column: "transaction_id",
            principalSchema: "dbo",
            principalTable: "transactions",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
