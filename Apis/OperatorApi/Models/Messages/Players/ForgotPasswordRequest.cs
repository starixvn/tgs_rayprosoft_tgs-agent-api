namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public class ForgotPasswordRequest : BaseRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
