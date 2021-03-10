using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.BrandPayment;
using TGS.Apis.BackendOffice.DataService.Services;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

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

        [HttpGet]
        [Description("Get brand payments")]
        public Task<PagedResponse<BrandPaymentModel>> GetBrandPaymentsAsync([FromQuery] GetBrandPaymentsRequest request)
        {
            return _brandPaymentService.GetPaymentsAsync()
                .Include(p => p.BrandPaymentPlayers).ThenInclude(pp => pp.Player)
                .Include(p => p.VipLevels)
                .Where(request.TransferType, p => p.TransferType == request.TransferType)
                .Where(request.PaymentStatus, p => p.PaymentStatus == request.PaymentStatus)
                .Where(request.Keyword, p => EF.Functions.ILike(p.Payment.Name, $"%{request.Keyword}%") ||
                                             EF.Functions.ILike(p.PaymentMethod.Name, $"%{request.Keyword}%") ||
                                             EF.Functions.ILike(p.PaymentChannel.Name, $"%{request.Keyword}%"))
                .Select(p => new BrandPaymentModel
                {
                    Id = p.Id,
                    Number = p.DisplayOrder,
                    PaymentName = p.Payment.Name,
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
        public async Task<GetBrandPaymentResponse> GetBrandPaymentMethodAsync(Guid id)
        {
            var brandPayment = await _brandPaymentService.GetPaymentAsync(q => q.Id == id,
                p => p
                    .Include(pm => pm.Payment)
                    .Include(pm => pm.PaymentMethod)
                    .Include(pm => pm.PaymentChannel)
                    .Include(pm => pm.BrandPaymentPlayers)
                    .ThenInclude(pp => pp.Player)
                    .Include(pm => pm.VipLevels)
                    .ThenInclude(pp => pp.VipLevel)
                    .AsNoTracking());
            if (brandPayment == null)
            {
                return CreateInvalidResponse<GetBrandPaymentResponse>(Resources.BrandPayment.PaymentNotFound);
            }

            return new GetBrandPaymentResponse
            {
                Valid = true,
                Payment = new BrandPaymentModel
                {
                    Id = brandPayment.Id,
                    PaymentId = brandPayment.Payment.Id,
                    PaymentName = brandPayment.Payment.Name,
                    PaymentMethodId = brandPayment.Payment.Id,
                    PaymentMethodName = brandPayment.PaymentMethod.Name,
                    PaymentChannelId = brandPayment.PaymentMethod.Id,
                    PaymentChannelName = brandPayment.PaymentChannel.Name,
                    ChannelName = brandPayment.ChannelName,
                    TransferLimitFrom = brandPayment.TransferLimitFrom,
                    TransferLimitTo = brandPayment.TransferLimitTo,
                    PlayerListCondition = brandPayment.PlayerListCondition,
                    AllPlayers = brandPayment.AllPlayers,
                    IdentityAuthentication = brandPayment.IdentityAuthentication,
                    PhoneAuthentication = brandPayment.PhoneAuthentication,
                    DepositedAmountUnlimited = brandPayment.DepositedAmountUnlimited,
                    DepositedAmountMin = brandPayment.DepositedAmountMin,
                    DepositedAmountMax = brandPayment.DepositedAmountMax,
                    DesignatedUsers = brandPayment.BrandPaymentPlayers.Count > 0,
                    PaymentStatus = brandPayment.PaymentStatus,
                    Reason = brandPayment.Reason,
                    Whitelist = brandPayment.BrandPaymentPlayers.Where(p => p.ListType == PlayerListType.White)
                        .Select(p => p.Player.Username).ToArray(),
                    Blacklist = brandPayment.BrandPaymentPlayers.Where(p => p.ListType == PlayerListType.Black)
                        .Select(p => p.Player.Username).ToArray(),
                    VipLevelIds = brandPayment.VipLevels.Select(p => p.VipLevelId).ToArray(),
                    CreatedByUsername = brandPayment.CreatedByUsername,
                    UpdatedByUsername = brandPayment.UpdatedByUsername,
                    CreatedOn = brandPayment.CreatedOn,
                    UpdatedOn = brandPayment.UpdatedOn
                }
            };
        }

        [HttpGet("{id}/handingFee")]
        [Description("Get a Payment handing Fee")]
        public async Task<GetPaymentFeeResponse> GetBrandPaymentHandingFeeAsync(Guid id)
        {
            var brandPayment = await _brandPaymentService.GetPaymentAsync(q => q.Id == id, p => p.AsNoTracking());

            return new GetPaymentFeeResponse
            {
                Valid = true,
                PaymentFee = new PaymentFeeModel
                {
                    FeePercentage = brandPayment.FeePercentage,
                    HighestFee = brandPayment.HighestFee,
                    LowestFee = brandPayment.LowestFee
                }
            };
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
                    return new CreateUpdateBrandPaymentResponse
                    {
                        Valid = false,
                        Message = Resources.BrandPayment.WhiteSameAsBlack,
                        DuplicatePlayers = duplicatePlayers
                    };
                }
            }

            if (await _brandPaymentService.ExistsAsync(p => p.ChannelName == request.ChannelName))
            {
                return CreateInvalidResponse<CreateUpdateBrandPaymentResponse>(Resources.BrandPayment.NameExists);
            }

            await _brandPaymentService.CreatePaymentAsync(request, Username);
            return CreateValidResponse<CreateUpdateBrandPaymentResponse>();
        }

        [HttpPut("{brandPaymentId:guid}")]
        [Description("Update a Payment")]
        public async Task<CreateUpdateBrandPaymentResponse> UpdateBrandPaymentAsync(Guid brandPaymentId,
            UpdateBrandPaymentRequest request)
        {
            if (request.Whitelist != null && request.Blacklist != null)
            {
                var duplicatePlayers = request.Whitelist.Intersect(request.Blacklist).ToArray();
                if (duplicatePlayers.Any())
                {
                    return new CreateUpdateBrandPaymentResponse
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

            if (await _brandPaymentService.ExistsAsync(p =>
                p.Id != brandPaymentId && p.ChannelName == request.ChannelName))
            {
                return CreateInvalidResponse<CreateUpdateBrandPaymentResponse>(Resources.BrandPayment.NameExists);
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
                ? new UploadFileResponse
                {
                    Valid = false,
                    Message = Resources.BrandPayment.PlayersNotFound,
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
            await _brandPaymentService.ChangeOrderAsync(request);

            return CreateValidResponse<MessageResponse>();
        }

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

            var players = GetDesignatedUsersAsync(brandPayment);

            return new GetDesignatedUsersResponse
            {
                Valid = true,
                Players = players
            };
        }

        private static string[] GetDesignatedUsersAsync(Core.DataServices.Tgs.Models.BrandPayment brandPayment)
        {
            var vipLevelIds = brandPayment.VipLevels.Select(x => x.VipLevelId).ToArray();
            var playerCodes = brandPayment.BrandPaymentPlayers.Where(x => x.ListType == PlayerListType.White)
                .Select(p => p.PlayerCode).ToArray();
            var isAll = brandPayment.PlayerListCondition == PlayerListCondition.All;

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
                .Select(x => x.Player.RealName).ToArray();
        }
    }
}