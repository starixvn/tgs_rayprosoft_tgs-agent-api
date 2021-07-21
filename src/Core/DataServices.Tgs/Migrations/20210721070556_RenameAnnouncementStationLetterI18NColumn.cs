using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class RenameAnnouncementStationLetterI18NColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "language_content",
                schema: "dbo",
                table: "station_letters",
                newName: "localization");

            migrationBuilder.RenameColumn(
                name: "language_content",
                schema: "dbo",
                table: "announcements",
                newName: "localization");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "localization",
                schema: "dbo",
                table: "station_letters",
                newName: "language_content");

            migrationBuilder.RenameColumn(
                name: "localization",
                schema: "dbo",
                table: "announcements",
                newName: "language_content");
        }
    }
}
