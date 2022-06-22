using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models;

public class AgentTransaction
{
    public Guid AgentId { get; set; }

    public decimal Amount { get; set; }

    public decimal Balance { get; set; }

    public virtual Brand Brand { get; set; }

    public Guid BrandId { get; set; }

    public string Description { get; set; }

    public string ExternalTransactionId { get; set; }

    public Guid Id { get; set; }

    public decimal LockAmount { get; set; }

    public decimal LockedBalance { get; set; }

    public DateTimeOffset RecordedOn { get; set; }

    public DateTimeOffset? SentOn { get; set; }

    public AgentTransactionType Type { get; set; }

    public virtual AgentWallet Wallet { get; set; }

    public Guid WalletId { get; set; }
}
