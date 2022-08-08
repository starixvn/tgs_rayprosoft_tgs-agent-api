using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using TGS.Core.DataServices.Tgs;
using TGS.Services.BackOffice.Domain;

namespace TGS.BackOffice.Repositories;
internal class MarketingReportRepository
{
    private readonly ITgsRepository _tgsRepository;

    public MarketingReportRepository(ITgsRepository tgsRepository)
    {
        _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
    }

    public async IAsyncEnumerable<object> QueryRealTimePlayerDailyDepositStatisticsAsync(
        Guid brandId,
        DateOnly queryDate,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var (From, To) = (queryDate.AddDays(-1), queryDate);
        var queryTimeRange = (
            From: new DateTimeOffset(
                From
                    .AddDays(-1)
                    .ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(16D)), DateTimeKind.Utc),
                TimeSpan.Zero),
            To: new DateTimeOffset(
                To
                    .AddDays(-1)
                    .ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(16D)), DateTimeKind.Utc),
                TimeSpan.Zero));

        var conn = _tgsRepository.GetDbConnection();

        var result = await conn.QueryAsync<RealTimePlayerRegisterItem>(
            @"
                SELECT
                    P.player_code AS PlayerCode,
                    P.username AS UserName,
                    P.agent_id AS AgentId,
                    P.registered_on AS RegisteredOn,
                    PIR.record_on AS LastLoginOn,
                    PLRC.login_count AS LoginCount
                FROM
                    dbo.players AS P
                    LEFT JOIN
                    dbo.player_ip_records AS PIR ON P.player_code = PIR.player_code
                        AND PIR.ip_type = 2
                    LEFT JOIN
                    (
                        SELECT
                            P1.player_code,
                            Count(1) AS login_count
                        FROM
                            dbo.players AS P1
                            JOIN
                            dbo.player_login_records AS PLR ON P1.player_code = PLR.player_code
                        WHERE
                            P1.brand_id = @BrandId
                            AND
                            P1.agent_id IS NOT NULL
                            AND
                            P1.registered_on >= @Begin
                            AND
                            P1.registered_on < @End
                        GROUP BY
                            P1.player_code
                    ) AS PLRC ON P.player_code = PLRC.player_code
                WHERE
                    brand_id = @BrandId
                    AND
                    agent_id IS NOT NULL
                    AND
                    registered_on >= @Begin
                    AND
                    registered_on < @End",
            new
            {
                Begin = queryTimeRange.From,
                End = queryTimeRange.To,
                BrandId = brandId
            }).ConfigureAwait(false);


    }
}
