using System.Text.Json.Serialization;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Models.Commons;

public record OrderInfo
{
    [JsonPropertyName("username")]
    public string Username { get; init; }
    [JsonPropertyName("ticketId")]
    public long TicketId { get; init; }
    [JsonPropertyName("serial")]
    public string Serial { get; init; }
    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }
}
