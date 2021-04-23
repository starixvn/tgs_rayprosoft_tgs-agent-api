using System;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.Controllers.Messages.BrandPayment
{
    public class BrandPaymentRequest
    {
        public Guid Id { get; set; }
        public Guid PaymentChannelId { get; set; }
        public SwitchStatus PaymentStatus { get; set; }
        public string ChannelName { get; set; }
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
        public string[] Whitelist { get; set; }
        public string[] Blacklist { get; set; }
        public Guid[] VipLevelIds { get; set; }
    }
}