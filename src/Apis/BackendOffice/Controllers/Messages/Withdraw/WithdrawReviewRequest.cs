using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Withdraw
{
    public record WithdrawReviewRequest
    {
        public Guid BrandPaymentId { get; init; }

        public string Remark { get; init; }

        public ReviewResponseState ReviewResponse { get; init; }
    }
}
