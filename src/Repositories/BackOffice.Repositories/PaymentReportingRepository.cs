using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Castle.Core.Logging;
using Dapper;
using Microsoft.Extensions.Logging;
using TGS.Core.DataServices.Tgs;
using TGS.Services.BackOffice.Domain;

namespace TGS.BackOffice.Repositories;

internal class PaymentReportingRepository : IPaymentReportingRepository
{
    private readonly ITgsRepository _tgsRepository;
    private readonly ILogger<PaymentReportingRepository> _logger;

    public PaymentReportingRepository(ITgsRepository tgsRepository, ILogger<PaymentReportingRepository> logger)
    {
        _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

    public async IAsyncEnumerable<PlayerDailyPaymentItem> QueryPlayerDailyDepositStatisticsAsync(
        Guid brandId,
        DateOnly queryDate,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("query date: {queryDate}", queryDate);

        var conn = _tgsRepository.GetDbConnection();

        var result = await conn.QueryAsync<PlayerDailyPaymentItem>(
            @"
                    WITH CTE(
                        player_code,
                        is_first_deposit,
                        un_second_deposit
                    )
                    AS
                    (
                        SELECT
                            COALESCE(FD.player_code, SD.player_code) AS player_code,
                            FD.is_first_deposit,
                            CASE
                                WHEN SD.player_code IS NULL THEN false
                                ELSE true
                            END AS un_second_deposit
                        FROM
                            (
                                SELECT
                                    PS.player_code,
                                    CASE
                                        WHEN PS.report_date = (PS.first_deposit_on AT TIME ZONE 'Asia/Shanghai')::date THEN true
                                        ELSE false
                                    END AS is_first_deposit
                                FROM
                                    dbo.player_daily_payment_statistics AS PS
                                WHERE
                                    PS.report_date = @ReportDate
                            ) AS FD
                            FULL OUTER JOIN
                            (
                                SELECT
                                    player_code
                                FROM
                                    dbo.player_daily_payment_statistics AS PS
                                WHERE
                                    report_date <= @UndepositCheckDate
                                GROUP BY
                                    player_code
                                HAVING
                                    Max(total_deposit_count) = 1
                            ) AS SD ON FD.player_code = SD.player_code
                    ),
                    Info(
                        player_code,
                        first_deposit_on,
                        first_deposit_amount,
                        total_deposit_amount,
                        total_withdraw_amount
                    )
                    AS
                    (
                        SELECT
                            PS.player_code,
                            Max(PS.first_deposit_on) AS first_deposit_on,
                            COALESCE(Max(PS.first_deposit_amount), 0) AS first_deposit_amount,
                            COALESCE(Max(PS.total_deposit_amount), 0) AS total_deposit_amount,
                            COALESCE(Max(PS.total_withdraw_amount), 0) AS total_withdraw_amount
                        FROM
                            dbo.player_daily_payment_statistics AS PS
                            JOIN
                            CTE ON PS.player_code = CTE.player_code
                        WHERE
                            PS.report_date <= @ReportDate
                        GROUP BY
                            PS.player_code
                    )
                    SELECT
                        CTE.player_code AS PlayerCode,
                        P.username AS PlayerName,
                        P.agent_id AS AgentId,
                        CTE.is_first_deposit AS IsFirstDeposit,
                        CTE.un_second_deposit AS UnSecondDeposit,
                        Info.first_deposit_on AS FirstDepositOn,
                        Info.first_deposit_amount AS FirstDepositAmount,
                        Info.total_deposit_amount AS TotalDepositAmount,
                        Info.total_withdraw_amount AS TotalWithdrawAmount,
                        PIR.ip AS RegisterIp
                    FROM
                        CTE
                        JOIN
                        INFO ON CTE.player_code = Info.player_code
                        JOIN
                        dbo.players AS P ON CTE.player_code = P.player_code
                        LEFT JOIN
                        dbo.player_ip_records AS PIR ON CTE.player_code = PIR.player_code
                            AND PIR.ip_type = 2",
            new
            {
                ReportDate = queryDate,
                UndepositCheckDate = queryDate.AddDays(-2)
            }).ConfigureAwait(false);

        foreach (var item in result)
            yield return item;
    }
}
