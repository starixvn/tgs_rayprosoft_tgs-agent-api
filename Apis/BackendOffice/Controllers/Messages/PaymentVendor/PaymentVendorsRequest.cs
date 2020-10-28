using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.PaymentVendor
{
    public class PaymentVendorsRequest : PageableRequest
    {
        public string Code { get; internal set; }
    }
}