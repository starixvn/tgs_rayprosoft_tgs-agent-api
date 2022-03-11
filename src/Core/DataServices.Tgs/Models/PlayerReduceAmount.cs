using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerReduceAmount
    {
        public Guid Id { get; set; }

        public string PlayerCode { get; set; }

        public PlayerReduceAmountType PlayerReduceAmountType { get; set; }

        public string TransferTarget { get; set; }

        public decimal Amount { get; set; }

        public string Remark { get; set; }
    }
}