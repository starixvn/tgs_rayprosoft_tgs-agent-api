using System;

namespace TGS.Core.DataServices.Tgs.Models;

public class AgentDailyMarketingStatistics
{
    public Guid AgentId { get; set; }

    public Guid BrandId { get; set; }

    public int GamingCount { get; set; }

    public int RegisterCount { get; set; }

    public DateOnly ReportDate { get; set; }
}
