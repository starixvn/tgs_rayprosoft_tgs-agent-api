namespace TGS.Shared.Game.Models.WMCasino.Incoming;

public record ManuelRoundRecordRequest
{
    public string PlayerCode { get; init; }
    public string ExternalGameId { get; init; }
    public string ExternalRoundId { get; init; }
}
