using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerIncreaseAmount
    {
        public Guid Id { get; set; }
        public string PlayerCode { get; set; }
        public PlayerIncreaseAmountType PlayerIncreaseAmountType { get; set; }
        public decimal Amount { get; set; }
        public decimal Turnover { get; set; }
        public string Remark { get; set; }
    }
}