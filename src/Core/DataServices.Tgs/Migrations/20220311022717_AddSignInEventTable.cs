using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class AddSignInEventTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "sign_in_events",
            schema: "dbo",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_on = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "Now()"),
                date = table.Column<DateTime>(type: "date", nullable: false),
                player_code = table.Column<string>(type: "character varying(16)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_sign_in_events", x => x.id);
                table.ForeignKey(
                    name: "fk_sign_in_events_brands_brand_id",
                    column: x => x.brand_id,
                    principalSchema: "dbo",
                    principalTable: "brands",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_sign_in_events_players_brand_id",
                    column: x => x.player_code,
                    principalSchema: "dbo",
                    principalTable: "players",
                    principalColumn: "player_code",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_sign_in_events_brand_id",
            schema: "dbo",
            table: "sign_in_events",
            column: "brand_id");

        migrationBuilder.CreateIndex(
            name: "ix_sign_in_events_player_code",
            schema: "dbo",
            table: "sign_in_events",
            column: "player_code");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "sign_in_events",
            schema: "dbo");
    }
}
