using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Messages
{
    public record TgsResponse
    {
        [JsonPropertyName("err")]
        public int Error { get; init; } = (int) TgsApiError.None;

        [JsonPropertyName("errmsg")]
        public string ErrorMessage { get; init; } = string.Empty;
    }
}
