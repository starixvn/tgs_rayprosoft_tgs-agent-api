namespace TGS.Apis.BackendOffice.Controllers.Messages
{
    public record DepositReviewRequest
    {
        public string Remark { get; init; }

        public ReviewResponses ReviewResponse { get; init; }
    }
}
