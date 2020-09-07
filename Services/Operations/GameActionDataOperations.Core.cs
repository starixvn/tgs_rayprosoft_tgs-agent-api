using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;
using TGS.Infrastructures.Extensions;
using TGS.Services.Operations.Models;
using TGS.Services.Operations.Models.Data;

namespace TGS.Services.Operations
{
    public sealed partial class GameActionDataOperations : IGameActionDataOperations
    {
        private async Task<BetResult> ExecuteGameActionAsync(BetData data)
        {
            if (data?.Transactions == null || data.Transactions.Length == 0)
            {
                throw new InvalidOperationException("No transactions in the request");
            }

            var cachedPlayer = await _playerOperations.GetCachedPlayerAsync(data.PlayerCode);

            // TODO: validate if place bet cap amount is exceed

            var playerRelatedData = new PlayerRelatedData
            {
                Wallet = new WalletData { 
                    Id = cachedPlayer.DefaultWalletId,
                    CurrencyCode = cachedPlayer.DefaultWalletCurrencyCode,
                    ExternalCurrencyCode = cachedPlayer.ExternalCurrencyCode 
                },
                PlayerCode = data.PlayerCode,
                BrandId = cachedPlayer.BrandId,
                BrandCode = cachedPlayer.BrandCode,
                Status = cachedPlayer.Status,
                PlayerType = cachedPlayer.Type,
                PlayerUsername = cachedPlayer.Username,
                PlayerExternalId = cachedPlayer.ExternalId,
                ExternalCurrencyCode = cachedPlayer.ExternalCurrencyCode,
                //PlayerIp = data..Ip
            };

            var games = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId);

            var externalTransactionIds = data.Transactions.Select(t => t.ExternalTransactionId).JoinAsStringBySemicolon();
            var transactionResult = await _transactionManager.ExecuteTransactionAsync(
                _tgsRepoFactory,
                IsolationLevel.RepeatableRead,
                externalTransactionIds,
                execute: async (r, trans) => await ExecuteGameActionTransactionsAsync(r, data, playerRelatedData, games));

            return transactionResult;
        }

        private async Task<BetResult> ExecuteGameActionTransactionsAsync(ITgsRepository repo, BetData data, PlayerRelatedData player, CachedGame[] games)
        {
            var hasChanges = false;
            var transactionResults = new List<TransactionResult>();

            foreach (var transaction in data.Transactions)
            {
                var transactionSettings = _transactionSettingsGenerator.GetTransactionSettings(transaction.GameActionType);
                var hasWalletUpdate = transactionSettings.HasWalletUpdate(transaction);

                var result = await ExecuteGameActionTransactionAsync(new ExecuteGameActionTransactionState(transaction)
                { 
                    Repo = repo,
                    GameProviderId  = data.GameProviderId,
                    GameProviderConfigurationId = data.GameProviderConfigurationId,
                    BatchId = data.BatchId,
                    PlatformType = data.PlatformType ?? PlatformType.Desktop,
                    HasWalletUpdate = hasWalletUpdate,
                    MustHaveExistingRound = transactionSettings.MustHaveExistingRound,
                    MustHaveReferencedTransaction = transactionSettings.MustHaveReferencedTransaction,
                    ValidateAmount =  transactionSettings.ValidateAmount(transaction),
                    PrepareAmount = transactionSettings.GetTransactionAmount,
                    PlayerData = player,
                    GameProviderGames = games,
                    ForceGetPreviousActions = data.ForceGetPreviousActions,
                    //OppositeBetPatterns = oppositeBetPatterns,
                    FreeBetAsSingleRound = data.FreeBetAsSingleRound                                                           
                });
            }

            return new BetResult { };
        }

        private Task<ExecuteGameActionTransactionResult> ExecuteGameActionTransactionAsync(ExecuteGameActionTransactionState s)
        {
            s.ValidateAmount(s.Amount);

            return null;
        }
    }
}