using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Operator;

namespace TGS.Operator.Repositories
{
    internal class StationLatterRepository : IStationLatterRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public StationLatterRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public IAsyncEnumerable<StationLatter> QueryStationLattersAsync(string playerCode, Paging<StationLatter> paging)
        {
            var query = QueryStationLattersAsync(playerCode);

            if (paging.OrderBy != null)
                query = paging.IsDesc
                    ? query.OrderByDescending(paging.OrderBy)
                    : query.OrderBy(paging.OrderBy);

            var page = paging.Page <= 0 ? 0 : paging.Page - 1;

            var pageSize = paging.RowCount <= 0 ? 10 : paging.RowCount;
            query = query.Skip(page * pageSize).Take(pageSize);

            return query;
        }

        public ValueTask<int> QueryStationLattersTotalCountAsync(string playerCode)
            => QueryStationLattersAsync(playerCode).CountAsync();

        public IAsyncEnumerable<StationLatter> QueryStationLattersAsync(string playerCode)
        {
            var query = from latterPlayer in _tgsRepository.StationLetterPlayers
                            .Include(latterPlayer => latterPlayer.StationLetter)
                            .AsAsyncEnumerable()
                        where latterPlayer.PlayerCode == playerCode
                        select new StationLatter(
                            latterPlayer.StationLetterId,
                            latterPlayer.StationLetter.Subject,
                            latterPlayer.StationLetter.Content,
                            latterPlayer.StationLetter.CreatedOn);

            return query;
        }

        public async ValueTask SetStationLatterHaveReadAsync(string playerCode, Guid latterId)
        {
            var latter = await _tgsRepository.StationLetterPlayers
                .FindAsync(latterId, playerCode)
                .ConfigureAwait(false);

            if (latter is not null)
            {
                latter.IsRead = true;
                latter.ReadOn = DateTimeOffset.UtcNow;

                await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
