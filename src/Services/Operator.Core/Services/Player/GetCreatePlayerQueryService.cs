using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Models.Services;
using TGS.Services.Operations.Validators;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Player
{
    internal class GetCreatePlayerQueryService : IGetService<GetCreatePlayerQuery, CreatePlayerInfo>
    {
        private readonly IStatusValidator _statusValidator;
        private readonly IPlayerOperations _playerOperations;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IBrandCurrencyCodeMapper _currencyCodeMapper;
        private readonly IBrandOperations _brandOperations;
        private readonly IAgentOperations _agentOperations;

        public GetCreatePlayerQueryService(
            IStatusValidator statusValidator,
            IPlayerOperations playerOperations,
            IPasswordHandler passwordHandler,
            IBrandCurrencyCodeMapper currencyCodeMapper,
            IBrandOperations brandOperations,
            IAgentOperations agentOperations)
        {
            _statusValidator = statusValidator ?? throw new ArgumentNullException(nameof(statusValidator));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
            _currencyCodeMapper = currencyCodeMapper ?? throw new ArgumentNullException(nameof(currencyCodeMapper));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _agentOperations = agentOperations ?? throw new ArgumentNullException(nameof(agentOperations));
        }

        public async ValueTask<CreatePlayerInfo> GetAsync(GetCreatePlayerQuery query, CancellationToken cancellationToken = default)
        {
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(query.BrandId, ValidStatus.Default);

            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(query.BrandId, query.UserName);
            if (playerCode.IsNotNullOrEmpty())
            {
                return new CreatePlayerInfo(playerCode, IsNew: false);
            }

            var (salt, password) = _passwordHandler.GenerateCredential(query.Password);

            var currency = await _currencyCodeMapper.ExternalToInternalAsync(query.CurrencyCode, query.BrandId);
            var betLimitId = await _brandOperations.GetBrandDefaultBetLimitAsync(query.BrandId, currency);

            if (string.IsNullOrWhiteSpace(query.AgentInvitationCode))
            {
                var result = await _playerOperations.CreatePlayerAsync(
                    new CreatePlayerData()
                    {
                        BrandId = query.BrandId,
                        Username = query.UserName,
                        Salt = salt,
                        Password = password,
                        CurrencyCode = currency,
                        ExternalCurrencyCode = query.CurrencyCode,
                        BetLimitId = betLimitId,
                        AgentId = null,
                        AgentInvitationCode = null,
                        Type = query.IsTestPlayer ? PlayerType.TestPlayer : PlayerType.RealPlayer
                    });

                return new CreatePlayerInfo(result, IsNew: true);
            }
            else
            {
                var agent = await _agentOperations.GetAgentByInvitationCodeAsync(query.AgentInvitationCode);
                if (agent == null)
                {
                    throw new InvalidInvitationCodeException(query.AgentInvitationCode, "Can't find the agent info");
                }

                var result = await _playerOperations.CreatePlayerAsync(
                    new CreatePlayerData()
                    {
                        BrandId = query.BrandId,
                        Username = query.UserName,
                        Salt = salt,
                        Password = password,
                        CurrencyCode = currency,
                        ExternalCurrencyCode = query.CurrencyCode,
                        BetLimitId = betLimitId,
                        AgentId = agent.Id,
                        AgentInvitationCode = query.AgentInvitationCode,
                        Type = query.IsTestPlayer ? PlayerType.TestPlayer : PlayerType.RealPlayer
                    });

                return new CreatePlayerInfo(result, IsNew: true);
            }
        }
    }
}