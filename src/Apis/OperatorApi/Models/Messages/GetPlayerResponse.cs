using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record GetPlayerResponse : TgsResponse
    {
        [JsonPropertyName("data")]
        public PlayerData Data { get; init; }
    }
}