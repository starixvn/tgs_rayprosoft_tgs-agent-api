namespace TGS.Shared.Game.Models.AsiaGaming.Outgoing
{
    public class GameUrlRequest : BaseRequest
    {
        public string dm { get; set; }

        public string sid { get; set; }

        public string mh5 { get; set; }

        public string lang { get; set; }

        public string gameType { get; set; }
    }
}