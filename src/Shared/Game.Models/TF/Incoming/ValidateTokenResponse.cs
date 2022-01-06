using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TF.Incoming
{
    public class ValidateTokenResponse
    {
        [JsonPropertyName("loginName")]
        public string LoginName { get; set; }
    }
}
