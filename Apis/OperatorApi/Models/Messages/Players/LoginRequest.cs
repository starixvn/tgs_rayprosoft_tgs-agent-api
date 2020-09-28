namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public class LoginRequest : BaseRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
