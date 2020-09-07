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

        public GameActionDataOperations(
            Func<ITgsRepository> tgsRepoFactory,
            //Func<IBetRepository> betRepoFactory,
            ITransactionManager transactionManager,
            IRedisCacheManager cacheManager)
        {
            _tgsRepoFactory = tgsRepoFactory;
            //_betRepoFactory = betRepoFactory;
            _transactionManager = transactionManager;
            _cacheManager = cacheManager;
        }

        Task<BetResult> IGameActionDataOperations.PlaceBetAsync(BetData data)
        {
            return null;
        }
    }
}