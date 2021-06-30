using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.GameProviders.Repositories.Models.GameProviderFundTransferTransaction
{
    public record UpdateGameProviderFundTransferTransactionRequest
    {
        public Guid GameProviderFundTransferTransactionId { get; set; }
        public Guid? TransactionId { get; set; }
        public string GameProviderTransactionId { get; set; }
        public FundTransferTransactionStatus Status { get; set; }
        public decimal Balance { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string Description { get; set; }
    }
}
