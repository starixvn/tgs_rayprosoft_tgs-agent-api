using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Bet.Migrations
{
    public partial class Initial
    {
        private static void InitAfterRun(MigrationBuilder migrationBuilder)
        {
            #region Store Procedure

            var assembly = Assembly.GetExecutingAssembly();

            // TYPE
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.TypeSqlFormat, "IntValuesType"));

            // SP
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlFormat, "usp_Round_GetRoundByExternalRoundId"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlFormat, "usp_Round_GetExternalRoundIdByExternalTransactionId"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlFormat, "usp_GameAction_GetGameActionsInSameRound"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlFormat, "usp_Round_InsertRound"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlFormat, "usp_Round_UpdateRound"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlFormat, "usp_GameAction_InsertGameAction"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlFormat, "usp_GameAction_GetValidGameActionExternalIds"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlFormat, "usp_GameAction_GetGameActionExternalIds"));
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.StoreProcedureSqlFormat, "usp_GameAction_GetGameActionsByExternalRoundId"));

            // View
            migrationBuilder.SqlResource(assembly,
                string.Format(Consts.ViewSqlFormat, "vw_ValidGameActions"));

            #endregion Store Procedure
        }
    }
}