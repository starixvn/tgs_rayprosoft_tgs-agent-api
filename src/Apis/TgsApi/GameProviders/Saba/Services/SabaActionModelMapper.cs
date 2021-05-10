using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Apis.TgsApi.GameProviders.Saba.Data;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Saba.Enums;
using TGS.Shared.Game.Models.Saba.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Saba.Services
{
    public interface ISabaActionModelMapper
    {
        BetData Request(string playerCode, GameProviderConfigurationData data, PlaceBetRequest request);

        BetData Request(string playerCode, GameProviderConfigurationData data, PlaceBetParlayRequest request);

        BetData Request(string playerCode, GameProviderConfigurationData data, ConfirmBetRequest request,
            decimal unlockAmount);

        BetData Request(string playerCode, GameProviderConfigurationData data, ConfirmBetParlayRequest request,
            IReadOnlyDictionary<string, decimal> unlockAmounts);

        BetData Request(string playerCode, GameProviderConfigurationData data, CancelBetRequest request);

        BatchBetData SettleBatchRequest(Guid gameProviderId, bool autoCloseRound,
            Dictionary<string, Guid> playerConfigurationIds, Dictionary<string, string> externalRoundIds,
            SettleTransaction[] transactions);

        BatchBetData VoidRoundRequest(Guid gameProviderId, bool autoCloseRound,
            Dictionary<string, Guid> playerConfigurationIds,
            Dictionary<string, List<GameActionExternalIds>> playerGameActionExternalIds);

        BatchBetData UnsettleBatchRequest(Guid gameProviderId,
            bool autoCloseRound, Dictionary<string, Guid> playerConfigurationIds,
            Dictionary<string, string> externalRoundIds, TransactionInfo[] transactions);

        BatchBetData ResettleBatchRequest(Guid gameProviderId, bool autoCloseRound,
            Dictionary<string, Guid> playerConfigurationIds, Dictionary<string, string> externalRoundIds,
            SettleTransaction[] transactions);

        GetBalanceResponse Response(string playerCode, GetBalanceResult result);

        PlaceBetResponse Response(string playerCode, PlaceBetRequest.Data body, BetResult result);

        PlaceBetParlayResponse Response(string playerCode, PlaceBetParlayRequest.Data body, BetResult result);

        ConfirmBetResponse Response(string playerCode, ConfirmBetRequest.Data body, BetResult result);

        ConfirmBetParlayResponse Response(string playerCode, ConfirmBetParlayRequest.Data body, BetResult result);

        CancelBetResponse Response(string playerCode, CancelBetRequest.Data body, BetResult result);

        SettleBetResponse Response(SettleBetRequest request, BatchBetResult[] results);

        UnsettleBetResponse Response(UnsettleBetRequest request, BatchBetResult result);

        ResettleBetResponse Response(ResettleBetRequest request, BatchBetResult[] results);
    }

    public sealed class SabaActionModelMapper : ISabaActionModelMapper
    {
        private static readonly string GameProviderCode = Consts.GameProvider.Saba;
        private static readonly TimeSpan TimeSpan = TimeSpan.FromHours(-4);

        private readonly string _externalGameId = SabaConsts.ExternalGameId;

        BetData ISabaActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            PlaceBetRequest request)
        {
            var body = request.Body;
            var externalRoundId = body.ReferenceId.ToString();

            var transaction = new TransactionData
            {
                ExternalGameId = _externalGameId,
                ExternalBetId = string.Empty,
                ExternalRoundId = externalRoundId,
                ExternalTransactionId = externalRoundId,
                ExternalGameActionGroupId = externalRoundId,
                Amount = body.ActualAmount,
                TimeStamp = GetTimestamp(body.BetTime),
                GameActionType = GameActionType.LockAmount
            };

            return GetBetData(playerCode, body.CurrencyCode, data, new[] {transaction});
        }

        BetData ISabaActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            PlaceBetParlayRequest request)
        {
            var body = request.Body;

            var transactions = body.Transactions.Select(t =>
            {
                var externalRoundId = t.ReferenceId.ToString();

                return new TransactionData
                {
                    ExternalGameId = _externalGameId,
                    ExternalBetId = string.Empty,
                    ExternalRoundId = externalRoundId,
                    ExternalTransactionId = externalRoundId,
                    ExternalGameActionGroupId = externalRoundId,
                    Amount = t.BetAmount,
                    TimeStamp = GetTimestamp(body.BetTime),
                    GameActionType = GameActionType.LockAmount
                };
            }).ToArray();

            return GetBetData(playerCode, body.CurrencyCode, data, transactions);
        }

        BetData ISabaActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            ConfirmBetRequest request, decimal unlockAmount)
        {
            var body = request.Body;
            var confirmBet = body.Transactions[0];

            var externalTransactionId = confirmBet.ExternalTransactionId.ToString();
            var externalRoundId = confirmBet.ExternalRoundId.ToString();

            var transactions = new TransactionData[]
            {
                new()
                {
                    ExternalGameId = _externalGameId,
                    ExternalBetId = string.Empty,
                    ExternalRoundId = externalRoundId,
                    ExternalTransactionId = $"{externalRoundId}_unlock",
                    ExternalTransactionReferenceId = externalRoundId,
                    ExternalGameActionGroupId = externalRoundId,
                    Amount = unlockAmount,
                    TimeStamp = GetTimestamp(body.UpdateTime),
                    GameActionType = GameActionType.UnlockAmount
                },
                new()
                {
                    ExternalGameId = _externalGameId,
                    ExternalBetId = externalTransactionId,
                    ExternalRoundId = externalRoundId,
                    ExternalTransactionId = externalTransactionId,
                    ExternalGameActionGroupId = externalRoundId,
                    Amount = confirmBet.ActualAmount,
                    TimeStamp = GetTimestamp(body.UpdateTime),
                    GameActionType = GameActionType.PlaceBet
                }
            };

            return GetBetData(playerCode, null, data, transactions);
        }

        BetData ISabaActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            ConfirmBetParlayRequest request, IReadOnlyDictionary<string, decimal> unlockAmounts)
        {
            var body = request.Body;

            var transactions = body.Transactions.SelectMany(t =>
            {
                var externalTransactionId = t.ExternalTransactionId.ToString();
                var externalRoundId = t.ExternalRoundId.ToString();

                var unlockAmount = unlockAmounts[externalRoundId];

                return new TransactionData[]
                {
                    new()
                    {
                        ExternalGameId = _externalGameId,
                        ExternalBetId = string.Empty,
                        ExternalRoundId = externalRoundId,
                        ExternalTransactionId = $"{externalRoundId}_unlock",
                        ExternalTransactionReferenceId = externalRoundId,
                        ExternalGameActionGroupId = externalRoundId,
                        Amount = unlockAmount,
                        TimeStamp = GetTimestamp(body.UpdateTime),
                        GameActionType = GameActionType.UnlockAmount
                    },
                    new()
                    {
                        ExternalGameId = _externalGameId,
                        ExternalBetId = externalTransactionId,
                        ExternalRoundId = externalRoundId,
                        ExternalTransactionId = externalTransactionId,
                        ExternalGameActionGroupId = externalRoundId,
                        Amount = t.ActualAmount,
                        TimeStamp = GetTimestamp(body.UpdateTime),
                        GameActionType = GameActionType.PlaceBet
                    }
                };
            }).ToArray();

            return GetBetData(playerCode, null, data, transactions);
        }

        BetData ISabaActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            CancelBetRequest request)
        {
            var body = request.Body;
            var transactions = body.Transactions.Select(t =>
            {
                var externalRoundId = t.ExternalRoundId.ToString();

                return new TransactionData
                {
                    ExternalGameId = _externalGameId,
                    ExternalBetId = string.Empty,
                    ExternalRoundId = externalRoundId,
                    ExternalTransactionId = $"{externalRoundId}_lock_cancel",
                    ExternalTransactionReferenceId = externalRoundId,
                    ExternalGameActionGroupId = externalRoundId,
                    TimeStamp = GetTimestamp(body.UpdateTime),
                    GameActionType = GameActionType.CancelTransaction
                };
            }).ToArray();

            return GetBetData(playerCode, null, data, transactions);
        }

        BatchBetData ISabaActionModelMapper.SettleBatchRequest(Guid gameProviderId, bool autoCloseRound,
            Dictionary<string, Guid> playerConfigurationIds, Dictionary<string, string> externalRoundIds,
            SettleTransaction[] transactions)
        {
            var batchTransactions = transactions.Select(t =>
            {
                var playerCode = t.PlayerCode;
                var externalTransactionId = t.ExternalTransactionId;
                var externalRoundId = externalRoundIds[externalTransactionId];

                return new BatchTransactionData
                {
                    PlayerCode = playerCode,
                    GameProviderConfigurationId = playerConfigurationIds[playerCode],
                    Amount = t.Amount,
                    ExternalBetId = externalTransactionId,
                    ExternalRoundId = externalRoundId,
                    ExternalTransactionId = $"{externalTransactionId}_settle",
                    ExternalTransactionReferenceId = externalTransactionId,
                    ExternalGameId = _externalGameId,
                    GameActionType = t.GameActionType,
                    Timestamp = GetTimestamp(t.UpdateTime)
                };
            }).ToArray();

            return GetBatchBetData(gameProviderId, autoCloseRound, batchTransactions);
        }

        BatchBetData ISabaActionModelMapper.VoidRoundRequest(Guid gameProviderId, bool autoCloseRound,
            Dictionary<string, Guid> playerConfigurationIds,
            Dictionary<string, List<GameActionExternalIds>> playerGameActionExternalIds)
        {
            var transactions = playerGameActionExternalIds.SelectMany(kvp =>
            {
                var playerCode = kvp.Key;
                var gameActionExternalIds = kvp.Value;

                return gameActionExternalIds.Select(a => new BatchTransactionData
                {
                    PlayerCode = playerCode,
                    GameProviderConfigurationId = playerConfigurationIds[playerCode],
                    ExternalBetId = a.ExternalBetId,
                    ExternalRoundId = a.ExternalRoundId,
                    ExternalTransactionId = $"{a.ExternalTransactionId}_cancel",
                    ExternalTransactionReferenceId = a.ExternalTransactionId,
                    ExternalGameId = _externalGameId,
                    GameActionType = GameActionType.CancelTransaction,
                    Timestamp = DateTimeOffset.Now
                });
            }).ToArray();

            return GetBatchBetData(gameProviderId, autoCloseRound, transactions);
        }

        BatchBetData ISabaActionModelMapper.UnsettleBatchRequest(Guid gameProviderId,
            bool autoCloseRound,
            Dictionary<string, Guid> playerConfigurationIds, Dictionary<string, string> externalRoundIds,
            TransactionInfo[] transactions)
        {
            var batchTransactions = transactions.Select(t =>
            {
                var playerCode = t.PlayerCode;
                var gameProviderConfigurationId = playerConfigurationIds[playerCode];
                var externalTransactionId = t.ExternalTransactionId.ToString();
                var externalRoundId = externalRoundIds[externalTransactionId];

                return new BatchTransactionData
                {
                    PlayerCode = playerCode,
                    GameProviderConfigurationId = gameProviderConfigurationId,
                    ExternalBetId = externalTransactionId,
                    ExternalRoundId = externalRoundId,
                    ExternalTransactionId = $"{externalTransactionId}_settle_canel",
                    ExternalTransactionReferenceId = $"{externalTransactionId}_settle",
                    ExternalGameId = _externalGameId,
                    GameActionType = GameActionType.CancelTransaction,
                    Timestamp = GetTimestamp(t.UpdateTime),
                    RoundClosed = false
                };
            }).ToArray();

            return GetBatchBetData(gameProviderId, autoCloseRound, batchTransactions);
        }

        BatchBetData ISabaActionModelMapper.ResettleBatchRequest(Guid gameProviderId, bool autoCloseRound,
            Dictionary<string, Guid> playerConfigurationIds, Dictionary<string, string> externalRoundIds,
            SettleTransaction[] transactions)
        {
            var batchTransactions = transactions.Select(t =>
            {
                var playerCode = t.PlayerCode;
                var externalTransactionId = t.ExternalTransactionId;
                var externalRoundId = externalRoundIds[externalTransactionId];

                return new BatchTransactionData
                {
                    PlayerCode = playerCode,
                    GameProviderConfigurationId = playerConfigurationIds[playerCode],
                    Amount = t.Amount,
                    ExternalBetId = externalTransactionId,
                    ExternalRoundId = externalRoundId,
                    ExternalTransactionId = $"{externalTransactionId}_resettle",
                    ExternalTransactionReferenceId = externalTransactionId,
                    ExternalGameId = _externalGameId,
                    GameActionType = t.GameActionType,
                    Timestamp = GetTimestamp(t.UpdateTime)
                };
            }).ToArray();

            return GetBatchBetData(gameProviderId, autoCloseRound, batchTransactions);
        }

        GetBalanceResponse ISabaActionModelMapper.Response(string playerCode, GetBalanceResult result) =>
            new()
            {
                Status = ((int)GameApiError.Success).ToString(),
                PlayerCode = playerCode,
                Balance = result.Balance,
                Timestamp = DateTimeOffset.Now.ToOffset(TimeSpan).ToIso8601String(true)
            };

        PlaceBetResponse ISabaActionModelMapper.
            Response(string playerCode, PlaceBetRequest.Data body, BetResult result) =>
            new()
            {
                Status = ((int)GameApiError.Success).ToString(),
                ExternalRoundId = body.ReferenceId,
                ReferenceId = body.ReferenceId
            };

        PlaceBetParlayResponse ISabaActionModelMapper.Response(string playerCode, PlaceBetParlayRequest.Data body,
            BetResult result) =>
            new()
            {
                Status = ((int)GameApiError.Success).ToString(),
                Transactions = body.Transactions.Select(t => new PlaceBetParlayResponse.Transaction
                {
                    ExternalRoundId = t.ReferenceId,
                    ReferenceId = t.ReferenceId
                }).ToArray()
            };

        ConfirmBetResponse ISabaActionModelMapper.
            Response(string playerCode, ConfirmBetRequest.Data body, BetResult result) =>
            new()
            {
                Status = ((int)GameApiError.Success).ToString(),
                Balance = result.Balance
            };

        ConfirmBetParlayResponse ISabaActionModelMapper.Response(string playerCode, ConfirmBetParlayRequest.Data body,
            BetResult result) =>
            new()
            {
                Status = ((int)GameApiError.Success).ToString(),
                Balance = result.Balance
            };

        CancelBetResponse ISabaActionModelMapper.Response(string playerCode, CancelBetRequest.Data body,
            BetResult result) =>
            new()
            {
                Status = ((int)GameApiError.Success).ToString(),
                Balance = result.Balance
            };

        SettleBetResponse ISabaActionModelMapper.Response(SettleBetRequest request, BatchBetResult[] results)
        {
            var errors = new List<string>();

            foreach (var r in results)
            {
                if (r.Error != TgsApiError.None)
                {
                    errors.Add(r.ErrorMessage);
                }

                if (r.Errors.Length <= 0)
                {
                    continue;
                }

                var messages = r.Errors.Select(e => e.ErrorMessage).ToArray();
                errors.AddRange(messages);
            }

            return errors.Count == 0
                ? new() {Status = ((int)GameApiError.Success).ToString()}
                : new SettleBetResponse
                {
                    Status = ((int)GameApiError.UnableProcessed).ToString(),
                    Message = errors.JoinAsString(";")
                };
        }

        UnsettleBetResponse ISabaActionModelMapper.Response(UnsettleBetRequest request, BatchBetResult result)
        {
            var errors = new List<string>();

            if (result.Error != TgsApiError.None)
            {
                errors.Add(result.ErrorMessage);
            }

            var messages = result.Errors.Select(e => e.ErrorMessage).ToArray();
            errors.AddRange(messages);

            return errors.Count == 0
                ? new() {Status = ((int)GameApiError.Success).ToString()}
                : new UnsettleBetResponse
                {
                    Status = ((int)GameApiError.UnableProcessed).ToString(),
                    Message = errors.JoinAsString(";")
                };
        }

        ResettleBetResponse ISabaActionModelMapper.Response(ResettleBetRequest request, BatchBetResult[] results)
        {
            var errors = new List<string>();

            foreach (var r in results)
            {
                if (r.Error != TgsApiError.None)
                {
                    errors.Add(r.ErrorMessage);
                }

                if (r.Errors.Length <= 0)
                {
                    continue;
                }

                var messages = r.Errors.Select(e => e.ErrorMessage).ToArray();
                errors.AddRange(messages);
            }

            return errors.Count == 0
                ? new() {Status = ((int)GameApiError.Success).ToString()}
                : new ResettleBetResponse
                {
                    Status = ((int)GameApiError.UnableProcessed).ToString(),
                    Message = errors.JoinAsString(";")
                };
        }

        private static BetData GetBetData(string playerCode, int? currency, GameProviderConfigurationData data,
            TransactionData[] transactions) =>
            new()
            {
                PlayerCode = playerCode,
                PlatformType = data.PlatformType,
                GameProviderId = data.GameProviderId,
                GameProviderCode = GameProviderCode,
                GameProviderConfigurationId = data.GameProviderConfigurationId,
                AutoCloseRound = data.AutoCloseRound,
                ForceGetPreviousActions = true,
                HasCurrencyFromGameProvider = currency.HasValue,
                CurrencyCode = currency?.ToString(),
                Transactions = transactions
            };

        private static BatchBetData GetBatchBetData(Guid gameProviderId, bool autoCloseRound,
            BatchTransactionData[] transactions) =>
            new()
            {
                GameProviderId = gameProviderId,
                GameProviderCode = GameProviderCode,
                AutoCloseRound = autoCloseRound,
                ExternalBatchId = $"{GameProviderCode}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
                HasCurrencyFromGameProvider = false,
                Transactions = transactions
            };

        private static DateTimeOffset GetTimestamp(string timestamp) =>
            DateTimeOffset.Parse(timestamp);
    }
}