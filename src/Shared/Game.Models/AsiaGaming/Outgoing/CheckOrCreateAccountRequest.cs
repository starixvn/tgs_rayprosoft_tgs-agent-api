namespace TGS.Shared.Game.Models.AsiaGaming.Outgoing
{
    public record CheckOrCreateAccountRequest : BaseRequest
    {
        public string method { get; } = "lg";
    }
}