namespace TGS.Shared.Game.Models.AsiaGaming.Outgoing
{
    public record GameUrlRequest : BaseRequest
    {
        public string dm { get; init; }

        public string sid { get; init; }

        public string mh5 { get; init; }

        public string lang { get; init; }

        public string gameType { get; init; }
    }
}