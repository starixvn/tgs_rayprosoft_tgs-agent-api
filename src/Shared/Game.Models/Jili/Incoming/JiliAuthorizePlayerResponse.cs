using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Jili.Incoming;

public record JiliAuthorizePlayerResponse : JiliBaseResponse
{
    [JsonPropertyName("balance")]
    public decimal Balance { get; init; }

    [JsonPropertyName("currency")]
    public string CurrencyCode { get; init; }

    [JsonPropertyName("username")]
    public string JiliPlayerId { get; init; }
}
