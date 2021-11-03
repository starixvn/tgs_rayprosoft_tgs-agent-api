using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Outgoing
{
    public record CreateAgentRequest : BaseRequest
    {
        [JsonPropertyName("Username")]
        public string AgentId { get; init; }

        [JsonPropertyName("CasinoTableLimit")]
        public int CasinoTableBetLimit => 1;

        [JsonPropertyName("Currency")]
        public string CurrencyCode { get; init; }

        [JsonPropertyName("Max")]
        public int MaxBet => 5000;

        [JsonPropertyName("MaxPerMatch")]
        public int MaxBetPerMatch => 20000;

        [JsonPropertyName("Min")]
        public int MinBet => 1;

        [JsonPropertyName("Password")]
        public string Password => "TgsPassw0rd";
    }
}