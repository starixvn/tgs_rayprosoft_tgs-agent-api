using System;
using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Models.Messages.Games
{
    public record GetGamesResponse : TgsResponse
    {
        [JsonPropertyName("games")]
        public GameModel[] Games { get; init; }

        public record GameModel
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
            public Guid Id { get; set; }

            [JsonPropertyName("code")]
            public string Code { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("gameprovidercode")]
            public string GameProviderCode { get; set; }

            [JsonPropertyName("isactive")]
            public bool IsActive { get; set; }

            [JsonPropertyName("type")]
            public GameType Type { get; set; }
        }
    }
}
