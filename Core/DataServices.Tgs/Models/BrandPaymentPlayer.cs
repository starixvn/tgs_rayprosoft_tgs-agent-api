using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class BrandPaymentPlayer
    {
        public PlayerListType ListType { get; set; }
        public string PlayerCode { get; set; }
        public virtual Player Player { get; set; }
        public Guid BrandPaymentId { get; set; }
        public virtual BrandPayment BrandPayment { get; set; }
    }
}