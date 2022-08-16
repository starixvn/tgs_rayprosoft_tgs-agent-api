using System;

namespace TGS.Core.DataServices.Tgs.Models;

public class PlayerDailyRegisterStatistics
{
    public int DailyLoginCount { get; set; }

    public DateTime? LastLoginOn { get; set; }

    public string PlayerCode { get; set; }

    public DateOnly ReportDate { get; set; }

    public int TotalLoginCount { get; set; }
}
