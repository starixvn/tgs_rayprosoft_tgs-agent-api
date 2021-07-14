using System.Collections.Generic;
using Tgs.Payment.Clients;
using TGS.Apis.OperatorApi.Models.Messages.Deposit;

namespace TGS.Apis.OperatorApi.Extensions
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
    }
}