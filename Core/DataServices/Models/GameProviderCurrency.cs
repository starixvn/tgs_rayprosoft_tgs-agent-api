using System;

namespace TGS.Core.DataServices.Models
{
    public class GameProviderCurrency
    {
        public Guid GameProviderId { get; set; }
        public virtual GameProvider GameProvider { get; set; }
        public string CurrencyCode { get; set; }
        public virtual Currency Currency { get; set; }
        public string GameProviderCurrencyCode { get; set; }
    }
}