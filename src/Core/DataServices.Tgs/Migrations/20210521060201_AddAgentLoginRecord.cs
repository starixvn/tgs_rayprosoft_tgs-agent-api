using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddAgentLoginRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "agent_login_records",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    agent_id = table.Column<Guid>("uuid", nullable: false),
                    brand_id = table.Column<Guid>("uuid", nullable: false),
                    status = table.Column<short>("smallint", nullable: false),
                    ip_address = table.Column<string>("text", nullable: true),
                    created_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                    expired_on = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agent_login_records", x => x.id);
                    table.ForeignKey(
                        "fk_agent_login_records_agent_id",
                        x => x.agent_id,
                        principalSchema: "dbo",
                        principalTable: "agents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "fk_agent_login_records_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "brand_domains",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>("uuid", nullable: false),
                    url = table.Column<string>("character varying(128)", maxLength: 128, nullable: false),
                    brand_id = table.Column<Guid>("uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brand_domains", x => x.id);
                    table.ForeignKey(
                        "fk_brand_domains_brands_brand_id",
                        x => x.brand_id,
                        principalSchema: "dbo",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "ix_agent_login_records_agent_id",
                schema: "dbo",
                table: "agent_login_records",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                "ix_agent_login_records_brand_id",
                schema: "dbo",
                table: "agent_login_records",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                "ix_brand_domains_brand_id",
                schema: "dbo",
                table: "brand_domains",
                column: "brand_id");

            var assembly = Assembly.GetExecutingAssembly();

            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.FunctionSqlFormat, "report_get_round_aggregates"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "agent_login_records",
                "dbo");

            migrationBuilder.DropTable(
                "brand_domains",
                "dbo");
        }
    }
}