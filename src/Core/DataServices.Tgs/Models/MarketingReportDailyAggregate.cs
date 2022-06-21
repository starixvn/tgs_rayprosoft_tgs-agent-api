#nullable enable

using System;

namespace TGS.Core.DataServices.Tgs.Models;

public class MarketingReportDailyAggregate
{
    public Guid AgentId { get; set; } = default!;

    public int FirstDepositCount { get; set; }

    public int GamingCount { get; set; }

    public int RegisterCount { get; set; }

    public DateOnly ReportDate { get; set; }
}
