using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class AdjustTransferRecord
    {
        public Guid Id { get; set; }

        public Guid TransactionFromId { get; set; }

        public Guid TransactionToId { get; set; }

        public AdjustTransferType TransferType { get; set; }

        public string TransferFromUsername { get; set; }

        public string TransferToUsername { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}