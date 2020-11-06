using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;

namespace TGS.Apis.BackendOffice.Controllers.Payment
{
    public partial class PaymentController
    {
        [HttpGet("{paymentId:guid}/auditLogs")]
        [Description("Get a payment's audit log")]
        public async Task<GetPaymentAuditLogResponse> GetAuditLogAsync(Guid paymentId)
        {
            var auditLogs = await _auditLogQueries.GetQueries().Where(q => q.PaymentId == paymentId).ToArrayAsync();

            return new GetPaymentAuditLogResponse
            {
                Valid = true,
                AuditLogs = auditLogs.Select(x => new PaymentAuditLogModel
                {
                    Id = x.Id,
                    PaymentId = x.PaymentId,
                    BeforeChannelName = x.BeforeChannelName,
                    AfterChannelName = x.AfterChannelName,
                    BeforePaymentStatus = x.BeforePaymentStatus,
                    AfterPaymentStatus = x.AfterPaymentStatus,
                    BeforeTransferLimitFrom = x.BeforeTransferLimitFrom,
                    AfterTransferLimitTo = x.AfterTransferLimitTo,
                    BeforePlayerListCondition = x.BeforePlayerListCondition,
                    BeforeAllPlayer = x.BeforeAllPlayer,
                    AfterAllPlayer = x.AfterAllPlayer,
                    BeforeIdentityVerification = x.BeforeIdentityVerification,
                    AfterIdentityVerification = x.AfterIdentityVerification,
                    BeforePhoneVerification = x.BeforePhoneVerification,
                    AfterPhoneVerification = x.AfterPhoneVerification,
                    BeforeDepositedAmountUnlimited = x.BeforeDepositedAmountUnlimited,
                    AfterDepositedAmountUnlimited = x.AfterDepositedAmountUnlimited,
                    BeforeDepositedAmountFrom = x.BeforeDepositedAmountFrom,
                    AfterDepositedAmountFrom = x.AfterDepositedAmountFrom,
                    BeforeDepositedAmountTo = x.BeforeDepositedAmountTo,
                    AfterDepositedAmountTo = x.AfterDepositedAmountTo,
                    Reason = x.Reason,
                    CreatedOn = x.CreatedOn,
                    CreatedByUsername = x.CreatedByUsername
                }).ToArray()
            };
        }
    }
}