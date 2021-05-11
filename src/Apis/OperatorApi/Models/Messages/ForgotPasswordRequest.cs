namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record ForgotPasswordRequest
    {
        public string Email { get; set; }
    }
}