using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.WMCasino.Common;

namespace TGS.Shared.Game.Models.WMCasino.Outgoing;

public record GetDateTimeReportResponse : ErrorResponse
{
    [JsonPropertyName("result")]
    public GameBetResult[] Results { get; init; }
}