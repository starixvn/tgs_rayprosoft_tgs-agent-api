using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.BrandPayment;

namespace TGS.Apis.BackendOffice.Controllers.BrandPayment
{
    public partial class BrandPaymentController
    {
        [HttpGet("{paymentId:guid}/auditLogs")]
        [Description("Get a payment's audit log")]
        public async Task<GetBrandPaymentAuditLogResponse> GetAuditLogAsync(Guid paymentId)
        {
            var auditLogs = await _auditLogQueries.GetQueries().Where(q => q.BrandPaymentId == paymentId).ToArrayAsync();

            return new GetBrandPaymentAuditLogResponse
            {
                Valid = true,
                AuditLogs = auditLogs.Select(x => new PaymentAuditLogModel
                {
                    Id = x.Id,
                    PaymentId = x.BrandPaymentId,
                    BeforeChannelName = x.BeforeChannelName,
                    AfterChannelName = x.AfterChannelName,
                    BeforePaymentStatus = x.BeforePaymentStatus,
                    AfterPaymentStatus = x.AfterPaymentStatus,
                    BeforeTransferLimitFrom = x.BeforeTransferLimitFrom,
                    AfterTransferLimitFrom = x.AfterTransferLimitFrom,
                    BeforeTransferLimitTo = x.BeforeTransferLimitTo,
                    AfterTransferLimitTo = x.AfterTransferLimitTo,
                    BeforePlayerListCondition = x.BeforePlayerListCondition,
                    AfterPlayerListCondition = x.AfterPlayerListCondition,
                    BeforeAllPlayers = x.BeforeAllPlayers,
                    AfterAllPlayers = x.AfterAllPlayers,
                    BeforeIdentityAuthentication = x.BeforeIdentityAuthentication,
                    AfterIdentityAuthentication = x.AfterIdentityAuthentication,
                    BeforePhoneAuthentication = x.BeforePhoneAuthentication,
                    AfterPhoneAuthentication = x.AfterPhoneAuthentication,
                    BeforeDepositedAmountUnlimited = x.BeforeDepositedAmountUnlimited,
                    AfterDepositedAmountUnlimited = x.AfterDepositedAmountUnlimited,
                    BeforeDepositedAmountMin = x.BeforeDepositedAmountMin,
                    AfterDepositedAmountMin = x.AfterDepositedAmountMin,
                    BeforeDepositedAmountMax = x.BeforeDepositedAmountMax,
                    AfterDepositedAmountMax = x.AfterDepositedAmountMax,
                    Reason = x.Reason,
                    CreatedOn = x.CreatedOn,
                    CreatedByUsername = x.CreatedByUsername
                }).ToArray()
            };
        }
    }
}