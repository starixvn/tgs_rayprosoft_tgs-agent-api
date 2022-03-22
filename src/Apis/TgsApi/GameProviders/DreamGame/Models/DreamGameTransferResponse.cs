using System.Text.Json.Serialization;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Models.Commons;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Models
{
    public record DreamGameTransferResponse : DreamGameResponse
    {
        [JsonPropertyName("data")]
        public string Data { get; init; }
        [JsonPropertyName("member")]
        public MemberInfo Member { get; init; }
    }
}
