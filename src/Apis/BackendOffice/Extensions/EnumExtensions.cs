using System.Collections.Generic;
using Tgs.Payment;
using Tgs.Payment.Clients;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Deposit;

namespace TGS.Apis.BackendOffice.Extensions
{
    public static class EnumExtensions
    {
        public static FlowState ToFlowState(this PaymentFlowState status)
            => status switch
            {
                PaymentFlowState.WaitPayment => FlowState.WaitingPayment,
                PaymentFlowState.WaitApprove => FlowState.Review,
                PaymentFlowState.Reject => FlowState.Rejected,
                PaymentFlowState.Approved => FlowState.Approved,
                _ => FlowState.Unkown,
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

        public static ReviewState ToReviewState(this ReviewResponseState reviewResponse)
                            => reviewResponse switch
                            {
                                ReviewResponseState.Approve => ReviewState.Approve,
                                _ => ReviewState.Reject
                            };
    }
}
