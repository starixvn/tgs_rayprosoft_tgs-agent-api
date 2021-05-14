using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Services.Operator
{
    public record GameBalanceInfo : TgsResponse
    {
        [JsonPropertyName("balance")]
        public decimal Balance { get; init; }
    }
}