using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Models.Messages.Players 
{
    public record GetPlayerResponse : TgsResponse
    {
        [JsonPropertyName("data")]
        public PlayerData Data { get; init; }

        public record PlayerData
        {
            [JsonPropertyName("username")]
            public string Username { get; init; }

            [JsonPropertyName("realname")]
            public string RealName { get; init; }

            [JsonPropertyName("phone")]
            public string Phone { get; init; }

            [JsonPropertyName("email")]
            public string Email { get; init; }

            [JsonPropertyName("qq")]
            public string QQ { get; init; }

            [JsonPropertyName("wechat")]
            public string Wechat { get; init; }
        }
    }
}
