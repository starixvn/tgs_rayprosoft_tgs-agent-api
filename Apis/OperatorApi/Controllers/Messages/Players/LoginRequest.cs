namespace TGS.Apis.OperatorApi.Controllers.Messages.Players
{
    public class LoginRequest : BaseRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
