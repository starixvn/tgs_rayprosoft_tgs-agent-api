using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddPlayerCodeToTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "fk_agent_transactions_agent_wallets_wallet_id",
                schema: "dbo",
                table: "agent_transactions");

            migrationBuilder.DropColumn(
                "confirmed_on",
                schema: "dbo",
                table: "transactions");

            migrationBuilder.DropColumn(
                "confirmed_on",
                schema: "dbo",
                table: "agent_transactions");

            migrationBuilder.AlterColumn<short>(
                "type",
                schema: "dbo",
                table: "transactions",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                "sent_on",
                schema: "dbo",
                table: "transactions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                "player_code",
                schema: "dbo",
                table: "transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                "player_code",
                schema: "bet",
                table: "game_actions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<short>(
                "type",
                schema: "dbo",
                table: "agent_transactions",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                "sent_on",
                schema: "dbo",
                table: "agent_transactions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<decimal>(
                "balance",
                schema: "dbo",
                table: "agent_transactions",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                "amount",
                schema: "dbo",
                table: "agent_transactions",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                "agent_id",
                schema: "dbo",
                table: "agent_transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                "ix_transactions_player_code",
                schema: "dbo",
                table: "transactions",
                column: "player_code");

            migrationBuilder.CreateIndex(
                "ix_game_actions_player_code",
                schema: "bet",
                table: "game_actions",
                column: "player_code");

            migrationBuilder.CreateIndex(
                "ix_agent_transactions_agent_id",
                schema: "dbo",
                table: "agent_transactions",
                column: "agent_id");

            migrationBuilder.AddForeignKey(
                "fk_agent_transaction_wallet_id",
                schema: "dbo",
                table: "agent_transactions",
                column: "wallet_id",
                principalSchema: "dbo",
                principalTable: "agent_wallets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            var assembly = Assembly.GetExecutingAssembly();

            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "report_get_player_account_change_records"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "fk_agent_transaction_wallet_id",
                schema: "dbo",
                table: "agent_transactions");

            migrationBuilder.DropIndex(
                "ix_transactions_player_code",
                schema: "dbo",
                table: "transactions");

            migrationBuilder.DropIndex(
                "ix_game_actions_player_code",
                schema: "bet",
                table: "game_actions");

            migrationBuilder.DropIndex(
                "ix_agent_transactions_agent_id",
                schema: "dbo",
                table: "agent_transactions");

            migrationBuilder.DropColumn(
                "player_code",
                schema: "dbo",
                table: "transactions");

            migrationBuilder.DropColumn(
                "player_code",
                schema: "bet",
                table: "game_actions");

            migrationBuilder.DropColumn(
                "agent_id",
                schema: "dbo",
                table: "agent_transactions");

            migrationBuilder.AlterColumn<int>(
                "type",
                schema: "dbo",
                table: "transactions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                "sent_on",
                schema: "dbo",
                table: "transactions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    new(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                "confirmed_on",
                schema: "dbo",
                table: "transactions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    new(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<int>(
                "type",
                schema: "dbo",
                table: "agent_transactions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                "sent_on",
                schema: "dbo",
                table: "agent_transactions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    new(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                "balance",
                schema: "dbo",
                table: "agent_transactions",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)",
                oldPrecision: 18,
                oldScale: 6);

            migrationBuilder.AlterColumn<decimal>(
                "amount",
                schema: "dbo",
                table: "agent_transactions",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)",
                oldPrecision: 18,
                oldScale: 6);

            migrationBuilder.AddColumn<DateTimeOffset>(
                "confirmed_on",
                schema: "dbo",
                table: "agent_transactions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    new(0, 0, 0, 0, 0)));

            migrationBuilder.AddForeignKey(
                "fk_agent_transactions_agent_wallets_wallet_id",
                schema: "dbo",
                table: "agent_transactions",
                column: "wallet_id",
                principalSchema: "dbo",
                principalTable: "agent_wallets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}