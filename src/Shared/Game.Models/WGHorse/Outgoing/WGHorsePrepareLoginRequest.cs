namespace TGS.Shared.Game.Models.WGHorse.Outgoing
{
    public record WGHorsePrepareLoginRequest
    {
        public long Timestamp { get; init; }
        public string PlayerCode { get; init; }
        public string PlayerNickname { get; init; }
        public string CurrencyCode { get; init; }
    }
}
