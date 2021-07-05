using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Extensions;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.Game
{
    internal class UpdateGameLocalizationQueryService : IGetService<UpdateGameLocalizationQuery, UpdateGameLocationInfo>
    {
        private readonly ITgsRepository _tgsRepository;

        public UpdateGameLocalizationQueryService(
            ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<UpdateGameLocationInfo> GetAsync(UpdateGameLocalizationQuery query, CancellationToken cancellationToken = default)
        {
            var existGameIds = await _tgsRepository.Games.AsAsyncEnumerable()
                .Where(p => query.Games.Select(x => x.Id).ToList().Contains(p.Id.ToString()))
                .Select(p => p.Id)
                .ToListAsync();

            var diffGames = query.Games.Select(x => int.Parse(x.Id)).ToArray().Except(existGameIds).ToArray();

            if (diffGames.Length == 0)
            {
                foreach (var item in existGameIds)
                {
                    var model = query.Games.First(x => x.Id == item.ToString());
                    var game = await _tgsRepository.Games.FirstOrDefaultAsync(x => x.Id == item);
                    game.Localization = model.Localization.ToJson();
                }
                await _tgsRepository.SaveChangesAsync();

                return new UpdateGameLocationInfo()
                {
                    DiffGameIds = Array.Empty<int>()
                };
            }
            else
            {
                return new UpdateGameLocationInfo()
                {
                    DiffGameIds = diffGames
                };
            }
        }
    }
}