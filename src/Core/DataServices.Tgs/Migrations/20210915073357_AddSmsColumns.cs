using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddSmsColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "area_code",
                schema: "dbo",
                table: "players",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "enable_sms",
                schema: "dbo",
                table: "brands",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "sms_vendor_code",
                schema: "dbo",
                table: "brands",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "area_code",
                schema: "dbo",
                table: "players");

            migrationBuilder.DropColumn(
                name: "enable_sms",
                schema: "dbo",
                table: "brands");

            migrationBuilder.DropColumn(
                name: "sms_vendor_code",
                schema: "dbo",
                table: "brands");
        }
    }
}
