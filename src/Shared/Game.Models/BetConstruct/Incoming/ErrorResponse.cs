using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BetConstruct.Incoming
{
    public record ErrorResponse
    {
        /// <summary>
        /// See Error Codes for valid values. Mandatory in  case of error.
        /// </summary>
        [JsonPropertyName("ErrorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Error description
        /// </summary>
        [JsonPropertyName("ErrorText")]
        public string ErrorText { get; set; }
    }
}
