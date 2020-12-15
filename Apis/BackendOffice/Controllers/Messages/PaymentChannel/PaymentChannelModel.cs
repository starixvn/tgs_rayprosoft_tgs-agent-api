using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.PaymentChannel
{
    public class PaymentChannelModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string PaymentName { get; set; }
        public string PaymentMethodName { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
    }
}