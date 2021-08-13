using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Incoming
{
    public record GetBalanceRequest : BaseRequest
    {
        [JsonPropertyName("user")]
        public string PlayerCode { get; set; }
    }
}