using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class BrandPayment
    {
        public Guid Id { get; set; }
        public int DisplayOrder { get; set; }
        public Guid PaymentId { get; set; }
        public virtual Payment Payment { get; set; }
        public Guid PaymentMethodId { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public Guid PaymentChannelId { get; set; }
        public virtual PaymentChannel PaymentChannel { get; set; }
        public PaymentTransferType TransferType { get; set; }
        public string ChannelName { get; set; }
        public SwitchStatus PaymentStatus { get; set; }
        public decimal TransferLimitFrom { get; set; }
        public decimal TransferLimitTo { get; set; }
        public PlayerListCondition PlayerListCondition { get; set; }
        public bool AllPlayers { get; set; }
        public bool IdentityAuthentication { get; set; }
        public bool PhoneAuthentication { get; set; }
        public bool DepositedAmountUnlimited { get; set; }
        public decimal DepositedAmountMin { get; set; }
        public decimal DepositedAmountMax { get; set; }
        public string Reason { get; set; }
        public decimal? FeePercentage { get; set; }
        public decimal? LowestFee { get; set; }
        public decimal? HighestFee { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
        public virtual ICollection<PaymentPlayerList> PaymentPlayerList { get; set; }
        public virtual ICollection<PaymentVipLevel> VipLevels { get; set; }
    }
}