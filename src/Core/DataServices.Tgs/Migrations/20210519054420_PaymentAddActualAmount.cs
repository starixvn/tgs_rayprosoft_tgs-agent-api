using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class PaymentAddActualAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                schema: "dbo",
                table: "withdraw_forms",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<decimal>(
                name: "actual_amount",
                schema: "dbo",
                table: "withdraw_forms",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "actual_amount",
                schema: "dbo",
                table: "deposit_forms",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "promotion_id",
                schema: "dbo",
                table: "deposit_forms",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "actual_amount",
                schema: "dbo",
                table: "withdraw_forms");

            migrationBuilder.DropColumn(
                name: "actual_amount",
                schema: "dbo",
                table: "deposit_forms");

            migrationBuilder.DropColumn(
                name: "promotion_id",
                schema: "dbo",
                table: "deposit_forms");

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                schema: "dbo",
                table: "withdraw_forms",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)",
                oldPrecision: 18,
                oldScale: 6);
        }
    }
}
