using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Enums
{
    [JsonConverter(typeof(ResultStatusConverter))]
    public enum ResultStatus
    {
        None,
        Unknown,
        Win = 1,
        Lose = 2,
        Draw = 3
    }
}