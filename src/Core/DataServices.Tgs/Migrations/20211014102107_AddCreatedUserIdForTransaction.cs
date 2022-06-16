using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Tgs.Migrations;

public partial class AddCreatedUserIdForTransaction : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "created_by_user_id",
            schema: "dbo",
            table: "transactions",
            type: "uuid",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "sent_by_username",
            schema: "dbo",
            table: "promotion_players",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "sent_by_username",
            schema: "dbo",
            table: "player_rebates",
            type: "text",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "created_by_user_id",
            schema: "dbo",
            table: "transactions");

        migrationBuilder.DropColumn(
            name: "sent_by_username",
            schema: "dbo",
            table: "promotion_players");

        migrationBuilder.DropColumn(
            name: "sent_by_username",
            schema: "dbo",
            table: "player_rebates");
    }
}
