using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TGS.Core.DataServices.Tgs;
using TGS.Services.BackOffice.Domain;

namespace TGS.BackOffice.Repositories;

internal class PaymentReportingReposiory : IPaymentReportingRepository
{
    private readonly ITgsRepository _tgsRepository;

    public PaymentReportingReposiory(ITgsRepository tgsRepository)
    {
        _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
    }

    public IAsyncEnumerable<AgentPlayerDepositStatistics> QueryAgentDailyDepositStatisticsAsync(
        Guid brandId,
        DateOnly queryLatestDate,
        CancellationToken cancellationToken = default)
    {
        var dateRange = (
            From: queryLatestDate.AddDays(-91),
            To: queryLatestDate);

        var query = _tgsRepository.AgentDailyPaymentStatistics.AsQueryable()
            .Where(row => row.BrandId == brandId
                && row.ReportDate >= dateRange.From
                && row.ReportDate < dateRange.To)
            .Select(row => new AgentPlayerDepositStatistics(
                row.ReportDate,
                row.AgentId,
                row.FirstDepositPlayers,
                row.DepositPlayers,
                row.UndepositAgainPlayers));

        return query
            .ToArray()
            .ToAsyncEnumerable();
    }
}
