using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PaymentMethodChannel
    {
        public Guid Id { get; set; }

        public Guid PaymentId { get; set; }

        public Guid MethodId { get; set; }

        public Guid ChannelId { get; set; }

        public ActivateStatus Status { get; set; }

        public virtual Payment Payment { get; set; }

        public virtual PaymentMethod Method { get; set; }

        public virtual PaymentChannel Channel { get; set; }
    }
}
