using System;
using System.Threading.Tasks;
using TGS.Core.DataServices;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Infrastructures.Redis;
using TGS.Services.Operations.Validators;

namespace TGS.Services.Operations
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
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ITransactionSettingsGenerator _transactionSettingsGenerator;
        private readonly IAmountValidator _amountValidator;

        private static readonly GameType[] BypassPreviousActionGameTypes = { GameType.Slot };

        public GameActionDataOperations(
            Func<ITgsRepository> tgsRepoFactory,
            ITransactionManager transactionManager,
            IRedisCacheManager cacheManager,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            ITransactionSettingsGenerator transactionSettingsGenerator,
            IAmountValidator amountValidator)
        {
            _tgsRepoFactory = tgsRepoFactory;
            _transactionManager = transactionManager;
            _cacheManager = cacheManager;
            _playerOperations = playerOperations;
            _gameProviderOperations = gameProviderOperations;
            _transactionSettingsGenerator = transactionSettingsGenerator;
            _amountValidator = amountValidator;
        }

        Task<BetResult> IGameActionDataOperations.PlaceBetAsync(BetData data)
        {
            return ExecuteGameActionAsync(data);
        }
    }
}