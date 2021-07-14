using System;
using TGS.Apis.OperatorApi.Models.Messages.Deposit;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record DepositFormViewModel
    {
        public Guid Id { get; init; }

        public FlowState Status { get; init; }

        public decimal Amount { get; init; }

        public DateTimeOffset CreatedOn { get; init; }

        public DateTimeOffset UpdatedOn { get; init; }
    }
}