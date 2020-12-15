using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.BrandPayment
{
    public class GetBrandPaymentsRequest : PageableRequest
    {
        public PaymentTransferType? TransferType { get; set; }
        public SwitchStatus? PaymentStatus { get; set; }
        public string Keyword { get; set; }
    }
}