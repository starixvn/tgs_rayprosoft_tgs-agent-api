using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class AddColumnsToPaymentHandlingFees : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "approved_on",
            schema: "dbo",
            table: "payment_handling_fees",
            type: "timestamp without time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<DateTime>(
            name: "created_on",
            schema: "dbo",
            table: "payment_handling_fees",
            type: "timestamp without time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<string>(
            name: "user_id",
            schema: "dbo",
            table: "payment_handling_fees",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "user_type",
            schema: "dbo",
            table: "payment_handling_fees",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "approved_on",
            schema: "dbo",
            table: "payment_handling_fees");

        migrationBuilder.DropColumn(
            name: "created_on",
            schema: "dbo",
            table: "payment_handling_fees");

        migrationBuilder.DropColumn(
            name: "user_id",
            schema: "dbo",
            table: "payment_handling_fees");

        migrationBuilder.DropColumn(
            name: "user_type",
            schema: "dbo",
            table: "payment_handling_fees");
    }
}
