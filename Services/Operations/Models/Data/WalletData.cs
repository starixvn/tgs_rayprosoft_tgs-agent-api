using System;

namespace TGS.Services.Operations.Models.Data
{
    internal class WalletData
    {
        public Guid Id { get; init; }
        public string CurrencyCode { get; init; }
        public string ExternalCurrencyCode { get; init; }

        // We will fetch the latest balance in proper cases.
        public decimal Balance { get; set; }
    }
}
