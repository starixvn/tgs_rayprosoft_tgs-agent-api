using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.GameProviders.Repositories.Models.GameProviderFundTransferTransaction
{
    public record GameProviderFundTransferTransactionGetResponse
    {
        public Guid GameProviderFundTransferTransactionId { get; set; }
        public Guid? TransactionId { get; set; }
        public Guid GameProviderId { get; set; }
        public Guid WalletId { get; set; }
        public string GameProviderTransactionId { get; set; }
        public string PlayerCode { get; set; }
        public FundSource FundSource { get; set; }
        public string FromWallet { get; set; }
        public string ToWallet { get; set; }
        public FundTransferTransactionStatus Status { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string? Description { get; set; }
    }
}
