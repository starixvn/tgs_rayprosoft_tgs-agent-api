using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Apis.BackendOffice.DataService.Services;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class PaymentController : BaseContextController
    {
        private readonly ITgsQueries<PaymentAuditLog> _auditLogQueries;
        private readonly ITgsCommands<Core.DataServices.Models.Payment> _commands;
        private readonly IPaymentService _paymentService;
        private readonly ITgsQueries<Player> _playerQueries;
        private readonly ITgsQueries<Core.DataServices.Models.Payment> _queries;
        private readonly IUploadService _uploadService;

        public PaymentController(
            ITgsQueries<Core.DataServices.Models.Payment> queries,
            ITgsCommands<Core.DataServices.Models.Payment> commands,
            ITgsQueries<PaymentAuditLog> auditLogQueries,
            ITgsQueries<Player> playerQueries,
            IUploadService uploadService,
            IPaymentService paymentService,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
            _uploadService = uploadService;
            _playerQueries = playerQueries;
            _paymentService = paymentService;
            _auditLogQueries = auditLogQueries;
        }

        [HttpGet]
        [Description("Get Payments")]
        public Task<PagedResponse<PaymentModel>> GetPaymentsAsync([FromQuery] PaymentsRequest request)
        {
            return _queries.GetQueries()
                .Where(request.TransferType, p => p.TransferType == request.TransferType)
                .Where(request.PaymentStatus, p => p.PaymentStatus == request.PaymentStatus)
                .Where(request.Keyword, p => p.PaymentVendor.Name.Contains(request.Keyword) ||
                                             p.PaymentMethod.Name.Contains(request.Keyword) ||
                                             p.PaymentChannel.Name.Contains(request.Keyword))
                .Select(q => new PaymentModel
                {
                    Id = q.Id,
                    Number = q.DisplayOrder,
                    PaymentVendorName = q.PaymentVendor.Name,
                    PaymentMethodName = q.PaymentMethod.Name,
                    PaymentChannelName = q.PaymentChannel.Name,
                    TransferType = q.TransferType,
                    ChannelName = q.ChannelName,
                    PlayerListCondition = q.PlayerListCondition,
                    AllPlayer = q.AllPlayers,
                    DepositedAmountMin = q.DepositedAmountMin,
                    DepositedAmountMax = q.DepositedAmountMax,
                    DepositedAmountUnlimited = q.DepositedAmountUnlimited,
                    IdentityAuthentication = q.IdentityAuthentication,
                    PhoneAuthentication = q.PhoneAuthentication,
                    DesignatedUsers = q.PaymentPlayerList.Count > 0,
                    TransferLimitFrom = q.TransferLimitFrom,
                    TransferLimitTo = q.TransferLimitTo,
                    PaymentStatus = q.PaymentStatus,
                    PaymentFee = new PaymentFeeModel
                    {
                        FeePercentage = q.FeePercentage,
                        HighestFee = q.HighestFee,
                        LowestFee = q.LowestFee
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
                    .Include(pm => pm.PaymentPlayerList)
                    .ThenInclude(pp => pp.Player)
                    .Include(pm => pm.VipLevels)
                    .ThenInclude(pp => pp.VipLevel)
                    .AsNoTracking());
            if (payment == null)
            {
                return CreateInvalidResponse<GetPaymentResponse>(Resources.Payment.PlayerNotFound);
            }

            return new GetPaymentResponse
            {
                Valid = true,
                Payment = new PaymentModel
                {
                    Id = payment.Id,
                    PaymentVendorId = payment.PaymentVendor.Id,
                    PaymentVendorName = payment.PaymentVendor.Name,
                    PaymentMethodId =  payment.PaymentVendor.Id,
                    PaymentMethodName = payment.PaymentMethod.Name,
                    PaymentChannelId = payment.PaymentMethod.Id,
                    PaymentChannelName = payment.PaymentChannel.Name,
                    ChannelName = payment.ChannelName,
                    TransferLimitFrom = payment.TransferLimitFrom,
                    TransferLimitTo = payment.TransferLimitTo,
                    PlayerListCondition = payment.PlayerListCondition,
                    AllPlayer = payment.AllPlayers,
                    IdentityAuthentication = payment.IdentityAuthentication,
                    PhoneAuthentication = payment.PhoneAuthentication,
                    DepositedAmountUnlimited = payment.DepositedAmountUnlimited,
                    DepositedAmountMin = payment.DepositedAmountMin,
                    DepositedAmountMax = payment.DepositedAmountMax,
                    DesignatedUsers = payment.PaymentPlayerList.Count > 0,
                    PaymentStatus = payment.PaymentStatus,
                    Reason = payment.Reason,
                    Whitelist = payment.PaymentPlayerList.Where(p => p.ListType == PlayerListType.White).Select(p => p.Player.Username).ToArray(),
                    Blacklist = payment.PaymentPlayerList.Where(p => p.ListType == PlayerListType.Black).Select(p => p.Player.Username).ToArray(),
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
        public async Task<GetPaymentFeeResponse> GetPaymentHandingFeeAsync(Guid id)
        {
            var payment = await _queries.GetSingleAsync(q => q.Id == id, p => p.AsNoTracking());

            return new GetPaymentFeeResponse
            {
                Valid = true,
                PaymentFee = new PaymentFeeModel
                {
                    FeePercentage = payment.FeePercentage,
                    HighestFee = payment.HighestFee,
                    LowestFee = payment.LowestFee
                }
            };
        }

        [HttpPost]
        [Description("Create a Payment")]
        public async Task<CreateUpdatePaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
        {
            if (request.Whitelist != null && request.Blacklist != null)
            {
                var duplicatePlayers = request.Whitelist.Intersect(request.Blacklist).ToArray();
                if (duplicatePlayers.Any())
                {
                    return new CreateUpdatePaymentResponse
                    {
                        Valid = false,
                        Message = Resources.Payment.WhiteSameAsBlack,
                        DuplicatePlayers = duplicatePlayers
                    };
                }
            }

            if (await _queries.ExistsAsync(p => p.ChannelName == request.ChannelName))
            {
                return CreateInvalidResponse<CreateUpdatePaymentResponse>(Resources.Payment.NameExists);
            }

            await _paymentService.CreatePaymentAsync(request, Username);
            return CreateValidResponse<CreateUpdatePaymentResponse>();
        }

        [HttpPut("{paymentId:guid}")]
        [Description("Update a Payment")]
        public async Task<CreateUpdatePaymentResponse> UpdatePaymentAsync(Guid paymentId,
            UpdatePaymentRequest request)
        {
            if (request.Whitelist != null && request.Blacklist != null)
            {
                var duplicatePlayers = request.Whitelist.Intersect(request.Blacklist).ToArray();
                if (duplicatePlayers.Any())
                {
                    return new CreateUpdatePaymentResponse
                    {
                        Valid = false,
                        Message = Resources.Payment.WhiteSameAsBlack,
                        DuplicatePlayers = duplicatePlayers
                    };
                }
            }

            var payment = await _queries.GetSingleAsync(q => q.Id == paymentId);

            if (payment == null)
            {
                return CreateInvalidResponse<CreateUpdatePaymentResponse>(Resources.Payment.NotFound);
            }

            if (await _queries.ExistsAsync(p => p.Id != paymentId && p.ChannelName == request.ChannelName))
            {
                return CreateInvalidResponse<CreateUpdatePaymentResponse>(Resources.Payment.NameExists);
            }


            await _paymentService.UpdatePaymentAsync(payment, request, Username);

            return CreateValidResponse<CreateUpdatePaymentResponse>();
        }

        [HttpPut("{paymentId:guid}/handingFee")]
        [Description("Update a Payment handing Fee")]
        public async Task<MessageResponse> UpdatePaymentHandlingFeeAsync(Guid paymentId,
            UpdatePaymentFeeRequest request)
        {
            var payment = await _queries.GetSingleAsync(q => q.Id == paymentId);
            payment.FeePercentage = request.FeePercentage;
            payment.LowestFee = request.LowestFee;
            payment.HighestFee = request.HighestFee;
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