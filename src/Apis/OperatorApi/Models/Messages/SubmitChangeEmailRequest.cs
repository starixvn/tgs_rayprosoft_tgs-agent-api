namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record SubmitChangeEmailRequest
    {
        public string Token { get; init; }

        public string ValidateCode { get; init; }
    }
}
