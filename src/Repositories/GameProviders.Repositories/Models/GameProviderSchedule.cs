using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TGS.GameProviders.Repositories.Models
{
    public class GameProviderSchedule
    {
        [BsonElement("agent_id")]
        public string AgentId { get; init; }

        [BsonElement("brand_code")]
        public string BrandCode { get; init; }

        [BsonElement("config")]
        public string Config { get; set; }

        [BsonElement("created_on")]
        public DateTimeOffset CreatedOn { get; set; }

        [BsonElement("currency_code")]
        public string CurrencyCode { get; set; }

        [BsonElement("game_provider_code")]
        public string GameProviderCode { get; init; }

        [BsonElement("game_provider_id")]
        public Guid GameProviderId { get; init; }

        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("updated_on")]
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
