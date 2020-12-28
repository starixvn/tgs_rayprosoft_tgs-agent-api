using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.GlobalApi.Models.Messages
{
    public record PagedResponse<T> : TgsResponse where T : class
    {
        [JsonPropertyName("totalcount")]
        public int TotalCount { get; init; }

        [JsonPropertyName("items")]
        public T[] Items { get; init; }
    }
}
