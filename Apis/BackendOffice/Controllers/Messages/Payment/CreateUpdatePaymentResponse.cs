using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Payment
{
    public class CreateUpdatePaymentResponse : MessageResponse
    {
        public string[] Players { get; set; }
    }
}