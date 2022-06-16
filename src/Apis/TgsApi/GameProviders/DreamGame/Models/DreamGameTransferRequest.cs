using System.Text.Json.Serialization;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Models.Commons;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Models;

public record DreamGameTransferRequest : DreamGameRequest
{
    [JsonPropertyName("ticketId")]
    public long TicketId { get; init; }
    [JsonPropertyName("data")]
    public string Data { get; init; }
    [JsonPropertyName("member")]
    public MemberInfo Member { get; init; }
}
