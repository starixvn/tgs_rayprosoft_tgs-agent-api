namespace TGS.Apis.AgentApi.Models.Messages
{
    public record ForgotPasswordRequest
    {
        public string Email { get; set; }
    }
}