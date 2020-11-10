using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using TGS.Core.DataServices;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.TimeZone;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations.Calculators;
using TGS.Services.GameOperations.Models.Data;
using TGS.Services.GameOperations.Validators;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Services.GameOperations.Data
{
    public interface IGameActionDataOperations
    {
        Task<BetResult> PlaceBetAsync(BetData data);
        
        Task<BetResult> SettleBetAsync(BetData data);

        Task<BetResult> CancelTransactionAsync(BetData data);
    }

    internal sealed partial class GameActionDataOperations : IGameActionDataOperations
    {
        private readonly Func<ITgsRepository> _tgsRepoFactory;
        private readonly ITransactionManager _transactionManager;
        private readonly IPlayerOperations _playerOperations;
        private readonly ILicenseeOperations _licenseeOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameActionProcessor _processor;
        private readonly IStatusValidator _statusValidator;

        public GameActionDataOperations(
            Func<ITgsRepository> tgsRepoFactory,
            ITransactionManager transactionManager,
            IStatusValidator statusValidator,
            IPlayerOperations playerOperations,
            ILicenseeOperations licenseeOperations,
            IGameProviderOperations gameProviderOperations,
            IGameActionProcessor processor)
        {
            _tgsRepoFactory = tgsRepoFactory;
            _transactionManager = transactionManager;
            _statusValidator = statusValidator;
            _playerOperations = playerOperations;
            _licenseeOperations = licenseeOperations;
            _gameProviderOperations = gameProviderOperations;
            _processor = processor;
        }

        Task<BetResult> IGameActionDataOperations.PlaceBetAsync(BetData data)
        {
            return ExecuteGameActionAsync(data);
        }

        Task<BetResult> IGameActionDataOperations.SettleBetAsync(BetData data)
        {
            return ExecuteGameActionAsync(data);
        }

        Task<BetResult> IGameActionDataOperations.CancelTransactionAsync(BetData data)
        {
            return ExecuteGameActionAsync(data);
        }

        private async Task<BetResult> ExecuteGameActionAsync(BetData data)
        {
            if (data?.Transactions == null || data.Transactions.Length == 0)
            {
                throw new InvalidOperationException("No transactions in the request");
            }

            var cachedPlayer = await _playerOperations.GetCachedPlayerAsync(data.PlayerCode);
            ThrowIfPlayerDisableToPlaceBet(data.Transactions, cachedPlayer.Status);

            // TODO: validate if place bet cap amount is exceed

            var cachedLicensee = await _licenseeOperations.GetCachedLicenseeAsync(cachedPlayer.LicenseeId);

            var playerRelatedData = new PlayerRelatedData
            {
                Wallet = new WalletData
                {
                    Id = cachedPlayer.DefaultWalletId,
                    CurrencyCode = cachedPlayer.DefaultWalletCurrencyCode,
                    ExternalCurrencyCode = cachedPlayer.ExternalCurrencyCode
                },
                PlayerCode = data.PlayerCode,
                LicenseeId = cachedPlayer.LicenseeId,
                LicenseeCode = cachedLicensee.Code,
                LicenseeName = cachedLicensee.Name,
                Status = cachedPlayer.Status,
                PlayerType = cachedPlayer.Type,
                PlayerUsername = cachedPlayer.Username,
                ExternalCurrencyCode = cachedPlayer.ExternalCurrencyCode,
                LicenseeTimeZoneId = cachedLicensee.TimeZoneId,
                PlayerIp = string.Empty
            };

            var cacheGameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(data.GameProviderId);
            var games = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId);

            var externalTransactionIds = data.Transactions.Select(t => t.ExternalTransactionId).JoinAsStringBySemicolon();
            var transactionResult = await _transactionManager.ExecuteTransactionAsync(
                _tgsRepoFactory,
                IsolationLevel.RepeatableRead,
                externalTransactionIds,
                execute: async (r, trans) => await _processor.ExecuteAsync(new ProcessorData
                {
                    Repository = r,
                    Transaction = trans.GetDbTransaction(),
                    BetData = data,
                    Player = playerRelatedData,
                    GameProvider = cacheGameProvider,
                    Games = games
                }));

            return transactionResult;
        }

        private void ThrowIfPlayerDisableToPlaceBet(TransactionData[] transactions, PlayerStatus playerStatus)
        {
            if (transactions.Any(t => t.GameActionType.IsOneOf(LockAmount, PlaceBet)))
            {
                _statusValidator.ThrowIfPlayerDisabledToPlaceBet(playerStatus);
            }
        }
    }
}