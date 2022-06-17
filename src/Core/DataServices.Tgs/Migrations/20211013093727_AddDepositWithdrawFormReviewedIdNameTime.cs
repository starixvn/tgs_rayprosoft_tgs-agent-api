using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class AddDepositWithdrawFormReviewedIdNameTime : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "reviewed_on",
            schema: "dbo",
            table: "withdraw_forms",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "reviewed_user_id",
            schema: "dbo",
            table: "withdraw_forms",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "reviewed_user_name",
            schema: "dbo",
            table: "withdraw_forms",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "reviewed_on",
            schema: "dbo",
            table: "deposit_forms",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "reviewed_user_id",
            schema: "dbo",
            table: "deposit_forms",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "reviewed_user_name",
            schema: "dbo",
            table: "deposit_forms",
            type: "text",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "reviewed_on",
            schema: "dbo",
            table: "withdraw_forms");

        migrationBuilder.DropColumn(
            name: "reviewed_user_id",
            schema: "dbo",
            table: "withdraw_forms");

        migrationBuilder.DropColumn(
            name: "reviewed_user_name",
            schema: "dbo",
            table: "withdraw_forms");

        migrationBuilder.DropColumn(
            name: "reviewed_on",
            schema: "dbo",
            table: "deposit_forms");

        migrationBuilder.DropColumn(
            name: "reviewed_user_id",
            schema: "dbo",
            table: "deposit_forms");

        migrationBuilder.DropColumn(
            name: "reviewed_user_name",
            schema: "dbo",
            table: "deposit_forms");
    }
}
