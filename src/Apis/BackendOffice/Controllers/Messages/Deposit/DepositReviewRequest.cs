using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages
{
    public record DepositReviewRequest
    {
        public Guid BrandId { get; init; }

        public string Remark { get; init; }

        public ReviewResponseState ReviewResponse { get; init; }
    }
}
