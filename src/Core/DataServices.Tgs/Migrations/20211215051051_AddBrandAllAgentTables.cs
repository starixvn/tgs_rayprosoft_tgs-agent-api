using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddBrandAllAgentTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "brand_all_agent_game_type_conditions",
                schema: "dbo",
                columns: table => new
                {
                    all_agent_game_type = table.Column<int>(type: "integer", nullable: false),
                    all_agent_rebate_type = table.Column<short>(type: "smallint", nullable: false),
                    rank = table.Column<int>(type: "integer", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    net_profit = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    active_number = table.Column<int>(type: "integer", nullable: false),
                    rebate_percentage = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_all_agent_game_type_conditions", x => new { x.brand_id, x.all_agent_game_type, x.rank, x.all_agent_rebate_type });
                });

            migrationBuilder.CreateTable(
                name: "brand_all_agent_game_types",
                schema: "dbo",
                columns: table => new
                {
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    all_agent_game_type = table.Column<short>(type: "smallint", nullable: false),
                    @switch = table.Column<bool>(name: "switch", type: "boolean", nullable: false),
                    has_valid_bet = table.Column<bool>(type: "boolean", nullable: false),
                    has_win_loss = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_all_agent_game_types", x => new { x.brand_id, x.all_agent_game_type });
                });

            migrationBuilder.CreateTable(
                name: "brand_all_agents",
                schema: "dbo",
                columns: table => new
                {
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_all_agents", x => x.brand_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "brand_all_agent_game_type_conditions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brand_all_agent_game_types",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "brand_all_agents",
                schema: "dbo");
        }
    }
}
