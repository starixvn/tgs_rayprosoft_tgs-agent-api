using System;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public class DepositFormResponse
    {
        public Guid Id { get; init; }

        public Guid MethodId { get; init; }

        public Guid ChannelId { get; init; }

        public decimal Amount { get; init; }

        public string BankName { get; init; }

        public string BankAccountName { get; init; }

        public string BankAccount { get; init; }
    }
}
