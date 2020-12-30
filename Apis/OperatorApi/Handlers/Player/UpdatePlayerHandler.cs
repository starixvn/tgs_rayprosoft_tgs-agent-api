using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Models;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.OperatorApi.Handlers.Player
{
    public sealed class UpdatePlayerHandler : IRequestHandler<UpdatePlayerRequest, UpdatePlayerResponse>
    {
        private readonly HttpContext _httpContext;
        private readonly IPlayerOperations _playerOperations;
        private readonly IStatusValidator _statusValidator;

        public UpdatePlayerHandler(
            IHttpContextAccessor httpContextAccessor,
            IPlayerOperations playerOperations,
            IStatusValidator statusValidator)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _statusValidator = statusValidator;
            _playerOperations = playerOperations;
        }

        async Task<UpdatePlayerResponse> IRequestHandler<UpdatePlayerRequest, UpdatePlayerResponse>.Handle(
            UpdatePlayerRequest request, CancellationToken cancellationToken)
        {
            var brandId = _httpContext.Items.GetBrandId();
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var playerTokenData = _httpContext.Items.GetPlayerTokenData();
            var cachedPlayer = await _playerOperations.GetCachedPlayerAsync(playerTokenData.PlayerCode);

            var updatedOn = DateTimeOffset.UtcNow;

            await _playerOperations.UpdatePlayerAsync(new()
            {
                Username = cachedPlayer.Username,
                PlayerCode = cachedPlayer.PlayerCode,
                RealName = request.RealName,
                Phone = request.Phone,
                Email = request.Email,
                QQ = request.QQ,
                Wechat = request.Wechat,
                UpdatedOn = updatedOn
            });

            return new()
            {
                UpdateTime = updatedOn
            };
        }
    }
}