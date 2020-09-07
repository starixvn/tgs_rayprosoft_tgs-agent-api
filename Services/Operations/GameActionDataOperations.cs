using System;
using System.Threading.Tasks;
using TGS.Core.DataServices;
using TGS.Core.Models.Services.Games;
using TGS.Infrastructures.Redis;

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

        public GameActionDataOperations(
            Func<ITgsRepository> tgsRepoFactory,
            ITransactionManager transactionManager,
            IRedisCacheManager cacheManager,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            ITransactionSettingsGenerator transactionSettingsGenerator)
        {
            _tgsRepoFactory = tgsRepoFactory;
            _transactionManager = transactionManager;
            _cacheManager = cacheManager;
            _playerOperations = playerOperations;
            _gameProviderOperations = gameProviderOperations;
            _transactionSettingsGenerator = transactionSettingsGenerator;
        }

        Task<BetResult> IGameActionDataOperations.PlaceBetAsync(BetData data)
        {
            return ExecuteGameActionAsync(data);
        }
    }
}