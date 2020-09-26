namespace TGS.Apis.OperatorApi.Controllers.Messages.Players
{
    public record CreatePlayerRequest
    {
        public string Answer { get; init; }
        public string Captcha { get; init; }
        public string Username { get; init; }
        public string Password { get; init; }
        public string ConfirmPassword { get; init; }
        public string ParentInvitationCode { get; init; }
    }
}
