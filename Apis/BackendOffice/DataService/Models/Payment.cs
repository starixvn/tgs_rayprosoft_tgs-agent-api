using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.DataService.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid PaymentVendorId { get; set; }
        public Guid PaymentMethodId { get; set; }
        public Guid PaymentChannelId { get; set; }
        public string ChannelName { get; set; }
        public Status Status { get; set; }
        public decimal TransferLimitFrom { get; set; }
        public decimal TransferLimitTo { get; set; }
        public TransferCondition TransferCondition { get; set; }
        public bool AllPlayer { get; set; }
        public bool IdentityVerification { get; set; }
        public bool PhoneVerification { get; set; }
        public decimal DepositedAmountFrom { get; set; }
        public decimal DepositedAmountTo { get; set; }
        public string Reason { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
        public ICollection<UserPaymentAccessControl> UserPaymentAccessControls { get; set; }
    }
}