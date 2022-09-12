using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AdjustPlayerBankIfnoTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_player_bank_info_tags",
                schema: "dbo",
                table: "player_bank_info_tags");

            migrationBuilder.AlterColumn<string>(
                name: "player_code",
                schema: "dbo",
                table: "player_bank_info_tags",
                type: "character varying(16)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "pk_player_bank_info_tags",
                schema: "dbo",
                table: "player_bank_info_tags",
                columns: new[] { "brand_id", "bank_info_id", "tag_type" });

            migrationBuilder.CreateIndex(
                name: "ix_player_bank_info_tags_bank_info_id",
                schema: "dbo",
                table: "player_bank_info_tags",
                column: "bank_info_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_bank_info_tags_player_code",
                schema: "dbo",
                table: "player_bank_info_tags",
                column: "player_code");

            migrationBuilder.AddForeignKey(
                name: "fk_player_bank_info_tags_player_bank_infos_bank_info_id",
                schema: "dbo",
                table: "player_bank_info_tags",
                column: "bank_info_id",
                principalSchema: "dbo",
                principalTable: "player_bank_infos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_player_bank_info_tags_players_player_code",
                schema: "dbo",
                table: "player_bank_info_tags",
                column: "player_code",
                principalSchema: "dbo",
                principalTable: "players",
                principalColumn: "player_code",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_player_bank_info_tags_player_bank_infos_bank_info_id",
                schema: "dbo",
                table: "player_bank_info_tags");

            migrationBuilder.DropForeignKey(
                name: "fk_player_bank_info_tags_players_player_code",
                schema: "dbo",
                table: "player_bank_info_tags");

            migrationBuilder.DropPrimaryKey(
                name: "pk_player_bank_info_tags",
                schema: "dbo",
                table: "player_bank_info_tags");

            migrationBuilder.DropIndex(
                name: "ix_player_bank_info_tags_bank_info_id",
                schema: "dbo",
                table: "player_bank_info_tags");

            migrationBuilder.DropIndex(
                name: "ix_player_bank_info_tags_player_code",
                schema: "dbo",
                table: "player_bank_info_tags");

            migrationBuilder.AlterColumn<string>(
                name: "player_code",
                schema: "dbo",
                table: "player_bank_info_tags",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)");

            migrationBuilder.AddPrimaryKey(
                name: "pk_player_bank_info_tags",
                schema: "dbo",
                table: "player_bank_info_tags",
                columns: new[] { "brand_id", "player_code", "bank_info_id", "tag_type" });
        }
    }
}
