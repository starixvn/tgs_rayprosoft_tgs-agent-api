using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record RollbackRequest : BaseRequest, IRequest<RollbackResponse>
    {
        [JsonPropertyName("transferCode")]
        public string ExternalTransactionId { get; set; }
    }
}