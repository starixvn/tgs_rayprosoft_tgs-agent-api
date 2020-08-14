using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Migrations
{
    public partial class addTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrandCurrency_Brands_BrandId",
                schema: "dbo",
                table: "BrandCurrency");

            migrationBuilder.DropForeignKey(
                name: "FK_BrandCurrency_Currencies_CurrencyCode",
                schema: "dbo",
                table: "BrandCurrency");

            migrationBuilder.DropForeignKey(
                name: "FK_GameProviderCurrency_Currencies_CurrencyCode",
                schema: "dbo",
                table: "GameProviderCurrency");

            migrationBuilder.DropForeignKey(
                name: "FK_GameProviderCurrency_GameProviders_GameProviderId",
                schema: "dbo",
                table: "GameProviderCurrency");

            migrationBuilder.DropForeignKey(
                name: "FK_GameProviderLanguage_GameProviders_GameProviderId",
                schema: "dbo",
                table: "GameProviderLanguage");

            migrationBuilder.DropForeignKey(
                name: "FK_GameProviderLanguage_Languages_LanguageCultureCode",
                schema: "dbo",
                table: "GameProviderLanguage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameProviderLanguage",
                schema: "dbo",
                table: "GameProviderLanguage");

            migrationBuilder.DropIndex(
                name: "IX_GameProviderLanguage_GameProviderId",
                schema: "dbo",
                table: "GameProviderLanguage");

            migrationBuilder.DropIndex(
                name: "IX_GameProviderLanguage_LanguageCultureCode",
                schema: "dbo",
                table: "GameProviderLanguage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameProviderCurrency",
                schema: "dbo",
                table: "GameProviderCurrency");

            migrationBuilder.DropIndex(
                name: "IX_GameProviderCurrency_GameProviderId",
                schema: "dbo",
                table: "GameProviderCurrency");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BrandCurrency",
                schema: "dbo",
                table: "BrandCurrency");

            migrationBuilder.DropColumn(
                name: "LanguageCultureCode",
                schema: "dbo",
                table: "GameProviderLanguage");

            migrationBuilder.RenameTable(
                name: "GameProviderLanguage",
                schema: "dbo",
                newName: "GameProviderLanguages",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "GameProviderCurrency",
                schema: "dbo",
                newName: "GameProviderCurrencies",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "BrandCurrency",
                schema: "dbo",
                newName: "BrandCurrencies",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_GameProviderCurrency_CurrencyCode",
                schema: "dbo",
                table: "GameProviderCurrencies",
                newName: "IX_GameProviderCurrencies_CurrencyCode");

            migrationBuilder.RenameIndex(
                name: "IX_BrandCurrency_CurrencyCode",
                schema: "dbo",
                table: "BrandCurrencies",
                newName: "IX_BrandCurrencies_CurrencyCode");

            migrationBuilder.RenameIndex(
                name: "IX_BrandCurrency_BrandId",
                schema: "dbo",
                table: "BrandCurrencies",
                newName: "IX_BrandCurrencies_BrandId");

            migrationBuilder.AlterColumn<string>(
                name: "CultureCode",
                schema: "dbo",
                table: "GameProviderLanguages",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                schema: "dbo",
                table: "GameProviderCurrencies",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BrandCurrencyCode",
                schema: "dbo",
                table: "BrandCurrencies",
                type: "nvarchar(16)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameProviderLanguages",
                schema: "dbo",
                table: "GameProviderLanguages",
                columns: new[] { "GameProviderId", "CultureCode" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameProviderCurrencies",
                schema: "dbo",
                table: "GameProviderCurrencies",
                columns: new[] { "GameProviderId", "CurrencyCode" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BrandCurrencies",
                schema: "dbo",
                table: "BrandCurrencies",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GameProviderLanguages_CultureCode",
                schema: "dbo",
                table: "GameProviderLanguages",
                column: "CultureCode");

            migrationBuilder.AddForeignKey(
                name: "FK_BrandCurrencies_Brands_BrandId",
                schema: "dbo",
                table: "BrandCurrencies",
                column: "BrandId",
                principalSchema: "dbo",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BrandCurrencies_Currencies_CurrencyCode",
                schema: "dbo",
                table: "BrandCurrencies",
                column: "CurrencyCode",
                principalSchema: "dbo",
                principalTable: "Currencies",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GameProviderCurrencies_Currencies_CurrencyCode",
                schema: "dbo",
                table: "GameProviderCurrencies",
                column: "CurrencyCode",
                principalSchema: "dbo",
                principalTable: "Currencies",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameProviderCurrencies_GameProviders_GameProviderId",
                schema: "dbo",
                table: "GameProviderCurrencies",
                column: "GameProviderId",
                principalSchema: "dbo",
                principalTable: "GameProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameProviderLanguages_Languages_CultureCode",
                schema: "dbo",
                table: "GameProviderLanguages",
                column: "CultureCode",
                principalSchema: "dbo",
                principalTable: "Languages",
                principalColumn: "CultureCode",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameProviderLanguages_GameProviders_GameProviderId",
                schema: "dbo",
                table: "GameProviderLanguages",
                column: "GameProviderId",
                principalSchema: "dbo",
                principalTable: "GameProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrandCurrencies_Brands_BrandId",
                schema: "dbo",
                table: "BrandCurrencies");

            migrationBuilder.DropForeignKey(
                name: "FK_BrandCurrencies_Currencies_CurrencyCode",
                schema: "dbo",
                table: "BrandCurrencies");

            migrationBuilder.DropForeignKey(
                name: "FK_GameProviderCurrencies_Currencies_CurrencyCode",
                schema: "dbo",
                table: "GameProviderCurrencies");

            migrationBuilder.DropForeignKey(
                name: "FK_GameProviderCurrencies_GameProviders_GameProviderId",
                schema: "dbo",
                table: "GameProviderCurrencies");

            migrationBuilder.DropForeignKey(
                name: "FK_GameProviderLanguages_Languages_CultureCode",
                schema: "dbo",
                table: "GameProviderLanguages");

            migrationBuilder.DropForeignKey(
                name: "FK_GameProviderLanguages_GameProviders_GameProviderId",
                schema: "dbo",
                table: "GameProviderLanguages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameProviderLanguages",
                schema: "dbo",
                table: "GameProviderLanguages");

            migrationBuilder.DropIndex(
                name: "IX_GameProviderLanguages_CultureCode",
                schema: "dbo",
                table: "GameProviderLanguages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameProviderCurrencies",
                schema: "dbo",
                table: "GameProviderCurrencies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BrandCurrencies",
                schema: "dbo",
                table: "BrandCurrencies");

            migrationBuilder.RenameTable(
                name: "GameProviderLanguages",
                schema: "dbo",
                newName: "GameProviderLanguage",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "GameProviderCurrencies",
                schema: "dbo",
                newName: "GameProviderCurrency",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "BrandCurrencies",
                schema: "dbo",
                newName: "BrandCurrency",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_GameProviderCurrencies_CurrencyCode",
                schema: "dbo",
                table: "GameProviderCurrency",
                newName: "IX_GameProviderCurrency_CurrencyCode");

            migrationBuilder.RenameIndex(
                name: "IX_BrandCurrencies_CurrencyCode",
                schema: "dbo",
                table: "BrandCurrency",
                newName: "IX_BrandCurrency_CurrencyCode");

            migrationBuilder.RenameIndex(
                name: "IX_BrandCurrencies_BrandId",
                schema: "dbo",
                table: "BrandCurrency",
                newName: "IX_BrandCurrency_BrandId");

            migrationBuilder.AlterColumn<string>(
                name: "CultureCode",
                schema: "dbo",
                table: "GameProviderLanguage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "LanguageCultureCode",
                schema: "dbo",
                table: "GameProviderLanguage",
                type: "nvarchar(6)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                schema: "dbo",
                table: "GameProviderCurrency",
                type: "nvarchar(10)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "BrandCurrencyCode",
                schema: "dbo",
                table: "BrandCurrency",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameProviderLanguage",
                schema: "dbo",
                table: "GameProviderLanguage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameProviderCurrency",
                schema: "dbo",
                table: "GameProviderCurrency",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BrandCurrency",
                schema: "dbo",
                table: "BrandCurrency",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GameProviderLanguage_GameProviderId",
                schema: "dbo",
                table: "GameProviderLanguage",
                column: "GameProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_GameProviderLanguage_LanguageCultureCode",
                schema: "dbo",
                table: "GameProviderLanguage",
                column: "LanguageCultureCode");

            migrationBuilder.CreateIndex(
                name: "IX_GameProviderCurrency_GameProviderId",
                schema: "dbo",
                table: "GameProviderCurrency",
                column: "GameProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_BrandCurrency_Brands_BrandId",
                schema: "dbo",
                table: "BrandCurrency",
                column: "BrandId",
                principalSchema: "dbo",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BrandCurrency_Currencies_CurrencyCode",
                schema: "dbo",
                table: "BrandCurrency",
                column: "CurrencyCode",
                principalSchema: "dbo",
                principalTable: "Currencies",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GameProviderCurrency_Currencies_CurrencyCode",
                schema: "dbo",
                table: "GameProviderCurrency",
                column: "CurrencyCode",
                principalSchema: "dbo",
                principalTable: "Currencies",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GameProviderCurrency_GameProviders_GameProviderId",
                schema: "dbo",
                table: "GameProviderCurrency",
                column: "GameProviderId",
                principalSchema: "dbo",
                principalTable: "GameProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameProviderLanguage_GameProviders_GameProviderId",
                schema: "dbo",
                table: "GameProviderLanguage",
                column: "GameProviderId",
                principalSchema: "dbo",
                principalTable: "GameProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameProviderLanguage_Languages_LanguageCultureCode",
                schema: "dbo",
                table: "GameProviderLanguage",
                column: "LanguageCultureCode",
                principalSchema: "dbo",
                principalTable: "Languages",
                principalColumn: "CultureCode",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
