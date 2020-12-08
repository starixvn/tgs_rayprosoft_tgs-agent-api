namespace TGS.Core.Models.Messages.Pluggables
{
    public record PlayerKickRequest
    {
        public string GameProviderCode { get; init; }
        public string PlayerCode { get; init; }
    }
}