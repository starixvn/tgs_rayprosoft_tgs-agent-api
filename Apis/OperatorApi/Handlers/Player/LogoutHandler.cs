using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.OperatorApi.Handlers.Player
{
    public sealed class LogoutHandler : IRequestHandler<LogoutRequest, LogoutResponse>
    {
        private readonly HttpContext _httpContext;
        private readonly IPlayerOperations _playerOperations;
        private readonly IStatusValidator _statusValidator;

        public LogoutHandler(
            IHttpContextAccessor httpContextAccessor,
            IPlayerOperations playerOperations,
            IStatusValidator statusValidator)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _statusValidator = statusValidator;
            _playerOperations = playerOperations;
        }

        async Task<LogoutResponse> IRequestHandler<LogoutRequest, LogoutResponse>.Handle(LogoutRequest request, CancellationToken cancellationToken)
        {
            var brandId = _httpContext.Items.GetBrandId();
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var playerTokenData = _httpContext.Items.GetPlayerTokenData();
            var playerCode = playerTokenData.PlayerCode;

            var cachedPlayer = await _playerOperations.GetCachedPlayerAsync(playerTokenData.PlayerCode);
            if (cachedPlayer == null)
            {
                throw new PlayerNotFoundException(playerCode, Resources.Player.NotFound);
            }

            var updatedOn = DateTimeOffset.UtcNow;
            await _playerOperations.PlayerLogoutAsync(new()
            {
                Username = cachedPlayer.Username,
                PlayerCode = playerCode,
                LoginRecordId = playerTokenData.LoginRecordId,
                UpdatedOn = updatedOn
            });

            return new()
            {
                UpdateTime = updatedOn
            };
        }
    }
}
