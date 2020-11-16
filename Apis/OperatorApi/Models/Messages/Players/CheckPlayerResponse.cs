using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record CheckPlayerResponse : TgsResponse
    {
        [JsonPropertyName("isexist")]
        public bool Exists { get; init; }
    }
}