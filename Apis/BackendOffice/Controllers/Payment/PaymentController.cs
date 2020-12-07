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
        private readonly IPaymentService _paymentService;
        private readonly ITgsQueries<Player> _playerQueries;
        private readonly IUploadService _uploadService;

        public PaymentController(
            ITgsQueries<Player> playerQueries,
            IUploadService uploadService,
            IPaymentService paymentService,
            ITgsQueries<PaymentAuditLog> auditLogQueries)
        {
            _uploadService = uploadService;
            _playerQueries = playerQueries;
            _paymentService = paymentService;
            _auditLogQueries = auditLogQueries;
        }

        [HttpGet]
        [Description("Get Payments")]
        public Task<PagedResponse<PaymentModel>> GetPaymentsAsync([FromQuery] GetPaymentsRequest request)
        {
            return _paymentService.GetPaymentsAsync()
                .Include(p => p.PaymentPlayerList).ThenInclude(pp => pp.Player)
                .Include(p => p.VipLevels)
                .Where(request.TransferType, p => p.TransferType == request.TransferType)
                .Where(request.PaymentStatus, p => p.PaymentStatus == request.PaymentStatus)
                .Where(request.Keyword, p => p.PaymentVendor.Name.Contains(request.Keyword) ||
                                             p.PaymentMethod.Name.Contains(request.Keyword) ||
                                             p.PaymentChannel.Name.Contains(request.Keyword))
                .Select(p => new PaymentModel
                {
                    Id = p.Id,
                    Number = p.DisplayOrder,
                    PaymentVendorName = p.PaymentVendor.Name,
                    PaymentMethodName = p.PaymentMethod.Name,
                    PaymentChannelName = p.PaymentChannel.Name,
                    TransferType = p.TransferType,
                    ChannelName = p.ChannelName,
                    PlayerListCondition = p.PlayerListCondition,
                    AllPlayers = p.AllPlayers,
                    DepositedAmountMin = p.DepositedAmountMin,
                    DepositedAmountMax = p.DepositedAmountMax,
                    DepositedAmountUnlimited = p.DepositedAmountUnlimited,
                    IdentityAuthentication = p.IdentityAuthentication,
                    PhoneAuthentication = p.PhoneAuthentication,
                    TransferLimitFrom = p.TransferLimitFrom,
                    TransferLimitTo = p.TransferLimitTo,
                    PaymentStatus = p.PaymentStatus,
                    DesignatedUsers = GetDesignatedUsersAsync(p).Any(),
                    PaymentFee = new PaymentFeeModel
                    {
                        FeePercentage = p.FeePercentage,
                        HighestFee = p.HighestFee,
                        LowestFee = p.LowestFee
                    },
                    VipLevelIds = p.VipLevels.Select(p => p.VipLevelId).ToArray(),
                    CreatedByUsername = p.CreatedByUsername,
                    UpdatedByUsername = p.UpdatedByUsername,
                    CreatedOn = p.CreatedOn,
                    UpdatedOn = p.UpdatedOn
                })
                .CreatePagedResponseAsync(request);
        }

        [HttpGet("{id}")]
        [Description("Get a Payment")]
        public async Task<GetPaymentResponse> GetPaymentMethodAsync(Guid id)
        {
            var payment = await _paymentService.GetPaymentAsync(q => q.Id == id,
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
                return CreateInvalidResponse<GetPaymentResponse>(Resources.Payment.PaymentNotFound);
            }

            return new GetPaymentResponse
            {
                Valid = true,
                Payment = new PaymentModel
                {
                    Id = payment.Id,
                    PaymentVendorId = payment.PaymentVendor.Id,
                    PaymentVendorName = payment.PaymentVendor.Name,
                    PaymentMethodId = payment.PaymentVendor.Id,
                    PaymentMethodName = payment.PaymentMethod.Name,
                    PaymentChannelId = payment.PaymentMethod.Id,
                    PaymentChannelName = payment.PaymentChannel.Name,
                    ChannelName = payment.ChannelName,
                    TransferLimitFrom = payment.TransferLimitFrom,
                    TransferLimitTo = payment.TransferLimitTo,
                    PlayerListCondition = payment.PlayerListCondition,
                    AllPlayers = payment.AllPlayers,
                    IdentityAuthentication = payment.IdentityAuthentication,
                    PhoneAuthentication = payment.PhoneAuthentication,
                    DepositedAmountUnlimited = payment.DepositedAmountUnlimited,
                    DepositedAmountMin = payment.DepositedAmountMin,
                    DepositedAmountMax = payment.DepositedAmountMax,
                    DesignatedUsers = payment.PaymentPlayerList.Count > 0,
                    PaymentStatus = payment.PaymentStatus,
                    Reason = payment.Reason,
                    Whitelist = payment.PaymentPlayerList.Where(p => p.ListType == PlayerListType.White)
                        .Select(p => p.Player.Username).ToArray(),
                    Blacklist = payment.PaymentPlayerList.Where(p => p.ListType == PlayerListType.Black)
                        .Select(p => p.Player.Username).ToArray(),
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
            var payment = await _paymentService.GetPaymentAsync(q => q.Id == id, p => p.AsNoTracking());

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

            if (await _paymentService.ExistsAsync(p => p.ChannelName == request.ChannelName))
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

            var payment = await _paymentService.GetPaymentAsync(q => q.Id == paymentId, tracking: true);

            if (payment == null)
            {
                return CreateInvalidResponse<CreateUpdatePaymentResponse>(Resources.Payment.NotFound);
            }

            if (await _paymentService.ExistsAsync(p => p.Id != paymentId && p.ChannelName == request.ChannelName))
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
            var payment = await _paymentService.GetPaymentAsync(q => q.Id == paymentId, tracking: true);

            if (payment == null)
            {
                return CreateInvalidResponse<CreateUpdatePaymentResponse>(Resources.Payment.NotFound);
            }

            await _paymentService.UpdatePaymentHandlingFeeAsync(payment, request, Username);

            return CreateValidResponse<MessageResponse>();
        }


        [HttpPost("upload")]
        [Description("Upload player file")]
        public async Task<MessageResponse> UploadPlayersAsync(IFormFile file)
        {
            var uploadedResult = await _uploadService.GetUploadedExcelAsync<UploadPlayerModel>(file);
            var users = uploadedResult.Where(p => !string.IsNullOrEmpty(p.Username))
                .Select(p => p.Username);

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
        public async Task<MessageResponse> ChangeOrderAsync(ChangeOrderRequest request)
        {
            await _paymentService.ChangeOrderAsync(request);

            return CreateValidResponse<MessageResponse>();
        }

        [HttpGet("{id}/DesignatedUsers")]
        [Description("Get Designated Users")]
        public async Task<GetDesignatedUsersResponse> GetDesignatedUsersAsync(Guid id)
        {
            var payment = await _paymentService.GetPaymentAsync(x => x.Id == id,
                x => x.Include(p => p.PaymentPlayerList)
                    .ThenInclude(p => p.Player)
                    .Include(p => p.VipLevels).AsNoTracking());

            if (payment == null)
            {
                return CreateInvalidResponse<GetDesignatedUsersResponse>(Resources.Payment.NotFound);
            }

            var players = GetDesignatedUsersAsync(payment);

            return new GetDesignatedUsersResponse
            {
                Valid = true,
                Players = players
            };
        }

        private static string[] GetDesignatedUsersAsync(Core.DataServices.Models.Payment payment)
        {
            var vipLevelIds = payment.VipLevels.Select(x => x.VipLevelId).ToArray();
            var playerCodes = payment.PaymentPlayerList.Where(x => x.ListType == PlayerListType.White)
                .Select(p => p.PlayerCode).ToArray();
            var isAll = payment.PlayerListCondition == PlayerListCondition.All;

            return payment.PaymentPlayerList
                .Where(x => playerCodes.Contains(x.PlayerCode))
                .Where(x =>
                    x.Player.AccumulatedDeposit >= payment.DepositedAmountMin &&
                    (payment.DepositedAmountUnlimited || x.Player.AccumulatedDeposit < payment.DepositedAmountMax))
                .Where(isAll && payment.IdentityAuthentication,
                    x => x.Player.IdentityAuthentication)
                .Where(isAll && payment.PhoneAuthentication,
                    x => x.Player.PhoneAuthentication)
                .Where(isAll && payment.VipLevels.Any(),
                    x => x.Player.VipId != null && vipLevelIds.Contains(x.Player.VipId.Value))
                .Select(x => x.Player.RealName).ToArray();
        }
    }
}