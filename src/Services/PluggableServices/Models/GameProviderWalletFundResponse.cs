using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Models.Messages;

namespace TGS.Services.PluggableServices.Models
{
    public record GameProviderWalletFundResponse : TgsResponse
    {
        public Guid? TransactionsId { get; init; }
        public string GameProviderTransactionId { get; init; }
    }
}
