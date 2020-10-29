using System;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Payment
{
    public class PaymentRequest
    {
        public Guid Id { get; set; }
        public Guid PaymentVendorId { get; set; }
        public Guid PaymentMethodId { get; set; }
        public Guid PaymentChannelId { get; set; }
        public string ChannelName { get; set; }
        public decimal TransferLimitFrom { get; set; }
        public decimal TransferLimitTo { get; set; }
        public MeetTransferCondition TransferCondition { get; set; }
        public bool AllPlayer { get; set; }
        public bool IdentityVerification { get; set; }
        public bool PhoneVerification { get; set; }
        public bool DesignatedUser { get; set; }
        public bool DepositedAmountUnlimited { get; set; }
        public decimal DepositedAmountFrom { get; set; }
        public decimal DepositedAmountTo { get; set; }
        public bool Enabled { get; set; }
        public string Reason { get; set; }
        public PaymentHandlingFeeModel PaymentHandlingFee { get; set; }
        public string[] Whitelist { get; set; }
        public string[] Blacklist { get; set; }
        public Guid[] VipLevelIds { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
    }
}