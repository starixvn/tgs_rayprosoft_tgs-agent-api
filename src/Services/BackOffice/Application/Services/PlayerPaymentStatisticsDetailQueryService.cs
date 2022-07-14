using System;
using System.Collections.Generic;
using System.Threading;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.Domain;

namespace TGS.Services.BackOffice.Application.Services;

internal class PlayerPaymentStatisticsDetailQueryService : IQueryService<PlayerPaymentStatisticsDetailQuery, PlayerDailyPaymentItem>
{
    private readonly IPaymentReportingRepository _paymentReportingRepository;

    public PlayerPaymentStatisticsDetailQueryService(IPaymentReportingRepository paymentReportingRepository)
    {
        _paymentReportingRepository = paymentReportingRepository ?? throw new ArgumentNullException(nameof(paymentReportingRepository));
    }

    public IAsyncEnumerable<PlayerDailyPaymentItem> QueryAsync(
        PlayerPaymentStatisticsDetailQuery query,
        CancellationToken cancellationToken = default)
        => _paymentReportingRepository.QueryPlayerDailyDepositStatisticsAsync(
            query.BrandId,
            query.QueryDate,
            cancellationToken);
}
