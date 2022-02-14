using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class Player_Identity_Authentication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "enable_player_identity",
                schema: "dbo",
                table: "brands",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "player_authentication_forms",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    data = table.Column<string>(type: "jsonb", nullable: false),
                    player_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    reviewed_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    reviewed_user_id = table.Column<string>(type: "text", nullable: true),
                    reviewed_user_name = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_authentication_forms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "player_identity",
                schema: "dbo",
                columns: table => new
                {
                    player_code = table.Column<string>(type: "character varying(16)", nullable: false),
                    player_authentication_form_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_identity", x => x.player_code);
                    table.ForeignKey(
                        name: "fk_player_identity_players",
                        column: x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_authentication_forms",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "player_identity",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "enable_player_identity",
                schema: "dbo",
                table: "brands");
        }
    }
}
