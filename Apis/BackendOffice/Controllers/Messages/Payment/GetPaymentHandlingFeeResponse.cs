using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Payment
{
    public class GetPaymentHandlingFeeResponse : MessageResponse
    {
        public PaymentHandlingFeeModel PaymentHandlingFee { get; set; }
    }
}