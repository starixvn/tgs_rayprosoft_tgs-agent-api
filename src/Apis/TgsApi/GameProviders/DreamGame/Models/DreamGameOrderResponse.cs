using System.Text.Json.Serialization;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Models.Commons;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Models;

public record DreamGameOrderResponse : DreamGameResponse
{
    [JsonPropertyName("ticketId")]
    public long TicketId { get; init; }
    [JsonPropertyName("list")]
    public OrderInfo[] List { get; init; }
}
