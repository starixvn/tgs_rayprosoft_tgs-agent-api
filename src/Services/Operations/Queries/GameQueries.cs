using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.Operations.Queries
{
    public interface IGameQueries
    {
        Task<Game[]> GetGamesLocalizationByGameIdsAsync(int[] gameIds);
    }

    public sealed class GameQueries : IGameQueries
    {
        private readonly ITgsRepository _repository;

        public GameQueries(ITgsRepository repository)
        {
            _repository = repository;
        }

        Task<Game[]> IGameQueries.GetGamesLocalizationByGameIdsAsync(int[] gameIds)
        {
            return _repository.Games
                .Where(x => gameIds.Contains(x.Id)
                    && x.Localization != null)
                .Select(x => new Game() { Id = x.Id, Localization = x.Localization })
                .ToArrayAsync();
        }
    }
}