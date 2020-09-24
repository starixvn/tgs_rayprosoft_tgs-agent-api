namespace TGS.Apis.OperatorApi.Controllers.Messages.Players
{
    public class UpdatePlayerRequest : BaseRequest
    {
        public string RealName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string QQ { get; set; }
        public string Wechat { get; set; }
    }
}
