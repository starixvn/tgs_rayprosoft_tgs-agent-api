using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Payment
{
    public partial class PaymentController
    {
        [HttpPut("{paymentId:guid}/enable")]
        [Description("Enable a Payment")]
        public async Task<MessageResponse> EnablePaymentAsync(Guid paymentId)
        {
            var payment = await _queries.GetSingleAsync(q => q.Id == paymentId);
            payment.PaymentStatus = PaymentStatus.Enabled;
            await _commands.UpdateAsync(payment);

            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut("{paymentId:guid}/disable")]
        [Description("Disable a Payment")]
        public async Task<MessageResponse> DisablePaymentAsync(Guid paymentId)
        {
            var payment = await _queries.GetSingleAsync(q => q.Id == paymentId);
            payment.PaymentStatus = PaymentStatus.Disabled;
            await _commands.UpdateAsync(payment);

            return CreateValidResponse<MessageResponse>();
        }
    }
}