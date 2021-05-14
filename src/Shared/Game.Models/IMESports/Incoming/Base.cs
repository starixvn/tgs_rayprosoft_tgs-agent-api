using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.IMESports.Enums;

namespace TGS.Shared.Game.Models.IMESports.Incoming
{
    public record Base
    {
        [JsonPropertyName("ProductWallet")]
        public string ProductWallet { get; init; }
    }
}