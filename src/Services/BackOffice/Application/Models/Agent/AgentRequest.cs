using System;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Domain.Enums;

namespace TGS.Services.BackOffice.Application.Models.Agent
{
    public class AgentRequest : BaseRequest
    {
        public string Password { get; init; }
        public string RealName { get; init; }
        public string ParentUsername { get; init; }
        public MemberUpdateType UpdateType { get; init; }
        public AgentLevelType LevelType { get; init; }
        public AgentType Type { get; init; }
        public Guid BrandId { get; init; }
        public AgentStatus Status { get; init; }
        public decimal PositionTakingPercentage { get; init; } //佔成
        public decimal BonusPercentage { get; init; }
        public decimal RebatePercentage { get; init; }
        public string Email { get; init; }
        public string Phone { get; init; }
        public string QQ { get; init; }
        public string Wechat { get; init; }
    }
}