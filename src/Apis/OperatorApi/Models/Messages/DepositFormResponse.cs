using System;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public class DepositFormResponse
    {
        public string MethodName { get; init; }

        public string ChannelName { get; init; }

        public decimal Amount { get; init; }

        public string BankName { get; init; }

        public string BankAccountName { get; init; }

        public string BankAccount { get; init; }
    }
}
