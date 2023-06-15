namespace TGS.Apis.AgentApi.Models.ReportAgentWinLoss;

public record AgentPlayerSpecialDetailReportModel
{
    public decimal Bonus { get; set; }
    public decimal BonusAdjust { get; set; }
    public decimal ProductFee { get; set; }
}