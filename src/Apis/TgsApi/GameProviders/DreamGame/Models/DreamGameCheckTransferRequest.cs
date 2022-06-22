using System.Text.Json.Serialization;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Models.Commons;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Models;

public record DreamGameCheckTransferRequest : DreamGameRequest
{
    [JsonPropertyName("data")]
    public string Data { get; init; }
}
