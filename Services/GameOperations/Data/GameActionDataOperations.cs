using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations.Models.Data;
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

    internal sealed class GameActionDataOperations : IGameActionDataOperations
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IGameActionProcessor _processor;
        private readonly IStatusValidator _statusValidator;
        private readonly Func<ITgsRepository> _tgsRepoFactory;
        private readonly ITransactionManager _transactionManager;

        public GameActionDataOperations(
            Func<ITgsRepository> tgsRepoFactory,
            ITransactionManager transactionManager,
            IStatusValidator statusValidator,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IGameActionProcessor processor)
        {
            _tgsRepoFactory = tgsRepoFactory;
            _transactionManager = transactionManager;
            _statusValidator = statusValidator;
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _processor = processor;
        }

        Task<BetResult> IGameActionDataOperations.PlaceBetAsync(BetData data) =>
            ExecuteGameActionAsync(data);

        Task<BetResult> IGameActionDataOperations.SettleBetAsync(BetData data) =>
            ExecuteGameActionAsync(data);

        Task<BetResult> IGameActionDataOperations.CancelTransactionAsync(BetData data) =>
            ExecuteGameActionAsync(data);

        private async Task<BetResult> ExecuteGameActionAsync(BetData data)
        {
            if (data?.Transactions == null || data.Transactions.Length == 0)
            {
                throw new InvalidOperationException("No transactions in the request");
            }

            var player = await _playerOperations.GetCachedPlayerAsync(data.PlayerCode);
            ThrowIfPlayerDisableToPlaceBet(data.Transactions, player.Status);

            // TODO: validate if place bet cap amount is exceed

            var brand = await _brandOperations.GetCachedBrandAsync(player.BrandId);

            var playerRelatedData = new PlayerRelatedData
            {
                Wallet = new()
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

            var gameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(data.GameProviderId);
            var games = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId);

            var externalTransactionIds =
                data.Transactions.Select(t => t.ExternalTransactionId).JoinAsStringBySemicolon();
            var transactionResult = await _transactionManager.ExecuteTransactionAsync(
                _tgsRepoFactory,
                IsolationLevel.RepeatableRead,
                externalTransactionIds,
                async r => await _processor.ExecuteAsync(new ProcessorData
                {
                    Repository = r,
                    BetData = data,
                    Player = playerRelatedData,
                    GameProvider = gameProvider,
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