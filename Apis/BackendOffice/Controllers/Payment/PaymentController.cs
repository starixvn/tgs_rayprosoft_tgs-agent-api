using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseContextController
    {
        private readonly ITgsCommands<Payment> _commands;
        private readonly ITgsQueries<Payment> _queries;

        public PaymentController(
            ITgsQueries<Payment> queries,
            ITgsCommands<Payment> commands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet]
        [Description("Get Payments")]
        public Task<PagedResponse<PaymentModel>> GetPaymentsAsync([FromQuery] PaymentsRequest request)
        {
            return _queries.GetQueries()
                .Where(request.Enabled, p => p.Enabled == request.Enabled)
                .Where(request.Filter, p => p.PaymentVendor.Name.Contains(request.Filter) ||
                                            p.PaymentMethod.Name.Contains(request.Filter) ||
                                            p.PaymentChannel.Name.Contains(request.Filter))
                .Select(q => new PaymentModel
                {
                    Id = q.Id,
                    PaymentVendorName = q.PaymentVendor.Name,
                    PaymentMethodName = q.PaymentMethod.Name,
                    PaymentChannelName = q.PaymentChannel.Name,
                    ChannelName = q.ChannelName,
                    TransferCondition = q.TransferCondition,
                    AllPlayer = q.AllPlayer,
                    DepositedAmountFrom = q.DepositedAmountFrom,
                    DepositedAmountTo = q.DepositedAmountTo,
                    IdentityVerification = q.IdentityVerification,
                    PhoneVerification = q.PhoneVerification,
                    DesignatedUser = q.PaymentUserAccessControls.Count > 0,
                    TransferLimitFrom = q.TransferLimitFrom,
                    TransferLimitTo = q.TransferLimitTo,
                    Enabled = q.Enabled,
                    PaymentHandlingFee = new PaymentHandlingFeeModel
                    {
                        HandlingFeePercentage = q.HandlingFeePercentage,
                        HighestHandlingFee = q.HighestHandlingFee,
                        LowestHandlingFee = q.LowestHandlingFee
                    },
                    CreatedByUsername = q.CreatedByUsername,
                    UpdatedByUsername = q.UpdatedByUsername,
                    CreatedOn = q.CreatedOn,
                    UpdatedOn = q.UpdatedOn
                }).CreatePagedResponseAsync(request);
        }

        [HttpGet("{id}")]
        [Description("Get a Payment")]
        public async Task<GetPaymentResponse> GetPaymentMethodAsync(Guid id)
        {
            var payment = await _queries.GetSingleAsync(q => q.Id == id,
                p => p
                    .Include(pm => pm.PaymentVendor)
                    .Include(pm => pm.PaymentMethod)
                    .Include(pm => pm.PaymentChannel)
                    .Include(pm => pm.PaymentUserAccessControls)
                    .ThenInclude(pp => pp.Player)
                    .Include(pm => pm.VipLevels)
                    .ThenInclude(pp => pp.VipLevel)
                    .AsNoTracking());

            return new GetPaymentResponse
            {
                Valid = true,
                Payment = new PaymentModel
                {
                    Id = payment.Id,
                    PaymentVendorName = payment.PaymentVendor.Name,
                    PaymentMethodName = payment.PaymentMethod.Name,
                    PaymentChannelName = payment.PaymentChannel.Name,
                    ChannelName = payment.ChannelName,
                    TransferLimitFrom = payment.TransferLimitFrom,
                    TransferLimitTo = payment.TransferLimitTo,
                    TransferCondition = payment.TransferCondition,
                    AllPlayer = payment.AllPlayer,
                    IdentityVerification = payment.IdentityVerification,
                    PhoneVerification = payment.PhoneVerification,
                    DepositedAmountUnlimited = payment.DepositedAmountUnlimited,
                    DepositedAmountFrom = payment.DepositedAmountFrom,
                    DepositedAmountTo = payment.DepositedAmountTo,
                    DesignatedUser = payment.PaymentUserAccessControls.Count > 0,
                    Enabled = payment.Enabled,
                    Reason = payment.Reason,
                    PaymentUserAccessControls = payment.PaymentUserAccessControls.Select(p =>
                        new PaymentUserAccessControlModel
                        {
                            ListType = p.ListType,
                            PlayerCode = p.PlayerCode,
                            PlayerName = p.Player.RealName
                        }).ToArray(),
                    VipLevelIds = payment.VipLevels.Select(p => p.VipLevelId).ToArray(),
                    CreatedByUsername = payment.CreatedByUsername,
                    UpdatedByUsername = payment.UpdatedByUsername,
                    CreatedOn = payment.CreatedOn,
                    UpdatedOn = payment.UpdatedOn
                }
            };
        }

        [HttpGet("{id}/handingFee")]
        [Description("Get a Payment")]
        public async Task<GetPaymentHandlingFeeResponse> GetPaymentHandingFeeAsync(Guid id)
        {
            var payment = await _queries.GetSingleAsync(q => q.Id == id, p => p.AsNoTracking());

            return new GetPaymentHandlingFeeResponse
            {
                Valid = true,
                PaymentHandlingFee = new PaymentHandlingFeeModel
                {
                    HandlingFeePercentage = payment.HandlingFeePercentage,
                    HighestHandlingFee = payment.HighestHandlingFee,
                    LowestHandlingFee = payment.LowestHandlingFee
                }
            };
        }
    }
}