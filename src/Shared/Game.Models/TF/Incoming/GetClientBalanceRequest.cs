using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TF.Incoming
{
    public record GetClientBalanceRequest
    {
        [JsonPropertyName("loginName")]
        public string LoginName { get; set; }
    }
}
