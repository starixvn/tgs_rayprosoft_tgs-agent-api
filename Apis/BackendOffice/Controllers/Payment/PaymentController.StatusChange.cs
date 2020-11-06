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
        [HttpPut("{paymentId:guid}/{status}")]
        [Description("change a Payment's status")]
        public async Task<MessageResponse> UpdatePaymentStatusAsync(Guid paymentId, PaymentStatus status)
        {
            var payment = await _queries.GetSingleAsync(q => q.Id == paymentId);

            if (payment == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Payment.PaymentNotFound);
            }

            await _paymentService.ChangeStatusAsync(payment, status, Username);

            return CreateValidResponse<MessageResponse>();
        }
    }
}