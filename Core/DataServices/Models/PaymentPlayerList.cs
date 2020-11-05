using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class PaymentPlayerList
    {
        public ListType ListType { get; set; }
        public string PlayerCode { get; set; }
        public virtual Player Player { get; set; }
        public Guid PaymentId { get; set; }
        public virtual Payment Payment { get; set; }
    }
}