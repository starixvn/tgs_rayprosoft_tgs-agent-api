using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Habanero.Incoming
{
    public record AuthorizePlayerResponse
    {
        [JsonPropertyName("playerdetailresponse")]
        public AuthorizeResult Result { get; init; }
    }

    public record AuthorizeResult
    {
        [JsonPropertyName("status")]
        public Status Status { get; init; }

        [JsonPropertyName("accountid")]
        [JsonIgnore(Condition =  JsonIgnoreCondition.WhenWritingDefault)]
        public string PlayerCode { get; init; }

        [JsonPropertyName("accountname")]
        [JsonIgnore(Condition =  JsonIgnoreCondition.WhenWritingDefault)]
        public string PlayerUsername { get; init; }

        [JsonPropertyName("balance")]
        [JsonIgnore(Condition =  JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Balance { get; init; }

        [JsonPropertyName("currencycode")]
        [JsonIgnore(Condition =  JsonIgnoreCondition.WhenWritingDefault)]
        public string CurrencyCode { get; init; }
    }
}
