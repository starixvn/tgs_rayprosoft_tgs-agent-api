using Microsoft.EntityFrameworkCore.Migrations;
using static TGS.Core.Models.Consts;

namespace TGS.Core.DataServices.Tgs.Migrations
{
    public partial class AddBrandCodeConstract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS dbo.ix_trgm_resellers_code_name_description_operational_email;

                CREATE INDEX ix_trgm_resellers_code_name_description_operational_email ON dbo.resellers USING gin (
                    code gin_trgm_ops,
                    name gin_trgm_ops,
                    description gin_trgm_ops,
                    operational_email gin_trgm_ops);

                CREATE UNIQUE INDEX ux_brands_code_lowercase ON dbo.brands (
                    lower(code::TEXT)
                );

                CREATE UNIQUE INDEX ux_resellers_code_lowercase ON dbo.resellers (
                    lower(code::TEXT)
                );
                
                CREATE UNIQUE INDEX ux_licensees_code_lowercase ON dbo.licensees (
                    lower(code::TEXT)
                );
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS dbo.ux_brands_code_lowercase;

                DROP INDEX IF EXISTS dbo.ux_resellers_code_lowercase;

                DROP INDEX IF EXISTS dbo.ux_licensees_code_lowercase;
            ");
        }
    }
}
