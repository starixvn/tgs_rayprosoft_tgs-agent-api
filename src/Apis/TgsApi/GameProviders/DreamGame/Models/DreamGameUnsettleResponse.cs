using System.Text.Json.Serialization;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Models.Commons;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Models;

public record DreamGameUnsettleResponse : DreamGameResponse
{
    [JsonPropertyName("list")]
    public OrderInfo[] List { get; init; }
}
