namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record UpdatePlayerRequest : BaseRequest
    {
        public string RealName { get; init; }
        public string Phone { get; init; }
        public string Email { get; init; }
        public string QQ { get; init; }
        public string Wechat { get; init; }
    }
}
