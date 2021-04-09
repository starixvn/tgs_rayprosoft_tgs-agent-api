using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Apis.OperatorApi.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.OperatorApi.Handlers.Player
{
    public sealed class CreatePlayerHandler : IRequestHandler<CreatePlayerRequest, CreatePlayerResponse>
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IBrandCurrencyCodeMapper _currencyCodeMapper;
        private readonly HttpContext _httpContext;
        private readonly IInvitationCodeValidator _invitationCodeValidator;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IPlayerOperations _playerOperations;
        private readonly IStatusValidator _statusValidator;

        public CreatePlayerHandler(
            IHttpContextAccessor httpContextAccessor,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IBrandCurrencyCodeMapper currencyCodeMapper,
            IPasswordHandler passwordHandler,
            IStatusValidator statusValidator,
            IInvitationCodeValidator invitationCodeValidator)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _passwordHandler = passwordHandler;
            _statusValidator = statusValidator;
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _currencyCodeMapper = currencyCodeMapper;
            _invitationCodeValidator = invitationCodeValidator;
        }

        async Task<CreatePlayerResponse> IRequestHandler<CreatePlayerRequest, CreatePlayerResponse>.Handle(
            CreatePlayerRequest request, CancellationToken cancellationToken)
        {
            var brandId = _httpContext.Items.GetBrandId();
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var username = request.Username;
            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(brandId, username);
            if (playerCode.IsNotNullOrEmpty())
            {
                return new() {PlayerCode = playerCode, IsNew = false};
            }

            var externalCurrency = request.CurrencyCode;
            var currency = await _currencyCodeMapper.ExternalToInternalAsync(externalCurrency, brandId);

            var (salt, password) = _passwordHandler.GenerateCredential(request.Password);

            var betLimitId = await _brandOperations.GetBrandDefaultBetLimitAsync(brandId, currency);

            var agent = request.AgentInvitationCode == null
                ? null
                : await _playerOperations.GetAgentAsync(request.AgentInvitationCode);

            playerCode = await _playerOperations.CreatePlayerAsync(new()
            {
                BrandId = brandId,
                Username = username,
                Salt = salt,
                Password = password,
                CurrencyCode = currency,
                ExternalCurrencyCode = externalCurrency,
                BetLimitId = betLimitId,
                AgentId = agent?.Id,
                AgentInvitationCode = request.AgentInvitationCode,
                Type = request.IsTestPlayer ? PlayerType.TestPlayer : PlayerType.RealPlayer
            });

            return new() {PlayerCode = playerCode, IsNew = true};
        }
    }
}