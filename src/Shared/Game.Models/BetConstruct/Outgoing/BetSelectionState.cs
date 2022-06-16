using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BetConstruct.Outgoing;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BetSelectionState
{
    NotResulted = 0,
    Returned = 2,
    Lost = 3,
    Won = 4,
    WinReturn = 5,
    LossReturn = 6
}
