using System;

#nullable enable

namespace TGS.Core.DataServices.Tgs.Models;

public class PlayerDailyPaymentStatistics
{
    public decimal? DailyDepositAmount { get; set; }

    public int? DailyDepositCount { get; set; }

    public decimal? DailyWithdrawAmount { get; set; }

    public int? DailyWithdrawCount { get; set; }

    public string PlayerCode { get; set; } = default!;

    public DateOnly ReportDate { get; set; }

    public decimal? TotalDepositAmount { get; set; }

    public int? TotalDepositCount { get; set; }

    public decimal? TotalWithdrawAmount { get; set; }

    public int? TotalWithdrawCount { get; set; }
}
