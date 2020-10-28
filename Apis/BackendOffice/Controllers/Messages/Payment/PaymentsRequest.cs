using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Payment
{
    public class PaymentsRequest : PageableRequest
    {
        public bool Enabled { get; set; }
        public string Filter { get; set; }
    }
}