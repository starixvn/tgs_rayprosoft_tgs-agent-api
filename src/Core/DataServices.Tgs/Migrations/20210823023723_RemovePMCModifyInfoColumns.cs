using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class RemovePMCModifyInfoColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_by_username",
                schema: "dbo",
                table: "payment_method_channels");

            migrationBuilder.DropColumn(
                name: "created_on",
                schema: "dbo",
                table: "payment_method_channels");

            migrationBuilder.DropColumn(
                name: "updated_by_username",
                schema: "dbo",
                table: "payment_method_channels");

            migrationBuilder.DropColumn(
                name: "updated_on",
                schema: "dbo",
                table: "payment_method_channels");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "created_by_username",
                schema: "dbo",
                table: "payment_method_channels",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_on",
                schema: "dbo",
                table: "payment_method_channels",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "Now()");

            migrationBuilder.AddColumn<string>(
                name: "updated_by_username",
                schema: "dbo",
                table: "payment_method_channels",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_on",
                schema: "dbo",
                table: "payment_method_channels",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "Now()");
        }
    }
}
