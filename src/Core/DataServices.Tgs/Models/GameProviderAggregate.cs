using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class GameProviderAggregate
    {
        public int Id { get; set; }

        public Guid BrandId { get; set; }

        public Guid GameProviderId { get; set; }

        public string CurrencyCode { get; set; }

        public int TransactionCount { get; set; }

        public decimal BetAmount { get; set; }

        public decimal ValidBetAmount { get; set; }

        public decimal Turnover { get; set; }

        public decimal WinLoss { get; set; }

        public DateTimeOffset ExternalClosedOn { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}