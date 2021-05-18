using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OutcomeType
    {
        Missed = 0,
        Hit = 1
    }
}
