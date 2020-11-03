using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Payment
{
    public class GetPaymentFeeResponse : MessageResponse
    {
        public PaymentFeeModel PaymentFee { get; set; }
    }
}