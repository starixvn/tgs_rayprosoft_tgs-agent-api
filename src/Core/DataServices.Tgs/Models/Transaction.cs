using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models;

public class Transaction
{
    public decimal Amount { get; set; }

    public decimal Balance { get; set; }

    public virtual Brand Brand { get; set; }

    public Guid BrandId { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public string Description { get; set; }

    public string ExternalTransactionId { get; set; }

    public Guid Id { get; set; }

    public decimal LockAmount { get; set; }

    public decimal LockedBalance { get; set; }

    public string PlayerCode { get; set; }

    public DateTimeOffset RecordedOn { get; set; }

    public DateTimeOffset? SentOn { get; set; }

    public TransactionType Type { get; set; }

    public virtual Wallet Wallet { get; set; }

    public Guid WalletId { get; set; }
}
