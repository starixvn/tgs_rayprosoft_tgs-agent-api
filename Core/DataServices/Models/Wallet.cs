using System;

namespace TGS.Core.DataServices.Models
{
    public class Wallet
    {
        public Guid Id { get; set; }
        public string PlayerCode { get; set; }
        public Player Player { get; set; }
        public string CurrencyCode { get; set; }
        public Currency Currency { get; set; }
        public string ExternalCurrencyCode { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}