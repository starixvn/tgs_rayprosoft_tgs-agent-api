namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record EndRoundRequest : BaseRequest
    {
        public string UserId { get; set; }

        public string GameId { get; set; }

        public string RoundId { get; set; }
    }
}