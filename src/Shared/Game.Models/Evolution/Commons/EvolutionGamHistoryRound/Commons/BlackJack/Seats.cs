using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons.BlackJack
{
    public record Seats
    {
        [JsonPropertyName("Seat1")]
        public Seat Seat1 { get; set; }
        [JsonPropertyName("Seat2")]
        public Seat Seat2 { get; set; }
        [JsonPropertyName("Seat3")]
        public Seat Seat3 { get; set; }
        [JsonPropertyName("Seat4")]
        public Seat Seat4 { get; set; }
        [JsonPropertyName("Seat5")]
        public Seat Seat5 { get; set; }
        [JsonPropertyName("Seat6")]
        public Seat Seat6 { get; set; }
        [JsonPropertyName("Seat7")]
        public Seat Seat7 { get; set; }
    }
}
