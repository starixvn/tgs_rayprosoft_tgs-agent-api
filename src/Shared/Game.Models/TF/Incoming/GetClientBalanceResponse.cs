using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TF.Incoming;

public record GetClientBalanceResponse
{
    [JsonPropertyName("balance")]
    public decimal Balance { get; set; }
}
