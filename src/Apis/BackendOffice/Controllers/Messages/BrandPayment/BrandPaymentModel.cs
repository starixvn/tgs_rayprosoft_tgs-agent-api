using System;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.Controllers.Messages.BrandPayment
{
    public class BrandPaymentModel
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public Guid? PaymentChannelId { get; set; }
        public int Number { get; set; }
        public string PaymentName { get; set; }
        public string PaymentMethodName { get; set; }
        public string PaymentChannelName { get; set; }
        public string ChannelName { get; set; }
        public decimal TransferLimitFrom { get; set; }
        public decimal TransferLimitTo { get; set; }
        public PaymentTransferType TransferType { get; set; }
        public PlayerListCondition PlayerListCondition { get; set; }
        public bool AllPlayers { get; set; }
        public bool IdentityAuthentication { get; set; }
        public bool PhoneAuthentication { get; set; }
        public bool DesignatedUsers { get; set; }
        public bool DepositedAmountUnlimited { get; set; }
        public decimal DepositedAmountMin { get; set; }
        public decimal DepositedAmountMax { get; set; }
        public SwitchStatus PaymentStatus { get; set; }
        public string Reason { get; set; }
        public PaymentFeeModel PaymentFee { get; set; }
        public string[] Whitelist { get; set; }
        public string[] Blacklist { get; set; }
        public Guid[] VipLevelIds { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
    }
}