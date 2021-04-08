using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Operator;

namespace TGS.Operator.Repositories
{
    internal class StationLetterRepository : IStationLetterRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public StationLetterRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public IAsyncEnumerable<StationLetter> QueryStationLettersAsync(string playerCode, Paging<StationLetter> paging)
        {
            var query = QueryStationLettersAsync(playerCode);

            if (paging.OrderBy != null)
                query = paging.IsDesc
                    ? query.OrderByDescending(paging.OrderBy)
                    : query.OrderBy(paging.OrderBy);

            var page = paging.Page <= 0 ? 0 : paging.Page - 1;

            var pageSize = paging.RowCount <= 0 ? 10 : paging.RowCount;
            query = query.Skip(page * pageSize).Take(pageSize);

            return query;
        }

        public ValueTask<int> QueryStationLettersTotalCountAsync(string playerCode)
            => QueryStationLettersAsync(playerCode).CountAsync();

        public IAsyncEnumerable<StationLetter> QueryStationLettersAsync(string playerCode)
        {
            var query = from letterPlayer in _tgsRepository.StationLetterPlayers
                            .Include(letterPlayer => letterPlayer.StationLetter)
                            .AsAsyncEnumerable()
                        where letterPlayer.PlayerCode == playerCode
                        select new StationLetter(
                            letterPlayer.StationLetterId,
                            letterPlayer.StationLetter.Subject,
                            letterPlayer.StationLetter.Content,
                            letterPlayer.StationLetter.CreatedOn,
                            letterPlayer.IsRead);

            return query;
        }

        public async ValueTask SetStationLetterHaveReadAsync(string playerCode, Guid letterId)
        {
            var letter = await _tgsRepository.StationLetterPlayers
                .FindAsync(letterId, playerCode)
                .ConfigureAwait(false);

            if (letter is not null)
            {
                letter.IsRead = true;
                letter.ReadOn = DateTimeOffset.UtcNow;

                await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
