using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public Wallet Wallet { get; set; }
        public Guid BrandId { get; set; }
        public Brand Brand { get; set; }
        public string ExternalTransactionId { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string Description { get; set; }
        public TransactionType Type { get; set; }
        public DateTimeOffset RecordedOn { get; set; }
        public DateTimeOffset SentOn { get; set; }
    }
}