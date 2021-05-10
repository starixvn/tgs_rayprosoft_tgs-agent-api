using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record GetBalanceRequest : BaseRequest
    {
        [JsonPropertyName("message")]
        public Data Body { get; init; }

        public record Data
        {
            [JsonPropertyName("action")]
            public string Action { get; init; }

            [JsonPropertyName("userId")]
            public string PlayerCode { get; init; }
        }
    }
}