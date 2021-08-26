using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Outgoing
{
    public record LoginGameRequest : BaseRequest
    {
        public override string Action => "LoginGame";

        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("password")]
        public string Password => "TgsP@ssword";

        [JsonPropertyName("lang")]
        public string Language { get; init; }
    }
}