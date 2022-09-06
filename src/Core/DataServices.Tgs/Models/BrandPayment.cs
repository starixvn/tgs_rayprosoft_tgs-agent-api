using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models;

public class BrandPayment
{
    public bool AllPlayers { get; set; }

    public virtual Brand Brand { get; set; }

    public Guid BrandId { get; set; }

    public virtual ICollection<BrandPaymentPlayer> BrandPaymentPlayers { get; set; }

    public string ChannelName { get; set; }

    public string CreatedByUsername { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public decimal DepositedAmountMax { get; set; }

    public decimal DepositedAmountMin { get; set; }

    public bool DepositedAmountUnlimited { get; set; }

    public int DisplayOrder { get; set; }

    public string ExtendedData { get; set; }

    public decimal HandlingFeeRateTenPercentile { get; set; }

    public Guid Id { get; set; }

    public bool IdentityAuthentication { get; set; }

    public virtual PaymentMethodChannel PaymentMethodChannel { get; set; }

    public Guid PaymentMethodChannelId { get; set; }

    public SwitchStatus PaymentStatus { get; set; }

    public bool PhoneAuthentication { get; set; }

    public PlayerListCondition PlayerListCondition { get; set; }

    public string Reason { get; set; }

    public bool ReducePlayerAmount { get; set; }

    public decimal TransferLimitFrom { get; set; }

    public decimal TransferLimitTo { get; set; }

    public PaymentTransferType TransferType { get; set; }

    public string UpdatedByUsername { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }

    public virtual ICollection<BrandPaymentVipLevel> VipLevels { get; set; }
}
