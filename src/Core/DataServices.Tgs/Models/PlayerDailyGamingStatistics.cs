using System;

namespace TGS.Core.DataServices.Tgs.Models;

public class PlayerDailyGamingStatistics
{
    public string GpCode { get; set; }

    public string PlayerCode { get; set; }

    public DateOnly ReportDate { get; set; }

    public string SubGpCode { get; set; }

    public decimal WinLoss { get; set; }
}
