namespace TGS.Apis.BackendOffice.Controllers.Messages.Players
{
    public class PlayersModel
    {
        public string PlayerCode { get; set; }
        public string Username { get; set; }
        public int VipLevel { get; set; }
        public string Status { get; set; }
        public bool HasAgent { get; set; }
    }
}