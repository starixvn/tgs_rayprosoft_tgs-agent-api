using TGS.Services.Payment;

namespace TGS.Apis.BackendOffice.Controllers.Messages
{
    public class ReviewRequest
    {
        public string Remark { get; init; }

        public ReviewStatus ReviewResponse { get; init; }
    }
}
