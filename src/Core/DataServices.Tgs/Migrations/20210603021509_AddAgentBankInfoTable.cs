using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddAgentBankInfoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "agent_bank_infos",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_type = table.Column<string>(type: "text", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    detail = table.Column<string>(type: "jsonb", nullable: true),
                    payment_channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_code = table.Column<string>(type: "character varying(16)", nullable: true),
                    updated_by_username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agent_bank_infos", x => x.id);
                    table.ForeignKey(
                        name: "fk_agent_bank_infos_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_agent_bank_infos_payment_channels_payment_channel_id",
                        column: x => x.payment_channel_id,
                        principalSchema: "dbo",
                        principalTable: "payment_channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_agent_bank_infos_players_player_code",
                        column: x => x.player_code,
                        principalSchema: "dbo",
                        principalTable: "players",
                        principalColumn: "player_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_agent_bank_infos_agent_id",
                schema: "dbo",
                table: "agent_bank_infos",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                name: "ix_agent_bank_infos_brand_id",
                schema: "dbo",
                table: "agent_bank_infos",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_agent_bank_infos_payment_channel_id",
                schema: "dbo",
                table: "agent_bank_infos",
                column: "payment_channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_agent_bank_infos_player_code",
                schema: "dbo",
                table: "agent_bank_infos",
                column: "player_code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agent_bank_infos",
                schema: "dbo");
        }
    }
}
