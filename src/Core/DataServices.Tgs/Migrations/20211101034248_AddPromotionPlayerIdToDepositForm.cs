using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class AddPromotionPlayerIdToDepositForm : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "promotion_player_id",
            schema: "dbo",
            table: "deposit_forms",
            type: "integer",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "promotion_player_id",
            schema: "dbo",
            table: "deposit_forms");
    }
}
