using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Migrations
{
    public partial class Initial
    {
        //InitAfterRun(migrationBuilder);
        public static void InitAfterRun(MigrationBuilder migrationBuilder)
        {
            #region Store Procedure

            var assembly = Assembly.GetExecutingAssembly();

            // TYPE
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.TypeSqlForamt, "IntValuesType"));

            // SP
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_GameProvider_GetGameProviderIdByCode"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_GameProvider_GetCurrenciesByInternalCode"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_GameProvider_GetCurrenciesByExternalCode"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Player_GetPlayerForCache"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Wallet_GetBalance"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Wallet_UpdateWalletBalance"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Licensee_GetLicenseeForCache"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt,
                    "usp_Licensee_GetLicenseeGameProviderConfigurationForCache"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_GameProvider_GetGameProviderForCache"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_GameProvider_GetGameProviderConfigurationForCache"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_GameProvider_GetGamesForCache"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_GameProvider_GetLanguagesByInternalCode"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_GameProvider_GetLanguagesByExternalCode"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_GameProvider_GetGameProviderBetLimitCode"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Player_GetPlayerLoginRecords"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Licensee_GetLicenseeGamesForCache"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Round_GetRoundByExternalRoundId"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Round_GetExternalRoundIdByExternalTransactionId"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_GameAction_GetGameActionsInSameRound"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Round_InsertRound"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Round_UpdateRound"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_GameAction_InsertGameAction"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Player_Deauthorize"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_GameProvider_GetSubGameIdMaps"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Licensee_GetLanguagesByInternalCode"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Licensee_GetLanguagesByExternalCode"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Licensee_GetCurrenciesByInternalCode"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Licensee_GetCurrenciesByExternalCode"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Licensee_GetLicenseeGameProviders"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlForamt, "usp_Localization_GetLocalizationByGameProviderId"));

            #endregion Store Procedure
        }
    }
}