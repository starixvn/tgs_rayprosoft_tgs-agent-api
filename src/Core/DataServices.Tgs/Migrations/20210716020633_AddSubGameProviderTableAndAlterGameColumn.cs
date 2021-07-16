using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddSubGameProviderTableAndAlterGameColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "sub_game_provider_code",
                schema: "dbo",
                table: "games",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sub_game_providers",
                schema: "dbo",
                columns: table => new
                {
                    code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                    localization = table.Column<string>(type: "jsonb", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sub_game_providers", x => x.code);
                    table.ForeignKey(
                        name: "fk_sub_game_providers_game_providers_game_provider_id",
                        column: x => x.game_provider_id,
                        principalSchema: "dbo",
                        principalTable: "game_providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_games_sub_game_provider_code",
                schema: "dbo",
                table: "games",
                column: "sub_game_provider_code");

            migrationBuilder.CreateIndex(
                name: "ix_sub_game_providers_game_provider_id",
                schema: "dbo",
                table: "sub_game_providers",
                column: "game_provider_id");

            migrationBuilder.AddForeignKey(
                name: "fk_games_sub_game_providers_sub_game_provider_code",
                schema: "dbo",
                table: "games",
                column: "sub_game_provider_code",
                principalSchema: "dbo",
                principalTable: "sub_game_providers",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_games_sub_game_providers_sub_game_provider_code",
                schema: "dbo",
                table: "games");

            migrationBuilder.DropTable(
                name: "sub_game_providers",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "ix_games_sub_game_provider_code",
                schema: "dbo",
                table: "games");

            migrationBuilder.DropColumn(
                name: "sub_game_provider_code",
                schema: "dbo",
                table: "games");
        }
    }
}
