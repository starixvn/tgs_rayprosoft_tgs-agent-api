using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Messages
{
    public record TgsResponse
    {
        [JsonPropertyName("err")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Error { get; init; } = (int) TgsApiError.None;

        [JsonPropertyName("errmsg")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ErrorMessage { get; init; }
    }
}
