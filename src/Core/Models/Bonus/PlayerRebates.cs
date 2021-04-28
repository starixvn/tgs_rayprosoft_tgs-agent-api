using System;

namespace TGS.Core.Models.Bonus
{
    public record PlayerRebates(string PlayerCode, decimal BonusAmount, DateTimeOffset ReportDate);
}