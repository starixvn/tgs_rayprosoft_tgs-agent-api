using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Incoming
{
    public record Base
    {
        [JsonPropertyName("ProductWallet")]
        public string ProductWallet { get; init; }
    }
}