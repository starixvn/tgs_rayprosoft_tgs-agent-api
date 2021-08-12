namespace TGS.Shared.Game.Models.WGHorse.Outgoing
{
    public record WGHorseGetBalanceRequest
    {
        public string PlayerCode { get; init; }
    }
}
