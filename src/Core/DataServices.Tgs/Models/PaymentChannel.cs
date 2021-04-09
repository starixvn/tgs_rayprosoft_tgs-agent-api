using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PaymentChannel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }
    }
}
