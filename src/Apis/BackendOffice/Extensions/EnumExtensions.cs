using System.Collections.Generic;
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
                PaymentFlowState.Paid => FlowState.Paid,
                _ => FlowState.Unkown,
            };

        public static IEnumerable<PaymentFlowState> ToPaymentFlowState(this FlowState status)
            => status switch
            {
                FlowState.WaitingPayment => new[] { PaymentFlowState.WaitPayment },
                FlowState.Review => new[] { PaymentFlowState.Init, PaymentFlowState.WaitApprove },
                FlowState.Approved => new[] { PaymentFlowState.Approved },
                FlowState.Rejected => new[] { PaymentFlowState.Reject },
                FlowState.Paid => new[] { PaymentFlowState.Paid },
                _ => new[] {
                    PaymentFlowState.WaitPayment,
                    PaymentFlowState.WaitApprove,
                    PaymentFlowState.Approved,
                    PaymentFlowState.Reject,
                    PaymentFlowState.Paid
                }
            };

        public static IEnumerable<PaymentFlowState> ToPaymentFlowState(this FlowState? status)
            => status?.ToPaymentFlowState() ?? new[] {
                PaymentFlowState.WaitPayment,
                PaymentFlowState.WaitApprove,
                PaymentFlowState.Approved,
                PaymentFlowState.Reject,
                PaymentFlowState.Paid
            };

        public static ReviewState ToReviewState(this ReviewResponseState reviewResponse)
            => reviewResponse switch
            {
                ReviewResponseState.Approve => ReviewState.Approve,
                _ => ReviewState.Reject
            };
    }
}
