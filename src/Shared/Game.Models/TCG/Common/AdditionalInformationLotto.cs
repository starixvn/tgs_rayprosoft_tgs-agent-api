using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TCG.Common
{
    public record AdditionalInformationLotto
    {
        [JsonPropertyName("order_no")]
        public string OrderNo { get; init; }
    }
}
