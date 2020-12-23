using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record GetGamesResponse : TgsResponse
    {
        [JsonPropertyName("games")]
        public GameModel[] Games { get; init; }

        public record GameModel
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
            public int Id { get; init; }

            [JsonPropertyName("code")]
            public string Code { get; init; }

            [JsonPropertyName("name")]
            public string Name { get; init; }

            [JsonPropertyName("description")]
            public string Description { get; init; }

            [JsonPropertyName("gameprovidercode")]
            public string GameProviderCode { get; init; }

            [JsonPropertyName("isactive")]
            public bool IsActive { get; init; }

            [JsonPropertyName("type")]
            public GameType Type { get; init; }

            [JsonPropertyName("tag")]
            public Tag Tag {get; init;}
        }

        public record Tag
        {
            [JsonPropertyName("new")]
            public bool New { get; init;}

            [JsonPropertyName("hot")]
            public bool Hot { get; init;}

            [JsonPropertyName("recommend")]
            public bool Recommend { get; init;}
        }
    }
}
