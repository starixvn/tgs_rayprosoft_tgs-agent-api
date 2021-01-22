using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.CustomQueryParameters;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Services.GameOperations.Models.Data;

namespace TGS.Services.GameOperations.Data
{
    internal sealed partial class GameActionProcessor
    {
        private static Task<RoundIdAndStatusData> GetRoundByExternalRoundIdAsync(ITgsRepository repository,
            string playerCode, Guid gameProviderConfigurationId, Guid walletId, string externalRoundId)
        {
            return repository.Rounds
                .Where(r => r.GameProviderConfigurationId == gameProviderConfigurationId &&
                    r.PlayerCode == playerCode &&
                    r.WalletId == walletId &&
                    r.ExternalRoundId == externalRoundId)
                .Select(r => new RoundIdAndStatusData
                {
                    RoundId = r.Id,
                    ExternalRoundId = r.ExternalRoundId,
                    Status = r.Status
                })
                .FirstOrDefaultAsync();
        }

        private static async Task<SameRoundActionData[]> GetGameActionsInSameRoundAsync(ITgsRepository repository,
            string playerCode, Guid gameProviderConfigurationId, Guid walletId, string externalRoundId)
        {
            const string sql = "bet.game_action_get_game_actions_in_same_round";
            var data = await repository.SqlQueryMultipleAsync<SameRoundActionData>(sql,
                parameters: new Dictionary<string, object>
                {
                    {":game_provider_configuration_id", gameProviderConfigurationId},
                    {":player_code", playerCode},
                    {":wallet_id", walletId},
                    {":external_round_id", externalRoundId}
                });

            return data.ToArray();
        }

        private static Task<string> GetExternalRoundIdAsync(ITgsRepository repository, string externalTransactionId)
        {
            return repository.GameActions
                .Where(ga => ga.ExternalTransactionId == externalTransactionId)
                .Include(ga => ga.Round)
                .Select(ga => ga.Round.ExternalRoundId)
                .FirstOrDefaultAsync();
        }

        private static Task<decimal> GetBalanceAsync(ITgsRepository repository, Guid walletId)
        {
            const string spName = "dbo.wallet_get_balance";
            return repository.SqlQuerySingleAsync<decimal>(spName, parameters: new Dictionary<string, object>
            {
                {":wallet_id", walletId}
            });
        }
    }
}