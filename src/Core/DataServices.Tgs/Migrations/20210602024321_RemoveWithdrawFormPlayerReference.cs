using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class RemoveWithdrawFormPlayerReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_withdraw_forms_players_player_code",
                schema: "dbo",
                table: "withdraw_forms");

            migrationBuilder.DropIndex(
                name: "ix_withdraw_forms_player_code",
                schema: "dbo",
                table: "withdraw_forms");

            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                schema: "dbo",
                table: "withdraw_forms",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(16)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                schema: "dbo",
                table: "withdraw_forms",
                type: "character varying(16)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_withdraw_forms_player_code",
                schema: "dbo",
                table: "withdraw_forms",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_withdraw_forms_players_player_code",
                schema: "dbo",
                table: "withdraw_forms",
                column: "user_id",
                principalSchema: "dbo",
                principalTable: "players",
                principalColumn: "player_code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
