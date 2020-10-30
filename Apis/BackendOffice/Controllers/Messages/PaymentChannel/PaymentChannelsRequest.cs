using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.PaymentChannel
{
    public class PaymentChannelsRequest : PageableRequest
    {
        public string Code { get; set; }
    }
}