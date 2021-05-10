using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddTransactionGameProviderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "game_provider_fund_transfer_transactions",
                schema: "dbo",
                columns: table => new
                {
                    game_provider_fund_transfer_transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: true),
                    game_provider_id = table.Column<Guid>(type: "uuid", nullable: false),
                    wallet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_provider_transaction_id = table.Column<string>(type: "text", nullable: true),
                    player_code = table.Column<string>(type: "text", nullable: false),
                    fund_source = table.Column<short>(type: "smallint", nullable: false),
                    from_wallet = table.Column<string>(type: "text", nullable: false),
                    to_wallet = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    balance = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_provider_fund_transfer_transaction_id", x => x.game_provider_fund_transfer_transaction_id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_game_provider_fund_transfer_transactions_player_code",
                schema: "dbo",
                table: "game_provider_fund_transfer_transactions",
                column: "player_code");

            migrationBuilder.CreateIndex(
                name: "ix_game_provider_fund_transfer_transactions_transaction_id",
                schema: "dbo",
                table: "game_provider_fund_transfer_transactions",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_provider_fund_transfer_transactions_wallet_id",
                schema: "dbo",
                table: "game_provider_fund_transfer_transactions",
                column: "wallet_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_provider_fund_transfer_transactions",
                schema: "dbo");
        }
    }
}
