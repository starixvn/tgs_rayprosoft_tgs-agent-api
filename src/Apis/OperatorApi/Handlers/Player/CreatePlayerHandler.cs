using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
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
        private readonly IPasswordHandler _passwordHandler;
        private readonly IPlayerOperations _playerOperations;
        private readonly IStatusValidator _statusValidator;

        public CreatePlayerHandler(
            IHttpContextAccessor httpContextAccessor,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IBrandCurrencyCodeMapper currencyCodeMapper,
            IPasswordHandler passwordHandler,
            IStatusValidator statusValidator)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _passwordHandler = passwordHandler;
            _statusValidator = statusValidator;
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _currencyCodeMapper = currencyCodeMapper;
        }

        public async Task<CreatePlayerResponse> Handle(
            CreatePlayerRequest request, CancellationToken cancellationToken)
        {
            var brandId = _httpContext.Items.GetBrandId();
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var username = request.Username;
            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(brandId, username);
            if (playerCode.IsNotNullOrEmpty())
            {
                return new() { PlayerCode = playerCode, IsNew = false };
            }

            var (salt, password) = _passwordHandler.GenerateCredential(request.Password);

            var externalCurrency = request.CurrencyCode;
            var currency = await _currencyCodeMapper.ExternalToInternalAsync(externalCurrency, brandId);
            var betLimitId = await _brandOperations.GetBrandDefaultBetLimitAsync(brandId, currency);

            if (string.IsNullOrWhiteSpace(request.AgentInvitationCode))
            {
                var result = await _playerOperations.CreatePlayerAsync(new()
                {
                    BrandId = brandId,
                    Username = username,
                    Salt = salt,
                    Password = password,
                    CurrencyCode = currency,
                    ExternalCurrencyCode = externalCurrency,
                    BetLimitId = betLimitId,
                    AgentId = null,
                    AgentInvitationCode = null,
                    Type = request.IsTestPlayer ? PlayerType.TestPlayer : PlayerType.RealPlayer
                });

                return new()
                {
                    PlayerCode = result,
                    IsNew = true
                };
            }
            else
            {
                var agent = await _playerOperations.GetAgentAsync(request.AgentInvitationCode);
                if (agent == null)
                    throw new InvalidInvitationCodeException(request.AgentInvitationCode, "Can't find the agent info");

                var result = await _playerOperations.CreatePlayerAsync(new()
                {
                    BrandId = brandId,
                    Username = username,
                    Salt = salt,
                    Password = password,
                    CurrencyCode = currency,
                    ExternalCurrencyCode = externalCurrency,
                    BetLimitId = betLimitId,
                    AgentId = agent.Id,
                    AgentInvitationCode = request.AgentInvitationCode,
                    Type = request.IsTestPlayer ? PlayerType.TestPlayer : PlayerType.RealPlayer
                });

                return new()
                {
                    PlayerCode = result,
                    IsNew = true
                };
            }
        }
    }
}