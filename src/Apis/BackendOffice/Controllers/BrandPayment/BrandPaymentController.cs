using System;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.BrandPayment;
using TGS.Apis.BackendOffice.Controllers.Messages.PaymentChannel;
using TGS.Apis.BackendOffice.Controllers.Messages.PaymentMethod;
using TGS.Apis.BackendOffice.DataService.Services;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Queries.BrandPayment;
using TGS.Services.BackOffice.Application.Queries.PaymentChannel;
using TGS.Services.BackOffice.Application.Queries.PaymentMethod;
using TGS.Services.Payment;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers.BrandPayment
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class BrandPaymentController : BaseContextController
    {
        private readonly ITgsQueries<BrandPaymentAuditLog> _auditLogQueries;
        private readonly IBrandPaymentService _brandPaymentService;
        private readonly ITgsQueries<Player> _playerQueries;
        private readonly IUploadService _uploadService;

        public BrandPaymentController(
            ITgsQueries<Player> playerQueries,
            IUploadService uploadService,
            IBrandPaymentService brandPaymentService,
            ITgsQueries<BrandPaymentAuditLog> auditLogQueries)
        {
            _uploadService = uploadService;
            _playerQueries = playerQueries;
            _brandPaymentService = brandPaymentService;
            _auditLogQueries = auditLogQueries;
        }

        [HttpPut("ChangeOrders")]
        [Description("Change Payment Order")]
        public async Task<MessageResponse> ChangeOrderAsync(ChangeOrderRequest request)
        {
            await _brandPaymentService.ChangeOrderAsync(request);

            return CreateValidResponse<MessageResponse>();
        }

        [HttpPost]
        [Description("Create a Payment")]
        public async Task<CreateUpdateBrandPaymentResponse> CreateBrandPaymentAsync(CreateBrandPaymentRequest request)
        {
            if (request.Whitelist != null && request.Blacklist != null)
            {
                var duplicatePlayers = request.Whitelist.Intersect(request.Blacklist).ToArray();
                if (duplicatePlayers.Any())
                {
                    return new()
                    {
                        Valid = false,
                        Message = Resources.BrandPayment.WhiteSameAsBlack,
                        DuplicatePlayers = duplicatePlayers
                    };
                }
            }

            await _brandPaymentService.CreatePaymentAsync(request, Username);
            return CreateValidResponse<CreateUpdateBrandPaymentResponse>();
        }

        [HttpGet("{id}/handingFee")]
        [Description("Get a Payment handing Fee")]
        public async Task<GetPaymentFeeResponse> GetBrandPaymentHandingFeeAsync(Guid id)
        {
            var brandPayment = await _brandPaymentService.GetPaymentAsync(q => q.Id == id, p => p.AsNoTracking());

            return new()
            {
                Valid = true,
                PaymentFee = new()
                {
                    FeePercentage = brandPayment.FeePercentage,
                    HighestFee = brandPayment.HighestFee,
                    LowestFee = brandPayment.LowestFee
                }
            };
        }

        [HttpGet]
        [Description("Get brand payments")]
        public Task<PagedResponse<BrandPaymentModel>> GetBrandPaymentsAsync([FromQuery] GetBrandPaymentsRequest request)
            => _brandPaymentService.GetPaymentQueries()
                .Include(p => p.BrandPaymentPlayers)
                .ThenInclude(pp => pp.Player)
                .Include(p => p.VipLevels)
                .Include(p => p.PaymentMethodChannel)
                .ThenInclude(pmc => pmc.Payment)
                .Include(p => p.PaymentMethodChannel)
                .ThenInclude(pmc => pmc.Method)
                .Include(p => p.PaymentMethodChannel)
                .ThenInclude(pmc => pmc.Channel)
                .Where(ResellerId, x => x.Brand.Licensee.ResellerId == ResellerId)
                .Where(LicenseeId, x => x.Brand.LicenseeId == LicenseeId)
                .Where(BrandId, x => x.BrandId == BrandId)
                .Where(request.BrandId, x => x.BrandId == request.BrandId)
                .Where(request.TransferType, x => x.TransferType == request.TransferType)
                .Where(request.PaymentStatus, x => x.PaymentStatus == request.PaymentStatus)
                .Select(x => new BrandPaymentModel
                {
                    Id = x.Id,
                    BrandId = x.BrandId,
                    Number = x.DisplayOrder,
                    PaymentId = x.PaymentMethodChannel.Payment.Id,
                    PaymentName = x.PaymentMethodChannel.Payment.Code,
                    PaymentMethodId = x.PaymentMethodChannel.Method.Id,
                    PaymentMethodName = x.PaymentMethodChannel.Method.Code,
                    PaymentChannelId = x.PaymentMethodChannel.Channel.Id,
                    PaymentChannelName = x.PaymentMethodChannel.Channel.Code,
                    PaymentMethodChannelId = x.PaymentMethodChannel.Id,
                    TransferType = x.TransferType,
                    ChannelName = x.ChannelName,
                    ExtendedData = JsonDocument.Parse(x.ExtendedData, default),
                    PlayerListCondition = x.PlayerListCondition,
                    AllPlayers = x.AllPlayers,
                    DepositedAmountMin = x.DepositedAmountMin,
                    DepositedAmountMax = x.DepositedAmountMax,
                    DepositedAmountUnlimited = x.DepositedAmountUnlimited,
                    IdentityAuthentication = x.IdentityAuthentication,
                    PhoneAuthentication = x.PhoneAuthentication,
                    TransferLimitFrom = x.TransferLimitFrom,
                    TransferLimitTo = x.TransferLimitTo,
                    PaymentStatus = x.PaymentStatus,
                    DesignatedUsers = GetDesignatedUsers(x).Any(),
                    PaymentFee =
                        new() {FeePercentage = x.FeePercentage, HighestFee = x.HighestFee, LowestFee = x.LowestFee},
                    VipLevelIds = x.VipLevels.Select(p => p.VipLevelId).ToArray(),
                    CreatedByUsername = x.CreatedByUsername,
                    UpdatedByUsername = x.UpdatedByUsername,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn,
                    BankType = x.PaymentMethodChannel.Method.BankType
                })
                .CreatePagedResponseAsync(request);

        [HttpGet("{id}/DesignatedUsers")]
        [Description("Get Designated Users")]
        public async Task<GetDesignatedUsersResponse> GetDesignatedUsersAsync(Guid id)
        {
            var brandPayment = await _brandPaymentService.GetPaymentAsync(x => x.Id == id,
                x => x.Include(p => p.BrandPaymentPlayers)
                    .ThenInclude(p => p.Player)
                    .Include(p => p.VipLevels).AsNoTracking());

            if (brandPayment == null)
            {
                return CreateInvalidResponse<GetDesignatedUsersResponse>(Resources.BrandPayment.NotFound);
            }

            var players = GetDesignatedUsers(brandPayment);

            return new() {Valid = true, Players = players};
        }

        [HttpPut("{brandPaymentId:guid}")]
        [Description("Update a Payment")]
        public async Task<CreateUpdateBrandPaymentResponse> UpdateBrandPaymentAsync(
            Guid brandPaymentId,
            UpdateBrandPaymentRequest request)
        {
            if (request.Whitelist != null && request.Blacklist != null)
            {
                var duplicatePlayers = request.Whitelist.Intersect(request.Blacklist).ToArray();
                if (duplicatePlayers.Any())
                {
                    return new()
                    {
                        Valid = false,
                        Message = Resources.BrandPayment.WhiteSameAsBlack,
                        DuplicatePlayers = duplicatePlayers
                    };
                }
            }

            var brandPayment = await _brandPaymentService.GetPaymentAsync(q => q.Id == brandPaymentId, tracking: true);

            if (brandPayment == null)
            {
                return CreateInvalidResponse<CreateUpdateBrandPaymentResponse>(Resources.BrandPayment.NotFound);
            }

            await _brandPaymentService.UpdatePaymentAsync(brandPayment, request, Username);

            return CreateValidResponse<CreateUpdateBrandPaymentResponse>();
        }

        [HttpPut("{brandPaymentId:guid}/handingFee")]
        [Description("Update a Payment handing Fee")]
        public async Task<MessageResponse> UpdateBrandPaymentHandlingFeeAsync(Guid brandPaymentId,
            UpdatePaymentFeeRequest request)
        {
            var brandPayment = await _brandPaymentService.GetPaymentAsync(q => q.Id == brandPaymentId, tracking: true);

            if (brandPayment == null)
            {
                return CreateInvalidResponse<CreateUpdateBrandPaymentResponse>(Resources.BrandPayment.NotFound);
            }

            await _brandPaymentService.UpdatePaymentHandlingFeeAsync(brandPayment, request, Username);

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
                ? new() {Valid = false, Message = Resources.BrandPayment.PlayersNotFound, NotExistsPlayers = diffUsers}
                : new UploadFileResponse {Valid = true, Players = allPlayers};
        }

        private static string[] GetDesignatedUsers(Core.DataServices.Tgs.Models.BrandPayment brandPayment)
        {
            var vipLevelIds = brandPayment.VipLevels.Select(x => x.VipLevelId).ToArray();
            var playerCodes = brandPayment.BrandPaymentPlayers.Where(x => x.ListType == PlayerListType.White)
                .Select(p => p.PlayerCode).ToArray();
            var isAll = brandPayment.PlayerListCondition == PlayerListCondition.MatchAll;

            return brandPayment.BrandPaymentPlayers
                .Where(x => playerCodes.Contains(x.PlayerCode))
                .Where(x =>
                    x.Player.AccumulatedDeposit >= brandPayment.DepositedAmountMin &&
                    (brandPayment.DepositedAmountUnlimited ||
                        x.Player.AccumulatedDeposit < brandPayment.DepositedAmountMax))
                .Where(isAll && brandPayment.IdentityAuthentication,
                    x => x.Player.IdentityAuthentication)
                .Where(isAll && brandPayment.PhoneAuthentication,
                    x => x.Player.PhoneAuthentication)
                .Where(isAll && brandPayment.VipLevels.Any(),
                    x => x.Player.VipId != null && vipLevelIds.Contains(x.Player.VipId.Value))
                .Select(x => x.Player.Username).ToArray();
        }
    }
}