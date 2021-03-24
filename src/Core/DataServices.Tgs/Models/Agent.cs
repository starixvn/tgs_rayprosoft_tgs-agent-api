using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class Agent
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string RealName { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public byte[] WithdrawPassword { get; set; }
        public byte[] WithdrawSalt { get; set; }
        public Status Status { get; set; }
        public AgentStatus AgentStatus { get; set; }
        public AgentLevelType LevelType { get; set; }
        public AgentType Type { get; set; }
        public string InvitationCode { get; set; }
        public Guid BrandId { get; set; }
        public virtual Brand Brand { get; set; }
        public Guid? ParentAgentId { get; set; }
        public virtual Agent ParentAgent { get; set; }
        public DateTimeOffset RegisteredOn { get; set; }
        public decimal PositionTakingPercentage { get; set; } //佔成
        public decimal BonusPercentage { get; set; }
        public decimal RebatePercentage { get; set; }
        public string Remarks { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string QQ { get; set; }
        public string Wechat { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public virtual ICollection<AgentWallet> Wallets { get; set; }
        public virtual ICollection<Player> Players { get; set; }
        public virtual ICollection<AgentBank> Banks { get; set; }
        public virtual ICollection<Agent> SubAgents { get; set; }
    }
}