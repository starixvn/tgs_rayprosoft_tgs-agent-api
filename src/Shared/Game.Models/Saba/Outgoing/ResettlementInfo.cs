using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Outgoing
{
    public record ResettlementInfo
    {
        [JsonPropertyName("winlost")]
        public decimal Winlost { get; init; }

        [JsonPropertyName("ActionDate")]
        public string ActionDate { get; init; }

        [JsonPropertyName("ticket_status")]
        public string TicketStatus { get; init; }

        [JsonPropertyName("balancechange")]
        public bool BalanceChange { get; init; }
    }
}