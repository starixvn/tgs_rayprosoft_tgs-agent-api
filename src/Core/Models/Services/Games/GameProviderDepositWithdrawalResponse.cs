using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.Services.Games
{
    public record GameProviderDepositWithdrawalResponse
    {
        public Guid TransactionsId { get; set; }
        public Guid TransactionGameProviderId { get; set; }
        public decimal Balance { get; set; }
    }
}
