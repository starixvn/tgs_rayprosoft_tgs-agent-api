namespace TGS.Apis.OperatorApi.Controllers.Messages.Players
{
    public class ForgotPasswordRequest : BaseRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
