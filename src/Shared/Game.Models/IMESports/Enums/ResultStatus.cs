using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResultStatus
    {
        Win = 1,
        Lose = 2,
        Draw = 3
    }
}
