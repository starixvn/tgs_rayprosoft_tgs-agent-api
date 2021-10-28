using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.LY.Commons
{
    public record LYBaseRequest
    {
        /// <summary>
        /// The action
        /// </summary>
        [JsonPropertyName("s")]
        public int Action { get; set; }
    }
}
