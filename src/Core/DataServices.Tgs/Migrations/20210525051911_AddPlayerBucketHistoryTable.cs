using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddPlayerBucketHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_player_buckets_players_player_code1",
                schema: "dbo",
                table: "player_buckets");

            migrationBuilder.DropIndex(
                name: "ix_player_buckets_player_code1",
                schema: "dbo",
                table: "player_buckets");

            migrationBuilder.DropColumn(
                name: "player_code1",
                schema: "dbo",
                table: "player_buckets");

            migrationBuilder.AlterColumn<string>(
                name: "player_code",
                schema: "dbo",
                table: "player_buckets",
                type: "character varying(16)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "bucket_type",
                schema: "dbo",
                table: "player_buckets",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<decimal>(
                name: "current_bucket_amount",
                schema: "dbo",
                table: "player_buckets",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "path",
                schema: "log",
                table: "api_logs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.CreateTable(
                name: "player_bucket_histories",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_code = table.Column<string>(type: "text", nullable: true),
                    turnover = table.Column<decimal>(type: "numeric", nullable: false),
                    from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    hour = table.Column<int>(type: "integer", nullable: false),
                    minute = table.Column<int>(type: "integer", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_bucket_histories", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_player_buckets_player_code",
                schema: "dbo",
                table: "player_buckets",
                column: "player_code");

            migrationBuilder.CreateIndex(
                name: "ix_player_bucket_histories_player_code",
                schema: "dbo",
                table: "player_bucket_histories",
                column: "player_code");

            migrationBuilder.AddForeignKey(
                name: "fk_player_buckets_players_player_code",
                schema: "dbo",
                table: "player_buckets",
                column: "player_code",
                principalSchema: "dbo",
                principalTable: "players",
                principalColumn: "player_code",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_player_buckets_players_player_code",
                schema: "dbo",
                table: "player_buckets");

            migrationBuilder.DropTable(
                name: "player_bucket_histories",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "ix_player_buckets_player_code",
                schema: "dbo",
                table: "player_buckets");

            migrationBuilder.DropColumn(
                name: "current_bucket_amount",
                schema: "dbo",
                table: "player_buckets");

            migrationBuilder.AlterColumn<string>(
                name: "player_code",
                schema: "dbo",
                table: "player_buckets",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(16)");

            migrationBuilder.AlterColumn<int>(
                name: "bucket_type",
                schema: "dbo",
                table: "player_buckets",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AddColumn<string>(
                name: "player_code1",
                schema: "dbo",
                table: "player_buckets",
                type: "character varying(16)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "path",
                schema: "log",
                table: "api_logs",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "ix_player_buckets_player_code1",
                schema: "dbo",
                table: "player_buckets",
                column: "player_code1");

            migrationBuilder.AddForeignKey(
                name: "fk_player_buckets_players_player_code1",
                schema: "dbo",
                table: "player_buckets",
                column: "player_code1",
                principalSchema: "dbo",
                principalTable: "players",
                principalColumn: "player_code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
