using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Services.Caches;
using TGS.Services.GameOperations.Models.Data;

namespace TGS.Services.GameOperations.Data
{
    internal sealed partial class GameActionProcessor
    {
        private static async ValueTask<decimal> GetBalanceAsync(ITgsRepository repository, Guid walletId)
        {
            const string spName = "dbo.wallet_get_balance";
            return await repository.SqlQuerySingleAsync<decimal>(spName,
                parameters: new Dictionary<string, object> { { ":wallet_id", walletId } }).ConfigureAwait(false);
        }

        private static async ValueTask<string> GetExternalRoundIdAsync(ITgsRepository repository,
            string externalTransactionId) =>
            await repository.GameActions
                .Where(ga => ga.ExternalTransactionId == externalTransactionId)
                .Include(ga => ga.Round)
                .Select(ga => ga.Round.ExternalRoundId)
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

        private static async ValueTask<CachedGame?> GetGameIdInSameRoundAsync(
            ITgsRepository repository,
            Guid gameProviderId,
            string playerCode,
            string externalRoundId,
            string externalTransactionReferenceId)
        {
            var gameId = await (
                from round in repository.Rounds.AsQueryable()
                join gameActions in repository.GameActions.AsQueryable()
                on round.Id equals gameActions.RoundId
                where
                round.GameProviderId == gameProviderId &&
                round.ExternalRoundId == externalRoundId &&
                round.PlayerCode == playerCode &&
                gameActions.ExternalTransactionId == externalTransactionReferenceId
                select (int?)round.GameId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (!gameId.HasValue)
            {
                return null;
            }

            var game = await repository.Games.AsQueryable()
                .Where(x => x.Id == gameId.Value)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

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

        private static async ValueTask<RoundIdAndStatusData> GetRoundByExternalRoundIdAsync(
            ITgsRepository repository,
            string externalRoundId,
            Guid gameProviderId,
            int gameId,
            Guid walletId)
            => await repository.Rounds.AsQueryable()
            .Where(r =>
                r.ExternalRoundId == externalRoundId &&
                r.GameProviderId == gameProviderId &&
                r.GameId == gameId &&
                r.WalletId == walletId
            )
            .Select(r => new RoundIdAndStatusData
            {
                RoundId = r.Id,
                ExternalRoundId = r.ExternalRoundId,
                Status = r.Status
            })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }
}
