using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.BackOffice.Application.Models.Agent
{
    public class AgentModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string RealName { get; set; }
        public AgentStatus Status { get; set; }
        public AgentConfigStatus AgentStatus { get; set; }
        public AgentType Type { get; set; }
        public AgentLevelType LevelType { get; init; }
        public string InvitationCode { get; set; }
        public Guid BrandId { get; set; }
        public string ParentAgentUsername { get; set; }
        public DateTimeOffset RegisteredOn { get; set; }
        public decimal Balance { get; set; }
        public decimal PositionTakingPercentage { get; set; } //佔成
        public decimal BonusPercentage { get; set; }
        public decimal RebatePercentage { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string QQ { get; set; }
        public string Wechat { get; set; }
        public string Remarks { get; set; }
        public int TeamNumber { get; set; }
        public AgentBankModel[] Banks { get; set; }
    }
}