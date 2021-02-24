using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Habanero.Incoming
{
    public record AuthorizePlayerResponse
    {
        [JsonPropertyName("playerdetailresponse")]
        public AuthorizeResult Result { get; set; }
    }

    public record AuthorizeResult
    {
        [JsonPropertyName("status")]
        public Status Status { get; set; }

        [JsonPropertyName("accountid")]
        public Guid PlayerId { get; set; }

        [JsonPropertyName("accountname")]
        public string PlayerUsername { get; set; }

        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }

        [JsonPropertyName("currencycode")]
        public string CurrencyCode { get; set; }
    }
}
