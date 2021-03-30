using System;
using System.Collections.Generic;
using System.Linq;
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

        public IAsyncEnumerable<StationLatter> QueryStationLattersAsync(string playerCode)
        {
            var query = from latterPlayer in _tgsRepository.StationLetterPlayers
                            .Include(latterPlayer => latterPlayer.StationLetter)
                            .AsAsyncEnumerable()
                        where latterPlayer.PlayerCode == playerCode
                        select new StationLatter(
                            latterPlayer.StationLetter.Subject,
                            latterPlayer.StationLetter.Content,
                            latterPlayer.StationLetter.CreatedOn);

            return query;
        }
    }
}
