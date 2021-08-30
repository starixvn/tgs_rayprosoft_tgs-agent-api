using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TCG.Common
{
    public record TCGSingleWalletResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("error_desc")]
        public string ErrorDesc { get; set; }
    }
}
