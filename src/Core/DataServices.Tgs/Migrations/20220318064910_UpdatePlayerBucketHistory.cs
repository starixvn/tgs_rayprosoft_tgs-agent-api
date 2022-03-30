using System;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class UpdatePlayerBucketHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hour",
                schema: "dbo",
                table: "player_bucket_histories");

            migrationBuilder.DropColumn(
                name: "minute",
                schema: "dbo",
                table: "player_bucket_histories");

            migrationBuilder.AlterColumn<string>(
                name: "player_code",
                schema: "dbo",
                table: "player_bucket_histories",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "player_bucket_id",
                schema: "dbo",
                table: "player_bucket_histories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "round_data",
                schema: "dbo",
                table: "player_bucket_histories",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");

            var assembly = Assembly.GetExecutingAssembly();

            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "report_get_round_and_set_game_setting_aggregates"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "player_bucket_id",
                schema: "dbo",
                table: "player_bucket_histories");

            migrationBuilder.DropColumn(
                name: "round_data",
                schema: "dbo",
                table: "player_bucket_histories");

            migrationBuilder.AlterColumn<string>(
                name: "player_code",
                schema: "dbo",
                table: "player_bucket_histories",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AddColumn<int>(
                name: "hour",
                schema: "dbo",
                table: "player_bucket_histories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "minute",
                schema: "dbo",
                table: "player_bucket_histories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
