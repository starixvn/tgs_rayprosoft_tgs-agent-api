using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Report.Application.Models.GameAction
{
    public class GameActionModel
    {
        public DateTimeOffset RecordedOn { get; set; }

        public string? ExternalTransactionId { get; set; }

        public GameActionType GameActionType { get; set; }

        public decimal Amount { get; set; }

        public decimal BeforeBalance { get; set; }

        public decimal AfterBalance { get; set; }
    }
}