using System;

namespace TGS.Core.DataService.Models
{
    public class GameProviderCurrency
    {
        public Guid Id { get; set; }
        public Guid GameProviderId { get; set; }
        public GameProvider GameProvider { get; set; }
        public string CurrencyCode { get; set; }
        public Currency Currency { get; set; }
        public string GameProviderCurrencyCode { get; set; }
        public int DisplayOrder { get; set; }
    }
}