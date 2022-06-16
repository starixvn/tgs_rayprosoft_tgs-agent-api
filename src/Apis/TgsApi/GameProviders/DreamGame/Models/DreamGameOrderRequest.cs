using System.Text.Json.Serialization;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Models.Commons;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Models;

public record DreamGameOrderRequest : DreamGameRequest
{
    [JsonPropertyName("ticketId")]
    public long TicketId { get; init; }
}
