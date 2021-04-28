using System;
using System.ComponentModel;
using System.Linq;
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

        [HttpGet]
        [Description("Get brand payments")]
        public Task<PagedResponse<BrandPaymentModel>>
            GetBrandPaymentsAsync([FromQuery] GetBrandPaymentsRequest request) =>
            _brandPaymentService.GetPaymentQueries()
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
                    PaymentName = x.PaymentMethodChannel.Payment.Code,
                    PaymentMethodId = x.PaymentMethodChannel.Method.Id,
                    PaymentMethodName = x.PaymentMethodChannel.Method.Code,
                    PaymentChannelId = x.PaymentMethodChannel.Channel.Id,
                    PaymentChannelName = x.PaymentMethodChannel.Channel.Code,
                    TransferType = x.TransferType,
                    ChannelName = x.ChannelName,
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
                        new() { FeePercentage = x.FeePercentage, HighestFee = x.HighestFee, LowestFee = x.LowestFee },
                    VipLevelIds = x.VipLevels.Select(p => p.VipLevelId).ToArray(),
                    CreatedByUsername = x.CreatedByUsername,
                    UpdatedByUsername = x.UpdatedByUsername,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn
                })
                .CreatePagedResponseAsync(request);

        //[HttpGet("{id}")]
        //[Description("Get a Payment")]
        //public async Task<GetBrandPaymentResponse> GetBrandPaymentMethodAsync(Guid id)
        //{
        //    var brandPayment = await _brandPaymentService.GetPaymentAsync(q => q.Id == id,
        //        p => p
        //            .Include(pm => pm.Payment)
        //            .Include(pm => pm.PaymentMethod)
        //            .Include(pm => pm.PaymentChannel)
        //            .Include(pm => pm.BrandPaymentPlayers)
        //            .ThenInclude(pp => pp.Player)
        //            .Include(pm => pm.VipLevels)
        //            .ThenInclude(pp => pp.VipLevel)
        //            .AsNoTracking());
        //    if (brandPayment == null)
        //    {
        //        return CreateInvalidResponse<GetBrandPaymentResponse>(Resources.BrandPayment.PaymentNotFound);
        //    }

        //    return new()
        //    {
        //        Valid = true,
        //        Payment = new()
        //        {
        //            Id = brandPayment.Id,
        //            BrandId = brandPayment.BrandId,
        //            PaymentId = brandPayment.Payment.Id,
        //            PaymentName = brandPayment.Payment.Name,
        //            PaymentMethodId = brandPayment.Payment.Id,
        //            PaymentMethodName = brandPayment.PaymentMethod.Name,
        //            PaymentChannelId = brandPayment.PaymentMethod.Id,
        //            PaymentChannelName = brandPayment.PaymentChannel.Name,
        //            ChannelName = brandPayment.ChannelName,
        //            TransferLimitFrom = brandPayment.TransferLimitFrom,
        //            TransferLimitTo = brandPayment.TransferLimitTo,
        //            PlayerListCondition = brandPayment.PlayerListCondition,
        //            AllPlayers = brandPayment.AllPlayers,
        //            IdentityAuthentication = brandPayment.IdentityAuthentication,
        //            PhoneAuthentication = brandPayment.PhoneAuthentication,
        //            DepositedAmountUnlimited = brandPayment.DepositedAmountUnlimited,
        //            DepositedAmountMin = brandPayment.DepositedAmountMin,
        //            DepositedAmountMax = brandPayment.DepositedAmountMax,
        //            DesignatedUsers = brandPayment.BrandPaymentPlayers.Count > 0,
        //            PaymentStatus = brandPayment.PaymentStatus,
        //            Reason = brandPayment.Reason,
        //            Whitelist = brandPayment.BrandPaymentPlayers.Where(p => p.ListType == PlayerListType.White)
        //                .Select(p => p.Player.Username).ToArray(),
        //            Blacklist = brandPayment.BrandPaymentPlayers.Where(p => p.ListType == PlayerListType.Black)
        //                .Select(p => p.Player.Username).ToArray(),
        //            VipLevelIds = brandPayment.VipLevels.Select(p => p.VipLevelId).ToArray(),
        //            CreatedByUsername = brandPayment.CreatedByUsername,
        //            UpdatedByUsername = brandPayment.UpdatedByUsername,
        //            CreatedOn = brandPayment.CreatedOn,
        //            UpdatedOn = brandPayment.UpdatedOn
        //        }
        //    };
        //}

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

            if (await _brandPaymentService.ExistsAsync(p => p.ChannelName == request.ChannelName))
            {
                return CreateInvalidResponse<CreateUpdateBrandPaymentResponse>(Resources.BrandPayment.NameExists);
            }

            await _brandPaymentService.CreatePaymentAsync(request, Username);
            return CreateValidResponse<CreateUpdateBrandPaymentResponse>();
        }

        [HttpGet("pmc")]
        [Description("Get All Brand PaymentMethodChannels")]
        public async Task<GenericResponse<AllPaymentMethodChannelsResponse>> GetAllPaymentMethodChannelsAsync(
            [FromServices] IQueryService<AllPaymentMethodChannelQuery, PaymentMethodChannelInfo> queryService,
            [FromServices] IQueryService<PaymentMethodQuery, PaymentMethodInfo> paymentMethodQueryService,
            [FromServices] IQueryService<PaymentChannelQuery, PaymentChannelInfo> paymentChannelQueryService)
        {
            var allPaymentMethodChannels = queryService.QueryAsync(new AllPaymentMethodChannelQuery());
            var allPaymentMethods = await paymentMethodQueryService
                .QueryAsync(new PaymentMethodQuery())
                .ToDictionaryAsync(pm => pm.Id)
                .ConfigureAwait(false);
            var allPaymentChannels = await paymentChannelQueryService
                .QueryAsync(new PaymentChannelQuery())
                .ToDictionaryAsync(pc => pc.Id)
                .ConfigureAwait(false);

            return new GenericResponse<AllPaymentMethodChannelsResponse>
            {
                Valid = true,
                Data = new AllPaymentMethodChannelsResponse
                {
                    AllData = allPaymentMethodChannels
                        .Select(pmc => new PaymentMethodChannelResponse
                        {
                            Id = pmc.Id,
                            PaymentId = pmc.PaymentId,
                            MethodId = pmc.MethodId,
                            ChannelId = pmc.ChannelId
                        })
                        .ToEnumerable(),
                    Methods = allPaymentMethodChannels
                        .GroupBy(pmc => pmc.MethodId)
                        .SelectAwait(async g => await g.FirstAsync().ConfigureAwait(false))
                        .Select(pmc => new PaymentMethodModel
                        {
                            Id = pmc.MethodId,
                            Code = allPaymentMethods[pmc.MethodId].Name
                        })
                        .ToEnumerable(),
                    Channels = allPaymentMethodChannels
                        .GroupBy(pmc => pmc.ChannelId)
                        .SelectAwait(async g => await g.FirstAsync().ConfigureAwait(false))
                        .Select(pmc => new PaymentChannelModel
                        {
                            Id = pmc.ChannelId,
                            Code = allPaymentChannels[pmc.ChannelId].Name
                        })
                        .ToEnumerable(),
                }
            };
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

            if (await _brandPaymentService.ExistsAsync(p => p.BrandId == brandPayment.BrandId &&
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
                ? new() { Valid = false, Message = Resources.BrandPayment.PlayersNotFound, NotExistsPlayers = diffUsers }
                : new UploadFileResponse { Valid = true, Players = allPlayers };
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

            var players = GetDesignatedUsers(brandPayment);

            return new() { Valid = true, Players = players };
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
                .Select(x => x.Player.RealName).ToArray();
        }
    }
}
