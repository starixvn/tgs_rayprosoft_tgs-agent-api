using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.SBOBET.Enums;
using TGS.Shared.Game.Models.SBOBET.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Factories
{
    internal sealed class RngBetService : IBetService
    {
        private static readonly string _RedisKeyPrefix = "SBO:RNG";
        private static readonly string _BalanceKey = $"{_RedisKeyPrefix}:Balance";
        private readonly IGamePlayActions _actions;

        private readonly IRedisCacheManager _redisCacheManager;

        public RngBetService(
            IRedisCacheManager redisCacheManager,
            IGamePlayActions actions)
        {
            _redisCacheManager = redisCacheManager;
            _actions = actions;
        }

        async ValueTask<GetBalanceResponse> IBetService.GetBalanceAsync(GetBalanceRequest request)
        {
            var playerCode = request.PlayerCode;

            var balance = await GetBalanceAsync(playerCode);

            return new()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMessage = string.Empty,
                PlayerCode = playerCode,
                Balance = balance
            };
        }

        public async ValueTask<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request)
        {
            var playerCode = request.PlayerCode;
            var betAmount = request.Amount;

            var transactionId = request.ExternalRoundId;
            var transferCode = request.ExternalTransactionId;

            var balance = await GetBalanceAsync(playerCode);
            var betInfos = await GetBetInfosAsync(transferCode);

            if (betInfos.Count == 0)
            {
                var betInfo = new BetInfo(transferCode, transactionId, betAmount);
                betInfos[transactionId] = betInfo;

                await CachedBetInfosInfoAsync(transferCode, betInfos);

                balance -= betAmount;

                await CacheBalanceAsync(playerCode, balance).ConfigureAwait(false);

                return new()
                {
                    ErrorCode = (int)GameApiError.Success,
                    ErrorMessage = string.Empty,
                    PlayerCode = playerCode,
                    Balance = balance
                };
            }

            if (!betInfos.TryGetValue(transactionId, out var data))
            {
                return new()
                {
                    ErrorCode = (int)GameApiError.BetNotExist,
                    ErrorMessage = GameApiError.BetNotExist.Description(),
                    PlayerCode = playerCode
                };
            }

            switch (data.Status)
            {
                case BetStatus.Settled:
                    return new()
                    {
                        ErrorCode = (int)GameApiError.BetAlreadySettled,
                        ErrorMessage = GameApiError.BetAlreadySettled.Description(),
                        PlayerCode = playerCode
                    };

                case BetStatus.Void:
                    return new()
                    {
                        ErrorCode = (int)GameApiError.BetAlreadyCanceled,
                        ErrorMessage = GameApiError.BetAlreadyCanceled.Description(),
                        PlayerCode = playerCode
                    };
            }

            if (betAmount <= data.BetAmount)
            {
                return new()
                {
                    ErrorCode = (int)GameApiError.BetAlreadyExists,
                    ErrorMessage = GameApiError.BetAlreadyExists.Description(),
                    PlayerCode = playerCode
                };
            }

            // deduct twice with greater amount
            // (should cancel the original bet amount and deduct new)
            balance = balance + data.BetAmount - betAmount;

            data = new(transferCode, transactionId, betAmount);
            betInfos[transactionId] = data;

            await CacheBalanceAsync(playerCode, balance).ConfigureAwait(false);
            await CachedBetInfosInfoAsync(transferCode, betInfos);

            return new()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMessage = string.Empty,
                PlayerCode = playerCode,
                Balance = balance
            };
        }

        public async ValueTask<SettleBetResponse> SettleBetAsync(SettleBetRequest request)
        {
            var playerCode = request.PlayerCode;
            var winloss = request.WinLoss;
            var transferCode = request.ExternalTransactionId;

            var betInfos = await GetBetInfosAsync(transferCode);
            if (betInfos == null || betInfos.Count == 0)
            {
                return new()
                {
                    ErrorCode = (int)GameApiError.BetNotExist,
                    ErrorMessage = GameApiError.BetNotExist.Description(),
                    PlayerCode = playerCode
                };
            }

            var balance = await GetBalanceAsync(playerCode);
            var betInfo = betInfos.ElementAt(0).Value;

            switch (betInfo.Status)
            {
                case BetStatus.Settled:
                    return new()
                    {
                        ErrorCode = (int)GameApiError.BetAlreadySettled,
                        ErrorMessage = GameApiError.BetAlreadySettled.Description(),
                        PlayerCode = playerCode
                    };

                case BetStatus.Void:
                    return new()
                    {
                        ErrorCode = (int)GameApiError.BetAlreadyCanceled,
                        ErrorMessage = GameApiError.BetAlreadyCanceled.Description(),
                        PlayerCode = playerCode
                    };
            }

            var settleInfo = betInfo with
            {
                SettleAmount = winloss,
                Status = BetStatus.Settled
            };

            betInfos[betInfo.TransactionId] = settleInfo;
            balance += settleInfo.SettleAmount!.Value;

            await CacheBalanceAsync(playerCode, balance).ConfigureAwait(false);
            await CachedBetInfosInfoAsync(transferCode, betInfos);

            return new()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMessage = string.Empty,
                PlayerCode = playerCode,
                Balance = balance
            };
        }

        public async ValueTask<CancelBetResponse> CancelBetAsync(CancelBetRequest request)
        {
            var playerCode = request.PlayerCode;
            var transferCode = request.ExternalTransactionId;

            var betInfos = await GetBetInfosAsync(transferCode);
            if (betInfos == null || betInfos.Count == 0)
            {
                return new()
                {
                    ErrorCode = (int)GameApiError.BetNotExist,
                    ErrorMessage = GameApiError.BetNotExist.Description(),
                    PlayerCode = playerCode
                };
            }

            var balance = await GetBalanceAsync(playerCode);
            var betInfo = betInfos.ElementAt(0).Value;

            if (betInfo.Status == BetStatus.Void)
            {
                return new()
                {
                    ErrorCode = (int)GameApiError.BetAlreadyCanceled,
                    ErrorMessage = GameApiError.BetAlreadyCanceled.Description(),
                    PlayerCode = playerCode
                };
            }

            var cancelInfo = betInfo with {Status = BetStatus.Void};

            betInfos[betInfo.TransactionId] = cancelInfo;
            balance = balance - betInfo.SettleAmount.GetValueOrDefault() + betInfo.BetAmount;

            await CacheBalanceAsync(playerCode, balance).ConfigureAwait(false);
            await CachedBetInfosInfoAsync(transferCode, betInfos);

            return new()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMessage = string.Empty,
                PlayerCode = playerCode,
                Balance = balance
            };
        }

        public async ValueTask<RollbackResponse> RollbackAsync(RollbackRequest request) =>
            throw new NotImplementedException();

        public async ValueTask<GetBetStatusResponse> GetBetStatusAsync(GetBetStatusRequest request)
        {
            var playerCode = request.PlayerCode;
            var transactionId = request.ExternalRoundId;
            var transferCode = request.ExternalTransactionId;

            var betInfos = await GetBetInfosAsync(transferCode);
            if (!betInfos.TryGetValue(transactionId, out var betInfo))
            {
                return new()
                {
                    ErrorCode = (int)GameApiError.BetNotExist,
                    ErrorMessage = GameApiError.BetNotExist.Description()
                };
            }

            return new()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMessage = string.Empty,
                ExternalRoundId = betInfo.TransactionId,
                ExternalTransactionId = betInfo.TransferCode,
                Stake = betInfo.BetAmount,
                WinLoss = betInfo.SettleAmount.HasValue ? betInfo.SettleAmount.Value - betInfo.BetAmount : 0,
                Status = betInfo.Status.Description()
            };
        }

        private async ValueTask<decimal> GetBalanceAsync(string playerCode)
        {
            var balance = await _redisCacheManager.SafeGetAndCacheAsync(_BalanceKey, playerCode, TimeSpan.FromDays(1),
                async () =>
                {
                    var r = await _actions.GetBalanceAsync(Consts.GameProvider.SBOBET, playerCode);
                    return r.Balance;
                }).ConfigureAwait(false);

            return balance;
        }

        private async ValueTask CacheBalanceAsync(string playerCode, decimal balance) =>
            await _redisCacheManager.SafeCacheAsync(_BalanceKey, playerCode, TimeSpan.FromDays(1), balance)
                .ConfigureAwait(false);

        private async Task CachedBetInfosInfoAsync(string transferCode, Dictionary<string, BetInfo> betInfos) =>
            await _redisCacheManager.SafeCacheAsync(_RedisKeyPrefix, transferCode,
                TimeSpan.FromDays(1), betInfos).ConfigureAwait(false);

        private async ValueTask<Dictionary<string, BetInfo>> GetBetInfosAsync(string transferCode) =>
            await _redisCacheManager.SafeGetAsync(_RedisKeyPrefix,
                transferCode, () =>
                {
                    var items = new Dictionary<string, BetInfo>();
                    return Task.FromResult(items);
                }).ConfigureAwait(false);

        private record BetInfo(string TransferCode, string TransactionId, decimal BetAmount,
            decimal? SettleAmount = null, BetStatus Status = BetStatus.Running);
    }
}