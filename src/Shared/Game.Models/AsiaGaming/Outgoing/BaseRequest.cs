namespace TGS.Shared.Game.Models.AsiaGaming.Outgoing
{
    public abstract record BaseRequest
    {
        public string cagent { get; init; }

        public string loginname { get; init; }

        public int actype { get; } = 1;

        public string password { get; } = "password";

        public string oddtype { get; init; }

        public string cur { get; init; }
    }
}