using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Report.Application.Models.Round
{
    public class RoundAggregateModel
    {
        public Guid BrandId { get; set; }
        public Guid? AgentId { get; set; }
        public string PlayerCode { get; set; }
        public GameType GameType { get; set; }
        public string Username { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public decimal BetAmount { get; set; }
        public decimal ValidBetAmount { get; set; }
        public decimal Turnover { get; set; }
        public decimal WinLoss { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ReportDate { get; set; }
    }
}