using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.AgentAdjust
{
    public class CreateAgentAdjustmentRequest : IRequest<MessageResponse>
    {
        public Guid BrandId { get; set; }
        public Guid AgentId { get; set; }
        public string TargetName { get; set; }
        public AccountType? AccountType { get; set; }
        public AdjustType AdjustType { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
    }
}