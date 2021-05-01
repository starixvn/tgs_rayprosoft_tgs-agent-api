using System;

namespace TGS.Services.Payment
{
    public record WithdrawFormInfo(
        string PlayerCode,
        Guid BrandId,
        decimal Amount,
        int Status,
        Guid BankInfoId,
        Guid? BrandPaymentId = null,
        string? BankInfo = null,
        string? ApplyInfo = null,
        string? PaymentReturn = null)
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public string ChannelName { get; set; }
        public string Remark { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
