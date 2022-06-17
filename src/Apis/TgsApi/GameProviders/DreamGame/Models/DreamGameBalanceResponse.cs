using System.Text.Json.Serialization;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Models.Commons;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Models;

public record DreamGameBalanceResponse : DreamGameResponse
{
    [JsonPropertyName("member")]
    public MemberInfo Member { get; init; }
}
