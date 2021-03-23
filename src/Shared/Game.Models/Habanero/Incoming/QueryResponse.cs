using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Habanero.Incoming
{
    public record QueryResponse
    {
        [JsonPropertyName("fundtransferresponse")]
        public QueryResult Result { get; set; }
    }

    public record QueryResult
    {
        [JsonPropertyName("status")]
        public Status Status { get; set; }
    }
}