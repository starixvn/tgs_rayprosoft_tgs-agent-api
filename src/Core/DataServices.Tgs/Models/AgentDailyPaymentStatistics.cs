using System;

namespace TGS.Core.DataServices.Tgs.Models;

public class AgentDailyPaymentStatistics
{
    public Guid AgentId { get; set; }

    public Guid BrandId { get; set; }

    public int FirstDepositPlayers { get; set; }

    public DateOnly ReportDate { get; set; }

    public int UndepositAgainPlayers { get; set; }
}
