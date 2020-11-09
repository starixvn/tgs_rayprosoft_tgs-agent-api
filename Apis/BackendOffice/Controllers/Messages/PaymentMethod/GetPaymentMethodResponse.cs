using TGS.Apis.BackendOffice.Controllers.Messages.PaymentVendor;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.PaymentMethod
{
    public class GetPaymentMethodResponse : MessageResponse
    {
        public PaymentMethodModel PaymentMethod { get; set; }
    }
}