using System;
using System.Threading.Tasks;
using TGS.Services.Operations.Queries;

namespace TGS.Services.Operations
{
    public interface IGameProviderOperations
    {
        Task<Guid> GetGameProviderIdByCodeAsync(string gameProviderCode);
    }

    public sealed class GameProviderOperations : IGameProviderOperations
    {
        private readonly Func<IGameProviderQueries> _gameProviderQueriesFactory;

        public GameProviderOperations(Func<IGameProviderQueries> gameProviderQueriesFactory)
        {
            _gameProviderQueriesFactory = gameProviderQueriesFactory;
        }

        Task<Guid> IGameProviderOperations.GetGameProviderIdByCodeAsync(string gameProviderCode)
        {
            // TODO: add cache if needed
            return _gameProviderQueriesFactory().GetGameProviderIdByCodeAsync(gameProviderCode);
        }
    }
}
