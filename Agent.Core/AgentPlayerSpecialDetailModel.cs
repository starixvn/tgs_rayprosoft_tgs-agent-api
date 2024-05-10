using System;

namespace TGS.Services.Agent;

public record AgentPlayerSpecialDetailModel
{
    public decimal Bonus { get; set; }
    public decimal BonusAdjust { get; set; }
    public decimal PlatformCharge { get; set; }
    public decimal ProductFee { get; set; }
}