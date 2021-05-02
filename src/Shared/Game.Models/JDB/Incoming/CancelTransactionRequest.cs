// unset

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.JDB.Incoming
{
    public record CancelTransactionRequest : BaseActionRequest
    {
        [JsonPropertyName("transferId")]
        public string TransferId { get; set; }
    }
}