using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameProviders.Repositories.Models
{
    public class GameProviderSchedule
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("game_provider_id")]
        public Guid GameProviderId { get; init; }

        [BsonElement("game_provider_code")]
        public string GameProviderCode { get; init; }
        [BsonElement("currency_code")]
        public string CurrencyCode { get; set; }

        [BsonElement("config")]
        public string Config { get; set; }

        [BsonElement("created_on")]
        public DateTimeOffset CreatedOn { get; set; }

        [BsonElement("updated_on")]
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
