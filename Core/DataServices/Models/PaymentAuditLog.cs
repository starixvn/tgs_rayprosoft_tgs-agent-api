using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class PaymentAuditLog
    {
        public Guid Id { get; set; }
        public string ChannelName { get; set; }
        public bool Enabled { get; set; }
        public decimal TransferLimitFrom { get; set; }
        public decimal TransferLimitTo { get; set; }
        public PlayerListCondition PlayerListCondition { get; set; }
        public bool AllPlayer { get; set; }
        public bool IdentityVerification { get; set; }
        public bool PhoneVerification { get; set; }
        public bool DepositedAmountUnlimited { get; set; }
        public decimal DepositedAmountFrom { get; set; }
        public decimal DepositedAmountTo { get; set; }
        public string Reason { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public AuditLogOperation Operation { get; set; }
    }
}