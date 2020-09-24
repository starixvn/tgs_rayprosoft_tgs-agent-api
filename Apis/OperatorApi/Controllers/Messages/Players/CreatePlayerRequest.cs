namespace TGS.Apis.OperatorApi.Controllers.Messages.Players
{
    public class CreatePlayerRequest : BaseRequest
    {
        public string Answer { get; set; }
        public string Captcha { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string InvitationCode { get; set; }
    }
}
