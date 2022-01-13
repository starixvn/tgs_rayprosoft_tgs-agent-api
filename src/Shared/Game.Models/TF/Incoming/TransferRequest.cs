using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TF.Incoming
{
    public class TransferRequest
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("description")]
        public string Description { get; init; }

        [JsonPropertyName("loginName")]
        public string LoginName { get; init; }

        [JsonPropertyName("placeBet")]
        public bool PlaceBet { get; init; }

        [JsonPropertyName("ticketDetail")]
        public TicketObject TicketDetail { get; init; }

        [JsonPropertyName("ticketNum")]
        public string TicketNum { get; init; }
    }
}
