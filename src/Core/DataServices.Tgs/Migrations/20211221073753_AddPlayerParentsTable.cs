using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class AddPlayerParentsTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "player_parents",
            schema: "dbo",
            columns: table => new
            {
                player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                parent_player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_player_parent_code", x => x.player_code);
            });

        migrationBuilder.CreateIndex("ix_parent_player_code", "player_parents", "parent_player_code","dbo");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "player_parents",
            schema: "dbo");
    }
}
