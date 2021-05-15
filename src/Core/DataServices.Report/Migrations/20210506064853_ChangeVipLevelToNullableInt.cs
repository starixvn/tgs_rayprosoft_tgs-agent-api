using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Report.Migrations
{
    public partial class ChangeVipLevelToNullableInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "vip_level",
                schema: "dbo",
                table: "player_game_type_rebate_aggregates",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "vip_level",
                schema: "dbo",
                table: "player_game_type_rebate_aggregates",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
