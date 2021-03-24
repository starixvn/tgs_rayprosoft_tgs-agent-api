using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Report.Application.Models.Round;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface ITgsRoundService
    {
        ITgsRepository TgsRepository { get; }

        IQueryable<Round> GetRoundQueries(Func<IQueryable<Round>, IQueryable<Round>> preQuery = null,
            bool tracking = false);

        Task<RoundAggregateModel[]> GetRoundsAsync(DateTimeOffset start, DateTimeOffset end);
    }

    public class TgsRoundService : BaseRepositoryOperations<ITgsRepository>, ITgsRoundService
    {
        public readonly ITgsRepository _repository;

        public TgsRoundService(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        ITgsRepository ITgsRoundService.TgsRepository => _repository;

        IQueryable<Round> ITgsRoundService.GetRoundQueries(
            Func<IQueryable<Round>, IQueryable<Round>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        async Task<RoundAggregateModel[]> ITgsRoundService.
            GetRoundsAsync(DateTimeOffset start, DateTimeOffset end)
        {
            const string spName = "dbo.report_get_round_aggregates";
            var result = await _repository
                .SqlQueryMultipleAsync<RoundAggregateModel>(spName, parameters:
                    new Dictionary<string, object>
                    {
                        {"_from", start},
                        {"_to", end},
                        {
                            "_report_date",
                            new DateTimeOffset(start.Year, start.Month, start.Day, 0, 0, 0, start.Offset)
                        },
                        {"_hour", start.Hour},
                        {"_minute", 0}
                    });
            return result.ToArray();
        }
    }
}