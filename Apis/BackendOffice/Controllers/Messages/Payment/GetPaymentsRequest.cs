using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Payment
{
    public class GetPaymentsRequest : PageableRequest
    {
        public PaymentTransferType? TransferType { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public string Keyword { get; set; }
    }
}