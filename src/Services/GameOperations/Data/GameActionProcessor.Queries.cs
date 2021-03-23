using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Services.GameOperations.Models.Data;

namespace TGS.Services.GameOperations.Data
{
    internal sealed partial class GameActionProcessor
    {
        private static async ValueTask<RoundIdAndStatusData> GetRoundByExternalRoundIdAsync(ITgsRepository repository,
            Guid gameProviderConfigurationId, Guid walletId, string externalRoundId, int gameId) =>
            await repository.Rounds
                .Where(r => r.GameProviderConfigurationId == gameProviderConfigurationId &&
                    r.WalletId == walletId && r.GameId == gameId &&
                    r.ExternalRoundId == externalRoundId)
                .Select(r => new RoundIdAndStatusData
                {
                    RoundId = r.Id,
                    ExternalRoundId = r.ExternalRoundId,
                    Status = r.Status
                })
                .FirstOrDefaultAsync().ConfigureAwait(false);

        private static async ValueTask<SameRoundActionData[]> GetGameActionsInSameRoundAsync(ITgsRepository repository,
            Guid gameProviderId, string playerCode, string externalRoundId, int gameId)
        {
            const string sql = "bet.game_action_get_game_actions_in_same_round";
            var data = await repository.SqlQueryMultipleAsync<SameRoundActionData>(sql,
                parameters: new Dictionary<string, object>
                {
                    {":game_provider_id", gameProviderId},
                    {":player_code", playerCode},
                    {":external_round_id", externalRoundId},
                    {":game_id", gameId}
                }).ConfigureAwait(false);

            return data.ToArray();
        }

        private static async ValueTask<string> GetExternalRoundIdAsync(ITgsRepository repository,
            string externalTransactionId) =>
            await repository.GameActions
                .Where(ga => ga.ExternalTransactionId == externalTransactionId)
                .Include(ga => ga.Round)
                .Select(ga => ga.Round.ExternalRoundId)
                .FirstOrDefaultAsync().ConfigureAwait(false);

        private static async ValueTask<decimal> GetBalanceAsync(ITgsRepository repository, Guid walletId)
        {
            const string spName = "dbo.wallet_get_balance";
            return await repository.SqlQuerySingleAsync<decimal>(spName,
                parameters: new Dictionary<string, object> {{":wallet_id", walletId}}).ConfigureAwait(false);
        }

        private static async ValueTask<int?> GetGameIdInSameRoundAsync(ITgsRepository repository, Guid gameProviderId,
            string playerCode, string externalRoundId, string externalTransactionReferenceId) =>
            await repository.Rounds.Where(r =>
                    r.GameProviderId == gameProviderId &&
                    r.ExternalRoundId == externalRoundId &&
                    r.PlayerCode == playerCode &&
                    r.GameActions.Any(ga => ga.ExternalTransactionId == externalTransactionReferenceId))
                .Select(r => (int?)r.GameId).FirstOrDefaultAsync().ConfigureAwait(false);
    }
}