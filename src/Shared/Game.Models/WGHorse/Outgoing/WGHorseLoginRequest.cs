namespace TGS.Shared.Game.Models.WGHorse.Outgoing
{
    public record WGHorseLoginRequest
    {
        public string Token { get; init; }
        public string Lang { get; init; }
    }
}
