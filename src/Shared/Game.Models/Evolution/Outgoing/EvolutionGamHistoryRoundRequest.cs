namespace TGS.Shared.Game.Models.Evolution.Outgoing
{
    public record EvolutionGamHistoryRoundRequest
    {
        public string PlayerCode { get; set; }
        public string RoundId { get; set; }
    }
}
