using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.JDB.Outgoing
{
    public class BaseActionResponse : BaseResponse
    {
        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }
    }
}