using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.JDB.Outgoing
{
    public class BaseResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }
        [JsonPropertyName("err_text")]
        public string ErrorMessage { get; set; }
    }
}