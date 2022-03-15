using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class AdjustAgentRecord
    {
        public Guid Id { get; set; }

        public Guid? TransactionId { get; set; }

        public Guid BrandId { get; set; }

        public Guid AgentId { get; set; }

        public AdjustmentType AdjustmentType { get; set; }

        public AgentAdjustmentTransactionType Type { get; set; }

        public AdjustTransferType TransferType { get; set; }

        public string TransferTarget { get; set; }

        public decimal Amount { get; set; }

        public string Remark { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public string CreatedByUsername { get; set; }
    }
}