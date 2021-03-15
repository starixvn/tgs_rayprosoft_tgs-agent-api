namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record ForgotPasswordRequest
    {
        public string Phone { get; set; }
    }
}
