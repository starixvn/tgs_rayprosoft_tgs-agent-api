using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.OperatorApi.Handlers.Player
{
    public sealed class GetPlayerHandler : IRequestHandler<GetPlayerRequest, GetPlayerResponse>
    {
        private readonly HttpContext _httpContext;
        private readonly IPlayerOperations _playerOperations;
        private readonly IStatusValidator _statusValidator;

        public GetPlayerHandler(
            IHttpContextAccessor httpContextAccessor,
            IPlayerOperations playerOperations,
            IStatusValidator statusValidator)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _statusValidator = statusValidator;
            _playerOperations = playerOperations;
        }

        async Task<GetPlayerResponse> IRequestHandler<GetPlayerRequest, GetPlayerResponse>.Handle(
            GetPlayerRequest request, CancellationToken cancellationToken)
        {
            var brandId = _httpContext.Items.GetBrandId();
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var playerTokenData = _httpContext.Items.GetPlayerTokenData();
            var playerCode = playerTokenData.PlayerCode;

            var player = await _playerOperations.GetPlayerByCodeAsync(playerCode);
            if (player == null)
            {
                throw new PlayerNotFoundException(playerCode, Resources.Player.NotFound);
            }

            return new()
            {
                Data = new()
                {
                    Username = player.Username,
                    RealName = player.RealName,
                    Email = player.Email,
                    Phone = player.Phone,
                    QQ = player.QQ,
                    Wechat = player.Wechat,
                    IsSetPayPassword = player.WithdrawPassword.Length > 0,
                    IdentityAuthentication = player.IdentityAuthentication
                }
            };
        }
    }
}