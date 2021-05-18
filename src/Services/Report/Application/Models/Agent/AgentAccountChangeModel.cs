using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Report.Application.Models.Agent
{
    public class AgentAccountChangeModel
    {
        public Guid TransactionId { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTimeOffset RecordedOn { get; set; }
    }
}