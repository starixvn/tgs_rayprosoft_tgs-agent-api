using System;
using System.Threading.Tasks;
using TGS.Core.DataServices;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Infrastructures;
using TGS.Infrastructures.Redis;
using TGS.Services.Operations.Validators;

namespace TGS.Services.Operations.Bets
{
    public interface IGameActionDataOperations
    {
        Task<BetResult> PlaceBetAsync(BetData data);
    }

    public sealed partial class GameActionDataOperations : IGameActionDataOperations
    {
        private readonly Func<ITgsRepository> _tgsRepoFactory;
        private readonly ITransactionManager _transactionManager;
        private readonly IRedisCacheManager _cacheManager;
        private readonly IPlayerOperations _playerOperations;
        private readonly ILicenseeOperations _licenseeOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ITransactionSettingsGenerator _transactionSettingsGenerator;
        private readonly IAmountValidator _amountValidator;
        private readonly IStatusValidator _statusValidator;
        private readonly ITimeZoneManager _timeZoneManager;
        private readonly IRoundStatusManager _roundStatusManager;

        private static readonly GameType[] BypassPreviousActionGameTypes = { GameType.Slot };

        public GameActionDataOperations(
            Func<ITgsRepository> tgsRepoFactory,
            ITransactionManager transactionManager,
            IRedisCacheManager cacheManager,
            IPlayerOperations playerOperations,
            ILicenseeOperations licenseeOperations,
            IGameProviderOperations gameProviderOperations,
            ITransactionSettingsGenerator transactionSettingsGenerator,
            IAmountValidator amountValidator,
            IStatusValidator statusValidator,
            ITimeZoneManager timeZoneManager,
            IRoundStatusManager roundStatusManager)
        {
            _tgsRepoFactory = tgsRepoFactory;
            _transactionManager = transactionManager;
            _cacheManager = cacheManager;
            _playerOperations = playerOperations;
            _licenseeOperations = licenseeOperations;
            _gameProviderOperations = gameProviderOperations;
            _transactionSettingsGenerator = transactionSettingsGenerator;
            _amountValidator = amountValidator;
            _statusValidator = statusValidator;
            _timeZoneManager = timeZoneManager;
            _roundStatusManager = roundStatusManager;
        }

        Task<BetResult> IGameActionDataOperations.PlaceBetAsync(BetData data)
        {
            return ExecuteGameActionAsync(data);
        }
    }
}