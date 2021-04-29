using System;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public class CreateWithdrawRequest
    {
        public decimal Amount { get; init; }

        public Guid BankInfoId { get; init; }
    }
}
