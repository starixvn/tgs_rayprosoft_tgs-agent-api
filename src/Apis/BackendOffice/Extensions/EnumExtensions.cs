using System.Collections.Generic;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Deposit;
using TGS.Services.Payment;

namespace TGS.Apis.BackendOffice.Extensions
{
    public static class EnumExtensions
    {
        public static FlowState ToFlowState(this int status)
            => status switch
            {
                100 => FlowState.WaitingPayment,
                1000 => FlowState.Review,
                7000 => FlowState.Rejected,
                8000 => FlowState.Approved,
                _ => FlowState.Unkown,
            };

        public static ReviewState ToReviewState(this ReviewResponseState reviewResponse)
                    => reviewResponse switch
                    {
                        ReviewResponseState.Approve => ReviewState.Approve,
                        _ => ReviewState.Reject
                    };

        public static IEnumerable<PaymentFlowState> ToPaymentFlowState(this FlowState? status)
        {
            return status switch
            {
                FlowState.WaitingPayment => new[] { PaymentFlowState.WaitPayment },
                FlowState.Review => new[] { PaymentFlowState.Init, PaymentFlowState.WaitApprove },
                FlowState.Approved => new[] { PaymentFlowState.Approved },
                FlowState.Rejected => new[] { PaymentFlowState.Reject },
                _ => new[] { PaymentFlowState.WaitPayment, PaymentFlowState.WaitApprove, PaymentFlowState.Approved, PaymentFlowState.Reject }
            };
        }
    }
}
