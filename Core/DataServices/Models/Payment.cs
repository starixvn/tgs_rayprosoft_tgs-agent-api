using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid PaymentVendorId { get; set; }
        public virtual PaymentVendor PaymentVendor { get; set; }
        public Guid PaymentMethodId { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public Guid PaymentChannelId { get; set; }
        public virtual PaymentChannel PaymentChannel { get; set; }
        public string ChannelName { get; set; }
        public bool Enabled { get; set; }
        public decimal TransferLimitFrom { get; set; }
        public decimal TransferLimitTo { get; set; }
        public MeetTransferCondition TransferCondition { get; set; }
        public bool AllPlayer { get; set; }
        public bool IdentityVerification { get; set; }
        public bool PhoneVerification { get; set; }
        public bool DepositedAmountUnlimited { get; set; }
        public decimal DepositedAmountFrom { get; set; }
        public decimal DepositedAmountTo { get; set; }
        public string Reason { get; set; }
        public decimal? HandlingFeePercentage { get; set; }
        public decimal? LowestHandlingFee { get; set; }
        public decimal? HighestHandlingFee { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
        public virtual ICollection<PaymentUserAccessControl> PaymentUserAccessControls { get; set; }
        public virtual ICollection<PaymentVipLevel> VipLevels { get; set; }
    }
}