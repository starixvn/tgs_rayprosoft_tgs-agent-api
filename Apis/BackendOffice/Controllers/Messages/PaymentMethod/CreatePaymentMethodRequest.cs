using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.PaymentMethod
{
    public class CreatePaymentMethodRequest
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Guid PaymentVendorId { get; set; }
    }
}