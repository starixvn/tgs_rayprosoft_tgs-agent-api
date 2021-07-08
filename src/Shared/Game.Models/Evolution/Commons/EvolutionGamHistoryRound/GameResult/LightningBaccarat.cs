using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.Commons;

namespace TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.GameResult
{
    public record LightningBaccarat
    {
        [JsonPropertyName("banker")]
        public Card Banker { get; set; }
        [JsonPropertyName("multipliers")]
        public Multipliers Multipliers { get; set; }
        [JsonPropertyName("outcome")]
        public string Outcome { get; set; }
        [JsonPropertyName("player")]
        public Card Player { get; set; }
    }

    public class Multipliers
    {
        [JsonPropertyName("betCodes")]
        public Betcodes BetCodes { get; set; }
        [JsonPropertyName("cards")]
        public MultipliersCards Cards { get; set; }
    }

    public record MultipliersCards
    {
        [JsonPropertyName("2D")]
        public int _2D { get; set; }
        [JsonPropertyName("5S")]
        public int _5S { get; set; }
        [JsonPropertyName("JC")]
        public int JC { get; set; }
        [JsonPropertyName("JC")]
        public int QC { get; set; }
    }

    public class Betcodes
    {
        [JsonPropertyName("BAC_Banker")]
        public int BAC_Banker { get; set; }
        [JsonPropertyName("BAC_Banker")]
        public int BAC_BankerPair { get; set; }
        [JsonPropertyName("BAC_Player")]
        public int BAC_Player { get; set; }
        [JsonPropertyName("BAC_PlayerPair")]
        public int BAC_PlayerPair { get; set; }
        [JsonPropertyName("BAC_Tie")]
        public int BAC_Tie { get; set; }
    }
}