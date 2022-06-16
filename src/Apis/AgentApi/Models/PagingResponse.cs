using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models;

public class PagingResponse<T>
{
    [JsonPropertyName("items")]
    public IEnumerable<T> Items { get; init; }

    [JsonPropertyName("totalcount")]
    public int TotalCount { get; init; }
}
