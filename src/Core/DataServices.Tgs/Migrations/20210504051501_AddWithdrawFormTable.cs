using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddWithdrawFormTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_deposit_forms_players_playerCode",
                schema: "dbo",
                table: "deposit_forms");

            migrationBuilder.DropIndex(
                name: "ix_player_bank_infos_brand_id",
                schema: "dbo",
                table: "player_bank_infos");

            migrationBuilder.DropIndex(
                name: "ix_player_bank_infos_payment_channel_id",
                schema: "dbo",
                table: "player_bank_infos");

            migrationBuilder.RenameIndex(
                name: "ix_deposit_forms_brandPaymentId",
                schema: "dbo",
                table: "deposit_forms",
                newName: "ix_deposit_forms_brand_payment_id");

            migrationBuilder.AddColumn<string>(
                name: "bank_type",
                schema: "dbo",
                table: "payment_methods",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "withdraw_forms",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_code = table.Column<string>(type: "character varying(16)", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    brand_payment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    remark = table.Column<string>(type: "text", nullable: true),
                    bank_info_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_info = table.Column<string>(type: "jsonb", nullable: false),
                    apply_info = table.Column<string>(type: "jsonb", nullable: true),
                    payment_return = table.Column<string>(type: "jsonb", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "Now()"),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "Now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_withdraw_forms", x => x.id);
                    table.ForeignKey(
                        name: "fk_withdraw_forms_brand_payments_brand_payment_id",
                        column: x => x.brand_payment_id,
                        principalSchema: "dbo",
                        principalTable: "brand_payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_withdraw_forms_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_withdraw_forms_players_player_code",
                        column: x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_player_bank_infos_brand_id",
                schema: "dbo",
                table: "player_bank_infos",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_bank_infos_payment_channel_id",
                schema: "dbo",
                table: "player_bank_infos",
                column: "payment_channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_withdraw_forms_brand_id",
                schema: "dbo",
                table: "withdraw_forms",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_withdraw_forms_player_code",
                schema: "dbo",
                table: "withdraw_forms",
                column: "player_code");

            migrationBuilder.CreateIndex(
                name: "ix_withdraw_froms_brand_payment_id",
                schema: "dbo",
                table: "withdraw_forms",
                column: "brand_payment_id");

            migrationBuilder.AddForeignKey(
                name: "fk_deposit_forms_players_player_code",
                schema: "dbo",
                table: "deposit_forms",
                column: "player_code",
                principalSchema: "dbo",
                principalTable: "players",
                principalColumn: "player_code",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_deposit_forms_players_player_code",
                schema: "dbo",
                table: "deposit_forms");

            migrationBuilder.DropTable(
                name: "withdraw_forms",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "ix_player_bank_infos_brand_id",
                schema: "dbo",
                table: "player_bank_infos");

            migrationBuilder.DropIndex(
                name: "ix_player_bank_infos_payment_channel_id",
                schema: "dbo",
                table: "player_bank_infos");

            migrationBuilder.DropColumn(
                name: "bank_type",
                schema: "dbo",
                table: "payment_methods");

            migrationBuilder.RenameIndex(
                name: "ix_deposit_forms_brand_payment_id",
                schema: "dbo",
                table: "deposit_forms",
                newName: "ix_deposit_forms_brandPaymentId");

            migrationBuilder.CreateIndex(
                name: "ix_player_bank_infos_brand_id",
                schema: "dbo",
                table: "player_bank_infos",
                column: "brand_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_player_bank_infos_payment_channel_id",
                schema: "dbo",
                table: "player_bank_infos",
                column: "payment_channel_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_deposit_forms_players_playerCode",
                schema: "dbo",
                table: "deposit_forms",
                column: "player_code",
                principalSchema: "dbo",
                principalTable: "players",
                principalColumn: "player_code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
