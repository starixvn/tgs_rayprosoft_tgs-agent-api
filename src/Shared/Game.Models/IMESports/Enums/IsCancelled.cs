using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum IsCancelled
    {
        NotCancel = 0,
        Cancel = 1
    }
}
