using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
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
                parameters: new Dictionary<string, object> { { ":wallet_id", walletId } }).ConfigureAwait(false);
        }

        private static async ValueTask<CachedGame?> GetGameIdInSameRoundAsync(
            ITgsRepository repository,
            Guid gameProviderId,
            string externalGameId)
        {
            var game = await repository.Games.AsQueryable()
                .Where(x => x.GameProviderId == gameProviderId &&
                x.ExternalId == externalGameId
                ).FirstOrDefaultAsync().ConfigureAwait(false);
            if (game == null)
            {
                return null;
            }
            return new CachedGame()
            {
                Id = game.Id,
                Code = game.Code,
                Name = game.Name,
                Description = game.Description,
                Status = game.Status,
                ExternalId = game.ExternalId,
                SubGame = game.SubGame,
                AvailableToBrand = game.AvailableToBrand,
                RuleUrl = game.RuleUrl,
                EndpointPath = game.EndpointPath,
                Type = game.Type,
                PlatformType = game.PlatformType,
                HasTurnoverOnTieBet = game.HasTurnoverOnTieBet,
                AvailableCurrencies = game.AvailableCurrencies.IsNotNullOrEmpty()
                    ? game.AvailableCurrencies.SplitByComma()
                    : Array.Empty<string>()
            };
        }
    }
}