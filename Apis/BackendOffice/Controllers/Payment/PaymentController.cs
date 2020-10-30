using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Apis.BackendOffice.DataService.Commands.TGS.Apis.BackendOffice.DataService.Commands;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class PaymentController : BaseContextController
    {
        private readonly ITgsCommands<Payment> _commands;
        private readonly IPaymentCommands _paymentCommands;
        private readonly ITgsQueries<Player> _playerQueries;
        private readonly ITgsQueries<Payment> _queries;
        private readonly IUploadService _uploadService;

        public PaymentController(
            ITgsQueries<Payment> queries,
            ITgsCommands<Payment> commands,
            ITgsQueries<Player> playerQueries,
            IPaymentCommands paymentCommands,
            IUploadService uploadService,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
            _paymentCommands = paymentCommands;
            _uploadService = uploadService;
            _playerQueries = playerQueries;
        }

        [HttpGet]
        [Description("Get Payments")]
        public Task<PagedResponse<PaymentModel>> GetPaymentsAsync(PaymentsRequest request)
        {
            return _queries.GetQueries()
                .Where(request.Enabled, p => p.Enabled == request.Enabled)
                .Where(request.Filter, p => p.PaymentVendor.Name.Contains(request.Filter) ||
                                            p.PaymentMethod.Name.Contains(request.Filter) ||
                                            p.PaymentChannel.Name.Contains(request.Filter))
                .Select(q => new PaymentModel
                {
                    Id = q.Id,
                    Number = q.DisplayOrder,
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
                }).AsNoTracking().CreatePagedResponseAsync(request);
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
            if (payment == null) return CreateInvalidResponse<GetPaymentResponse>(Resources.Payment.PlayerNotFound);
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
                    Whitelist = payment.PaymentUserAccessControls.Where(p => p.ListType == ListType.White).Select(p =>
                        new PaymentUserAccessControlModel
                        {
                            ListType = p.ListType,
                            PlayerCode = p.PlayerCode,
                            PlayerName = p.Player.RealName
                        }).ToArray(),
                    Blacklist = payment.PaymentUserAccessControls.Where(p => p.ListType == ListType.Black).Select(p =>
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
        [Description("Get a Payment handing Fee")]
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

        [HttpPost]
        [Description("Create a Payment")]
        public async Task<CreateUpdatePaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
        {
            if (request.Whitelist != null && request.Blacklist != null)
            {
                var samePlayers = request.Whitelist.Intersect(request.Blacklist).ToArray();
                if (samePlayers.Any())
                    return new CreateUpdatePaymentResponse
                    {
                        Valid = false,
                        Message = Resources.Payment.WhiteSameAsBlack,
                        Players = samePlayers
                    };
            }

            if (await _queries.ExistsAsync(p => p.ChannelName == request.ChannelName))
                return CreateInvalidResponse<CreateUpdatePaymentResponse>(Resources.Payment.NameExists);

            var paymentId = Guid.NewGuid();
            var payment = new Payment
            {
                Id = paymentId,
                DisplayOrder = 1,
                PaymentVendorId = request.PaymentVendorId,
                PaymentMethodId = request.PaymentMethodId,
                PaymentChannelId = request.PaymentChannelId,
                ChannelName = request.ChannelName,
                TransferLimitFrom = request.TransferLimitFrom,
                TransferLimitTo = request.TransferLimitTo,
                TransferCondition = request.TransferCondition,
                AllPlayer = request.AllPlayer,
                IdentityVerification = request.IdentityVerification,
                PhoneVerification = request.PhoneVerification,
                DepositedAmountUnlimited = request.DepositedAmountUnlimited,
                DepositedAmountFrom = request.DepositedAmountFrom,
                DepositedAmountTo = request.DepositedAmountTo,
                Reason = request.Reason,
                CreatedByUsername = Username,
                UpdatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            var whitelist = request.Whitelist?.Select(code => new PaymentUserAccessControl
            {
                Id = Guid.NewGuid(),
                ListType = ListType.White,
                PaymentId = paymentId,
                PlayerCode = code
            }).ToArray() ?? Array.Empty<PaymentUserAccessControl>();
            var blacklist = request.Blacklist?.Select(code => new PaymentUserAccessControl
            {
                Id = Guid.NewGuid(),
                ListType = ListType.Black,
                PaymentId = paymentId,
                PlayerCode = code
            }).ToArray() ?? Array.Empty<PaymentUserAccessControl>();
            var players = whitelist.Union(blacklist).ToArray();

            var vipLevels = request.VipLevelIds.Select(id => new PaymentVipLevel
            {
                PaymentId = paymentId,
                VipLevelId = id
            }).ToArray();

            await _paymentCommands.CreateAsync(payment, players, vipLevels);

            return CreateValidResponse<CreateUpdatePaymentResponse>();
        }

        [HttpPut("{paymentId:guid}")]
        [Description("Update a Payment")]
        public async Task<CreateUpdatePaymentResponse> UpdatePaymentAsync(Guid paymentId,
            UpdatePaymentRequest request)
        {
            var samePlayers = request.Whitelist.Intersect(request.Blacklist).ToArray();
            if (samePlayers.Any())
                return new CreateUpdatePaymentResponse
                {
                    Valid = false,
                    Message = Resources.Payment.WhiteSameAsBlack,
                    Players = samePlayers
                };

            var payment = await _queries.GetSingleAsync(q => q.Id == paymentId);

            if (payment == null) return CreateInvalidResponse<CreateUpdatePaymentResponse>(Resources.Payment.NotFound);

            if (payment.ChannelName == request.ChannelName)
                return CreateInvalidResponse<CreateUpdatePaymentResponse>(Resources.Payment.NameExists);

            payment.ChannelName = request.ChannelName;
            payment.TransferLimitFrom = request.TransferLimitFrom;
            payment.TransferLimitTo = request.TransferLimitTo;
            payment.TransferCondition = request.TransferCondition;
            payment.AllPlayer = request.AllPlayer;
            payment.IdentityVerification = request.IdentityVerification;
            payment.PhoneVerification = request.PhoneVerification;
            payment.DepositedAmountUnlimited = request.DepositedAmountUnlimited;
            payment.DepositedAmountFrom = request.DepositedAmountFrom;
            payment.DepositedAmountTo = request.DepositedAmountTo;
            payment.Reason = request.Reason;
            payment.UpdatedByUsername = Username;
            payment.UpdatedOn = DateTimeOffset.Now;

            var whitelist = request.Whitelist.Select(code => new PaymentUserAccessControl
            {
                Id = Guid.NewGuid(),
                ListType = ListType.White,
                PaymentId = paymentId,
                PlayerCode = code
            }).ToArray();
            var blacklist = request.Blacklist.Select(code => new PaymentUserAccessControl
            {
                Id = Guid.NewGuid(),
                ListType = ListType.Black,
                PaymentId = paymentId,
                PlayerCode = code
            }).ToArray();
            var players = whitelist.Union(blacklist).ToArray();

            var vipLevels = request.VipLevelIds.Select(id => new PaymentVipLevel
            {
                PaymentId = payment.Id,
                VipLevelId = id
            }).ToArray();

            await _paymentCommands.UpdateAsync(payment, players, vipLevels);

            return CreateValidResponse<CreateUpdatePaymentResponse>();
        }

        [HttpPut("{paymentId:guid}/handingFee")]
        [Description("Update a Payment handing Fee")]
        public async Task<MessageResponse> UpdatePaymentHandlingFeeAsync(Guid paymentId,
            UpdatePaymentHandlingFeeRequest request)
        {
            var payment = await _queries.GetSingleAsync(q => q.Id == paymentId);
            payment.HandlingFeePercentage = request.HandlingFeePercentage;
            payment.LowestHandlingFee = request.LowestHandlingFee;
            payment.HighestHandlingFee = request.HighestHandlingFee;
            await _commands.UpdateAsync(payment);
            return CreateValidResponse<MessageResponse>();
        }


        [HttpPost("upload")]
        [Description("Upload player file")]
        public async Task<MessageResponse> UploadPlayersAsync(IFormFile file)
        {
            var uploadedResult = await _uploadService.GetUploadedExcelAsync(file);
            var users = uploadedResult.Select(p => p.FirstOrDefault());

            var allPlayers = await _playerQueries.GetQueries()
                .Where(p => p.Status == PlayerStatus.Enabled && users.Contains(p.Username))
                .Select(p => p.Username)
                .ToArrayAsync();

            var diffUsers = users.Except(allPlayers).ToArray();

            return diffUsers.Any()
                ? new UploadFileResponse
                {
                    Valid = false,
                    Message = Resources.Payment.PlayersNotFound,
                    NotExistsPlayers = diffUsers
                }
                : new UploadFileResponse
                {
                    Valid = true,
                    Players = allPlayers
                };
        }

        [HttpPut("ChangeOrders")]
        [Description("Change Payment Order")]
        public async Task<MessageResponse> ChangeOrder(ChangeOrderRequest request)
        {
            var ids = request.Orders.Select(q => q.Id);
            var payments = await _queries.GetQueries().Where(q => ids.Contains(q.Id)).ToListAsync();
            payments.ForEach(p => p.DisplayOrder = request.Orders.FirstOrDefault(x => x.Id == p.Id).DisplayOrder);

            await _commands.BatchInsertOrUpdateAsync(payments.ToArray());

            return CreateValidResponse<MessageResponse>();
        }
    }
}