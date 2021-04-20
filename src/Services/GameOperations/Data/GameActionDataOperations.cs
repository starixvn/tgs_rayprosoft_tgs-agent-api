using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Interfaces;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations.Models.Data;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Services.GameOperations.Data
{
    public interface IGameActionDataOperations
    {
        ValueTask<BetResult> LockAmountAsync(BetData data);

        ValueTask<BetResult> PlaceBetAsync(BetData data);

        ValueTask<BetResult> SettleBetAsync(BetData data);

        ValueTask<BetResult> CancelTransactionAsync(BetData data);

        ValueTask<BetResult> PlaceAndSettleBetAsync(BetData data);

        ValueTask<BatchBetResult> SettleBatchAsync(BatchBetData data);

        ValueTask<BatchBetResult> CancelBatchAsync(BatchBetData data);
    }

    internal sealed class GameActionDataOperations : IGameActionDataOperations
    {
        private readonly IBrandOperations _brandOperations;
        private readonly Func<string, IErrorMapper> _errorMapperFactory;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameActionDataOperations _interface;
        private readonly IJsonSerializationProvider _json;
        private readonly ITgsLog _log;
        private readonly IPlayerOperations _playerOperations;
        private readonly IGameActionProcessor _processor;
        private readonly Func<ITgsRepository> _repoFactory;
        private readonly IStatusValidator _statusValidator;
        private readonly ITransactionManager _transactionManager;

        public GameActionDataOperations(
            Func<ITgsRepository> repoFactory,
            ITransactionManager transactionManager,
            IStatusValidator statusValidator,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IGameActionProcessor processor,
            ITgsLog log,
            IJsonSerializationProvider json,
            Func<string, IErrorMapper> errorMapperFactory)
        {
            _repoFactory = repoFactory;
            _transactionManager = transactionManager;
            _statusValidator = statusValidator;
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _processor = processor;
            _json = json;
            _log = log;
            _errorMapperFactory = errorMapperFactory;

            _interface = this;
        }

        ValueTask<BetResult> IGameActionDataOperations.LockAmountAsync(BetData data) =>
            ExecuteGameActionAsync(data);

        ValueTask<BetResult> IGameActionDataOperations.PlaceBetAsync(BetData data)
        {
            if (data.Transactions.Any(t => t.GameActionType.IsNotAllOf(UnlockAmount, PlaceBet)))
            {
                throw new InvalidOperationException(
                    "Only allow game action type UnlockAmount / PlaceBet in place bet action");
            }

            return ExecuteGameActionAsync(data);
        }

        ValueTask<BetResult> IGameActionDataOperations.SettleBetAsync(BetData data) =>
            ExecuteGameActionAsync(data);

        ValueTask<BetResult> IGameActionDataOperations.CancelTransactionAsync(BetData data) =>
            ExecuteGameActionAsync(data);

        ValueTask<BetResult> IGameActionDataOperations.PlaceAndSettleBetAsync(BetData data) =>
            ExecuteGameActionAsync(data);

        ValueTask<BatchBetResult> IGameActionDataOperations.SettleBatchAsync(BatchBetData data) =>
            ExecuteBatchAsync(data, betData => _interface.SettleBetAsync(betData));

        ValueTask<BatchBetResult> IGameActionDataOperations.CancelBatchAsync(BatchBetData data) =>
            ExecuteBatchAsync(data, betData => _interface.CancelTransactionAsync(betData));

        private async ValueTask<BetResult> ExecuteGameActionAsync(BetData data)
        {
            if (data?.Transactions == null || data.Transactions.Length == 0)
            {
                throw new InvalidOperationException("No transactions in the request");
            }

            var player = await _playerOperations.GetCachedPlayerAsync(data.PlayerCode).ConfigureAwait(false);
            ThrowIfPlayerDisableToPlaceBet(data.Transactions, player.Status);

            // TODO: validate if place bet cap amount is exceed

            var brand = await _brandOperations.GetCachedBrandAsync(player.BrandId).ConfigureAwait(false);

            PlayerRelatedData playerRelatedData = new()
            {
                Wallet =
                    new()
                    {
                        Id = player.DefaultWalletId,
                        CurrencyCode = player.DefaultWalletCurrencyCode,
                        ExternalCurrencyCode = player.ExternalCurrencyCode
                    },
                PlayerCode = data.PlayerCode,
                ResellerId = brand.ResellerId,
                ResellerCode = brand.ResellerCode,
                LicenseeId = brand.LicenseeId,
                LicenseeCode = brand.LicenseeCode,
                BrandId = player.BrandId,
                BrandCode = brand.Code,
                BrandName = brand.Name,
                Status = player.Status,
                PlayerType = player.Type,
                PlayerUsername = player.Username,
                ExternalCurrencyCode = player.ExternalCurrencyCode,
                BrandTimeZoneId = brand.TimeZoneId,
                PlayerIp = string.Empty
            };

            var gameProvider =
                await _gameProviderOperations.GetCachedGameProviderAsync(data.GameProviderId).ConfigureAwait(false);
            var games = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId).ConfigureAwait(false);

            var externalTransactionIds =
                data.Transactions.Select(t => t.ExternalTransactionId).JoinAsStringBySemicolon();
            var transactionResult = await _transactionManager.ExecuteTransactionAsync(
                    _repoFactory,
                    IsolationLevel.RepeatableRead,
                    externalTransactionIds,
                    async r => await _processor.ExecuteAsync(new()
                    {
                        Repository = r,
                        BetData = data,
                        Player = playerRelatedData,
                        GameProvider = gameProvider,
                        Games = games
                    }), autoDetectChangesEnabled: false)
                .ConfigureAwait(false);

            return transactionResult;
        }

        private void ThrowIfPlayerDisableToPlaceBet(TransactionData[] transactions, PlayerStatus playerStatus)
        {
            if (transactions.Any(t => t.GameActionType.IsOneOf(LockAmount, PlaceBet)))
            {
                _statusValidator.ThrowIfPlayerDisabledToPlaceBet(playerStatus);
            }
        }

        #region Batch Actions

        private async ValueTask<BatchBetResult> ExecuteBatchAsync(BatchBetData data,
            Func<BetData, ValueTask<BetResult>> execute)
        {
            await using var repo = _repoFactory();
            repo.ChangeTracker.AutoDetectChangesEnabled = false;

            var gameProviderId = data.GameProviderId;
            var gameProviderCode = data.GameProviderCode;

            var batch = await CreateBatchAsync(repo, gameProviderId, data.ExternalBatchId);

            AssignIndexPerPlayer(data.Transactions);

            var degreeOfParallelism = Environment.ProcessorCount;
            List<BatchBetPartitionState> states = new();

            // group transactions by index per player in order to avoid transaction lock conflict
            // so first we execute all transaction with index 0 for all players,
            // then index 1 for all players with more than 1 transaction
            // then index 2 for all players with more than 2 transaction etc.

            foreach (var group in data.Transactions.GroupBy(t => t.IndexPerPlayer))
            {
                var segments = group.ToArraySegments(degreeOfParallelism * 12);

                foreach (var segment in segments)
                {
                    var subStates = await segment.EachAsync(degreeOfParallelism, CancellationToken.None,
                        () => new BatchBetPartitionState(batch.Id),
                        async (t, state, _) =>
                        {
                            var errorMapper = _errorMapperFactory(data.ErrorMapperKey);
                            await ExecuteSegmentTransactionsAsync(
                                gameProviderId, gameProviderCode,
                                data.AutoCloseRound, data.HasCurrencyFromGameProvider,
                                t, state, execute, errorMapper);
                        }).ConfigureAwait(false);

                    states.AddRange(subStates);
                }
            }

            var errors = states.SelectMany(s => s.Errors).ToArray();

            Dictionary<string, decimal> playerBalances = new();
            foreach (var (playerCode, balance) in states.SelectMany(s => s.PlayerBalances))
            {
                playerBalances[playerCode] = balance;
            }

            var result = new BatchBetResult
            {
                Error = TgsApiError.None,
                ErrorMessage = null,
                ExternalBatchId = data.ExternalBatchId,
                IsDuplicate = false,
                BatchTime = DateTimeOffset.Now,
                TransactionsCount = states.Sum(s => s.Successes),
                Errors = errors,
                PlayerBalances = playerBalances
            };

            await SaveBatchAsync(batch.Id, repo, result).ConfigureAwait(false);

            return result;
        }

        private async Task ExecuteSegmentTransactionsAsync(Guid gameProviderId,
            string gameProviderCode, bool autoCloseRound,
            bool hasCurrencyFromGameProvider,
            BatchTransactionData transaction,
            BatchBetPartitionState state, Func<BetData, ValueTask<BetResult>> execute,
            IErrorMapper errorMapper)
        {
            try
            {
                var betData = MapBetData(state.BatchId, gameProviderId, gameProviderCode, autoCloseRound,
                    hasCurrencyFromGameProvider, transaction);

                var betResult = await execute(betData).ConfigureAwait(false);

                if (betResult.Error == TgsApiError.None)
                {
                    ++state.Successes;
                    state.PlayerBalances[transaction.PlayerCode] = betResult.Balance;
                }
                else
                {
                    var firstTran = betResult.Transactions.FirstOrDefault();

                    state.Errors.Add(new()
                    {
                        Error = (int)betResult.Error,
                        ErrorMessage = betResult.Error.ToString(),
                        PlayerCode = transaction.PlayerCode,
                        ExternalTransactionId = transaction.ExternalTransactionId,
                        InternalTransactionId = firstTran?.InternalTransactionId,
                        IsDuplicate = firstTran?.IsDuplicate ?? false
                    });
                }
            }
            catch (Exception ex)
            {
                _log.LogError("ErrorInBatch", ex);

                state.Errors.Add(new()
                {
                    Error = errorMapper.GetApiErrorByException(ex),
                    ErrorMessage = ex.Message,
                    PlayerCode = transaction.PlayerCode,
                    ExternalTransactionId = transaction.ExternalTransactionId,
                    InternalTransactionId = null,
                    IsDuplicate = false
                });
            }
        }

        private static async ValueTask<GameActionBatch> CreateBatchAsync(ITgsRepository repo,
            Guid gameProviderId, string externalBatchId)
        {
            var batch = await repo.GameActionBatches.FirstOrDefaultAsync(b =>
                b.GameProviderId == gameProviderId && b.ExternalId == externalBatchId).ConfigureAwait(false);

            if (batch != null)
            {
                externalBatchId = $"{externalBatchId}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            }

            await repo.GameActionBatches.AddAsync(batch = new()
            {
                Id = GuidExtensions.NewSequentialGuid(),
                GameProviderId = gameProviderId,
                ExternalId = externalBatchId,
                CreatedOn = DateTimeOffset.UtcNow,
                Error = TgsApiError.None,
                TransactionsCount = 0
            }).ConfigureAwait(false);

            await repo.SaveChangesAsync().ConfigureAwait(false);

            return batch;
        }

        /*
         Given player transaction (letters signify player code)
         A, B, A, C, A, D, D, E
         The result of this is as follow:
         PLAYER => INDEX of TRANSACTION
         A => 0
         A => 1
         A => 2 (player A has 3 transaction that are being assigned indices 0,1,2)
         B => 0
         C => 0
         D => 0
         D => 1 (player D has 2 transaction that are being assigned indices 0,1)
         E => 0
         */
        private static void AssignIndexPerPlayer(BatchTransactionData[] transactions)
        {
            Dictionary<string, int> dict = new();
            foreach (var btd in transactions)
            {
                var playerCode = btd.PlayerCode;

                if (!dict.TryGetValue(playerCode, out var val))
                {
                    val = -1;
                }

                val += 1;
                btd.IndexPerPlayer = val;
                dict[playerCode] = val;
            }
        }

        private static BetData MapBetData(Guid batchId, Guid gameProviderId, string gameProviderCode,
            bool autoCloseRound, bool hasCurrencyFromGameProvider, BatchTransactionData transaction) =>
            new()
            {
                PlayerCode = transaction.PlayerCode,
                CurrencyCode = transaction.CurrencyCode,
                BatchId = batchId,
                GameProviderId = gameProviderId,
                GameProviderCode = gameProviderCode,
                GameProviderConfigurationId = transaction.GameProviderConfigurationId,
                HasCurrencyFromGameProvider = hasCurrencyFromGameProvider,
                AutoCloseRound = autoCloseRound,
                Transactions = new TransactionData[]
                {
                    new()
                    {
                        Amount = transaction.Amount,
                        ExternalGameId = transaction.ExternalGameId,
                        ExternalBetId = transaction.ExternalBetId,
                        ExternalRoundId = transaction.ExternalRoundId,
                        ExternalTransactionId = transaction.ExternalTransactionId,
                        ExternalTransactionReferenceId = transaction.ExternalTransactionReferenceId,
                        BetType = transaction.BetType,
                        ExtraData = transaction.Description,
                        RoundDetail = transaction.RoundDetail,
                        RoundClosed = transaction.RoundClosed,
                        GameActionType = transaction.GameActionType,
                        TimeStamp = transaction.Timestamp
                    }
                }
            };

        private async Task SaveBatchAsync(Guid batchId, ITgsRepository repo, BatchBetResult result)
        {
            var batch = await repo.GameActionBatches.FindAsync(batchId).ConfigureAwait(false);

            if (result.Error != TgsApiError.None)
            {
                batch.Error = result.Error;
            }

            if (result.ErrorMessage != null)
            {
                batch.ErrorMessage = result.ErrorMessage;
            }

            if (result.TransactionsCount > 0)
            {
                batch.TransactionsCount = result.TransactionsCount;
            }

            if (result.Errors.Length > 0)
            {
                batch.ErrorsJson = _json.SerializeToString(result.Errors);
            }

            repo.GameActionBatches.Update(batch);
            await repo.SaveChangesAsync().ConfigureAwait(false);
        }

        #endregion
    }
}