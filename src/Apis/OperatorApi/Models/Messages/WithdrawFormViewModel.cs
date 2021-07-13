using System;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record WithdrawFormViewModel
    {
        public Guid Id { get; init; }

        public string Status { get; init; }

        public decimal Amount { get; init; }

        public DateTimeOffset CreatedOn { get; init; }

        public DateTimeOffset UpdatedOn { get; init; }
    }
}