using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.PaymentVendor
{
    public class CreateUpdatePaymentVendorRequest
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}