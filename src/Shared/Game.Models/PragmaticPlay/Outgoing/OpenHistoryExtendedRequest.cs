using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.PragmaticPlay.Common;

namespace TGS.Shared.Game.Models.PragmaticPlay.Outgoing;

public record OpenHistoryExtendedRequest : HashRequest
{
    [JsonPropertyName("language")]
    public string Language { get; init; }

    [JsonPropertyName("playerId")]
    public string PlayerCode { get; init; }

    [JsonPropertyName("roundId")]
    public string RoundId { get; init; }

    [JsonPropertyName("secureLogin")]
    public string SecureLogin { get; init; }
}