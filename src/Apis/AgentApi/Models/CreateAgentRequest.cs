namespace TGS.Apis.AgentApi.Models;

public class CreateAgentRequest
{
    public decimal CardGameRebatePercentage { get; init; }

    public decimal DividendPercentage { get; init; }

    public string Email { get; set; }

    public decimal LiveDealerRebatePercentage { get; init; }

    public decimal LotteryRebatePercentage { get; init; }

    public string Password { get; init; }

    public string Phone { get; set; }

    public string RealName { get; set; }

    public string Remarks { get; init; }

    public decimal SlotRebatePercentage { get; init; }

    public decimal SportsRebatePercentage { get; init; }

    public string UserName { get; set; }

    public decimal VirtualSportsRebatePercentage { get; init; }
}
