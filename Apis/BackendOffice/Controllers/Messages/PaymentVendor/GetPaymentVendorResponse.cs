using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.PaymentVendor
{
    public class GetPaymentVendorResponse : MessageResponse
    {
        public PaymentVendorModel PaymentVendor { get; set; }
    }
}