using System;
using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Models.Messages.GameProviders
{
    public record GetGameProvidersResponse : TgsResponse
    {
        [JsonPropertyName("gameproviders")]
        public GameProviderModel[] GameProviders { get; init; }

        public record GameProviderModel
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
            public Guid Id { get; init; }

            [JsonPropertyName("code")]
            public string Code { get; init; }

            [JsonPropertyName("name")]
            public string Name { get; init; }

            [JsonPropertyName("description")]
            public string Description { get; init; }

            [JsonPropertyName("status")]
            public Status Status { get; init; }
        }
    }
}