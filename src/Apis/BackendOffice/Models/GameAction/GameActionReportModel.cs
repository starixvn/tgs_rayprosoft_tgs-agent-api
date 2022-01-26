using System;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.Models.GameAction
{
    public record GameActionReportModel
    {
        public DateTimeOffset RecordedOn { get; init; }

        public string ExternalTransactionId { get; init; } = default!;

        public GameActionType GameActionType { get; init; }

        public decimal Amount { get; init; }

        public decimal BeforeBalance { get; init; }

        public decimal AfterBalance { get; init; }
    }
}