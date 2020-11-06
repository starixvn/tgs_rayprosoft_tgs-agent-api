using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class PaymentAuditLog
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public string BeforeChannelName { get; set; }
        public string AfterChannelName { get; set; }
        public PaymentStatus BeforePaymentStatus { get; set; }
        public PaymentStatus AfterPaymentStatus { get; set; }
        public decimal BeforeTransferLimitFrom { get; set; }
        public decimal AfterTransferLimitFrom { get; set; }
        public decimal BeforeTransferLimitTo { get; set; }
        public decimal AfterTransferLimitTo { get; set; }
        public PlayerListCondition BeforePlayerListCondition { get; set; }
        public PlayerListCondition AfterPlayerListCondition { get; set; }
        public bool BeforeAllPlayer { get; set; }
        public bool AfterAllPlayer { get; set; }
        public bool BeforeIdentityVerification { get; set; }
        public bool AfterIdentityVerification { get; set; }
        public bool BeforePhoneVerification { get; set; }
        public bool AfterPhoneVerification { get; set; }
        public bool BeforeDepositedAmountUnlimited { get; set; }
        public bool AfterDepositedAmountUnlimited { get; set; }
        public decimal BeforeDepositedAmountFrom { get; set; }
        public decimal AfterDepositedAmountFrom { get; set; }
        public decimal BeforeDepositedAmountTo { get; set; }
        public decimal AfterDepositedAmountTo { get; set; }
        public string Reason { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public AuditLogOperation Operation { get; set; }
    }
}