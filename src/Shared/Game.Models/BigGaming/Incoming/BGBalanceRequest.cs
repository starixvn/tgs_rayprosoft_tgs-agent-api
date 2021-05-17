using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.BigGaming.Common;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BGBalanceRequest : BGSignParamsRequest
    {
        [JsonPropertyName("loginId")]
        public string LoginId { get; set; }
    }
}
