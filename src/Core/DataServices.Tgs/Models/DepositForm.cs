using System;

namespace TGS.Core.DataServices.Tgs.Models;

public class DepositForm
{
    public decimal? ActualAmount { get; set; }

    public decimal Amount { get; set; }

    public string ApplyInfo { get; set; }

    public virtual Brand Brand { get; set; }

    public Guid BrandId { get; set; }

    public virtual BrandPayment BrandPayment { get; set; }

    public Guid BrandPaymentId { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public decimal HandlingFee { get; set; }

    public Guid Id { get; set; }

    public string OrderNo { get; set; }

    public string PaymentReturn { get; set; }

    public virtual Player Player { get; set; }

    public string PlayerCode { get; set; }

    public Guid? PromotionId { get; set; }

    public int? PromotionPlayerId { get; set; }

    public string Remark { get; set; }

    public DateTimeOffset? ReviewedOn { get; set; }

    public string ReviewedUserId { get; set; }

    public string ReviewedUserName { get; set; }

    public int Status { get; set; }

    public string ThirdPartyOrderNo { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }
}
