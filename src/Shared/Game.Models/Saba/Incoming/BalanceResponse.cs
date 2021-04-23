using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public abstract record BalanceResponse : BaseResponse
    {
        [JsonPropertyName("balance")]
        public decimal Balance { get; init; }
    }
}