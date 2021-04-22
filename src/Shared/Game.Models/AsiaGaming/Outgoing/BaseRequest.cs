namespace TGS.Shared.Game.Models.AsiaGaming.Outgoing
{
    public class BaseRequest
    {
        public string cagent { get; set; }

        public string loginname { get; set; }

        public int actype { get; } = 1;

        public string password { get; } = "password";

        public string oddtype { get; set; }

        public string cur { get; set; }
    }
}