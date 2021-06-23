using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record GetPromotionsRequest : PagingRequest
    {
        [JsonPropertyName("language")]
        public string Language { get; set; }
    }
}