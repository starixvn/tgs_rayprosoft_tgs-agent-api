using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record CreatePlayerResponse : TgsResponse
    {
        [JsonPropertyName("isnew")]
        public bool IsNew { get; set; }

        [JsonPropertyName("playercode")]
        public string PlayerCode { get; set; }
    }
}
