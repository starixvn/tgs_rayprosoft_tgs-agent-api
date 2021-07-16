using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class UpdatePromotionParticipantTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_promotion_participant_player_code",
                schema: "dbo",
                table: "promotion_participants");

            migrationBuilder.DropPrimaryKey(
                name: "pk_promotion_participants",
                schema: "dbo",
                table: "promotion_participants");

            migrationBuilder.AlterColumn<string>(
                name: "player_code",
                schema: "dbo",
                table: "promotion_participants",
                type: "character varying(16)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(16)");

            migrationBuilder.AddColumn<int>(
                name: "id",
                schema: "dbo",
                table: "promotion_participants",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_on",
                schema: "dbo",
                table: "promotion_participants",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddPrimaryKey(
                name: "pk_promotion_participants_id",
                schema: "dbo",
                table: "promotion_participants",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_promotion_participants_promotion_id",
                schema: "dbo",
                table: "promotion_participants",
                column: "promotion_id");

            migrationBuilder.AddForeignKey(
                name: "fk_promotion_participant_player_code",
                schema: "dbo",
                table: "promotion_participants",
                column: "player_code",
                principalSchema: "dbo",
                principalTable: "players",
                principalColumn: "player_code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_promotion_participant_player_code",
                schema: "dbo",
                table: "promotion_participants");

            migrationBuilder.DropPrimaryKey(
                name: "pk_promotion_participants_id",
                schema: "dbo",
                table: "promotion_participants");

            migrationBuilder.DropIndex(
                name: "ix_promotion_participants_promotion_id",
                schema: "dbo",
                table: "promotion_participants");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "dbo",
                table: "promotion_participants");

            migrationBuilder.DropColumn(
                name: "created_on",
                schema: "dbo",
                table: "promotion_participants");

            migrationBuilder.AlterColumn<string>(
                name: "player_code",
                schema: "dbo",
                table: "promotion_participants",
                type: "character varying(16)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_promotion_participants",
                schema: "dbo",
                table: "promotion_participants",
                columns: new[] { "promotion_id", "player_code" });

            migrationBuilder.AddForeignKey(
                name: "fk_promotion_participant_player_code",
                schema: "dbo",
                table: "promotion_participants",
                column: "player_code",
                principalSchema: "dbo",
                principalTable: "players",
                principalColumn: "player_code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
