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

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string CreatedByUsername { get; set; }

        public string UpdatedByUsername { get; set; }

        public Payment Payment { get; set; }

        public PaymentMethod Method { get; set; }

        public PaymentChannel Channel { get; set; }
    }
}
