using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TF.Incoming
{
    public class TransferRequest
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("loginName")]
        public string LoginName { get; set; }

        [JsonPropertyName("placeBet")]
        public bool PlaceBet { get; set; }

        [JsonPropertyName("ticketDetail")]
        public TicketObject TicketDetail { get; set; }

        [JsonPropertyName("ticketNum")]
        public string TicketNum { get; set; }
    }
}
