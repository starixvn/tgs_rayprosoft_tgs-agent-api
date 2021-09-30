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
    internal class CreatePlayerCommandService : IGetService<CreatePlayerCommand, CreatePlayerInfo>
    {
        private readonly IAgentOperations _agentOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly IBrandCurrencyCodeMapper _currencyCodeMapper;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IPlayerOperations _playerOperations;
        private readonly IPlayerRepository _playerRepository;
        private readonly IStatusValidator _statusValidator;
        private readonly ISysRepository _sysRepository;

        public CreatePlayerCommandService(
            IStatusValidator statusValidator,
            IPlayerOperations playerOperations,
            IPasswordHandler passwordHandler,
            IBrandCurrencyCodeMapper currencyCodeMapper,
            IBrandOperations brandOperations,
            IAgentOperations agentOperations,
            IPlayerRepository playerRepository,
            ISysRepository sysRepository)
        {
            _statusValidator = statusValidator ?? throw new ArgumentNullException(nameof(statusValidator));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
            _currencyCodeMapper = currencyCodeMapper ?? throw new ArgumentNullException(nameof(currencyCodeMapper));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _agentOperations = agentOperations ?? throw new ArgumentNullException(nameof(agentOperations));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _sysRepository = sysRepository ?? throw new ArgumentNullException(nameof(sysRepository));
        }

        public async ValueTask<CreatePlayerInfo> GetAsync(CreatePlayerCommand command,
            CancellationToken cancellationToken = default)
        {
            await _statusValidator
                .ThrowIfBrandStatusIsNotOneOfAsync(command.BrandId, ValidStatus.Default)
                .ConfigureAwait(false);

            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(command.BrandId, command.UserName)
                .ConfigureAwait(false);

            if (playerCode.IsNotNullOrEmpty())
                return new CreatePlayerInfo(playerCode, false);

            var (salt, password) = _passwordHandler.GenerateCredential(command.Password);

            var brand = await _sysRepository.GetBrandInfoAsync(command.BrandId).ConfigureAwait(false);

            if (brand == null)
                throw new BrandNotFoundException();

            var currency = await _currencyCodeMapper.ExternalToInternalAsync(command.CurrencyCode, command.BrandId);
            var betLimitId = await _brandOperations.GetBrandDefaultBetLimitAsync(command.BrandId, currency)
                .ConfigureAwait(false);

            var playerStatus = brand.Code.Equals("HKJP")
                ? PlayerStatus.InBlacklist
                : PlayerStatus.Enabled;

            var resultPlayerCode = default(string);

            if (string.IsNullOrWhiteSpace(command.AgentInvitationCode))
            {
                resultPlayerCode = await CreatePlayerAsync(
                    command,
                    salt,
                    password,
                    currency,
                    betLimitId,
                    playerStatus).ConfigureAwait(false);
            }
            else
            {
                resultPlayerCode = await CreatePlayerByAgentAsync(
                    command,
                    salt,
                    password,
                    currency,
                    betLimitId,
                    playerStatus).ConfigureAwait(false);
            }

            if (!string.IsNullOrEmpty(resultPlayerCode))
            {
                var player = await _playerOperations.GetPlayerByCodeAsync(resultPlayerCode)
                    .ConfigureAwait(false);

                await _playerRepository
                    .PlayerRegisterIpAsync(
                        resultPlayerCode,
                        command.IpAddress,
                        player.RegisteredOn)
                    .ConfigureAwait(false);
            }

            return new CreatePlayerInfo(resultPlayerCode, true);
        }

        private Task<string> CreatePlayerAsync(
            CreatePlayerCommand query,
            byte[]? salt,
            byte[]? password,
            string currency,
            int betLimitId,
            PlayerStatus status)
            => _playerOperations.CreatePlayerAsync(
                new CreatePlayerData
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
                    Type = query.IsTestPlayer ? PlayerType.TestPlayer : PlayerType.RealPlayer,
                    PlayerStatus = status
                });

        private async Task<string> CreatePlayerByAgentAsync(
            CreatePlayerCommand query,
            byte[]? salt,
            byte[]? password,
            string currency,
            int betLimitId,
            PlayerStatus status)
        {
            var agent = await _agentOperations
                .GetAgentByInvitationCodeAsync(query.BrandId, query.AgentInvitationCode)
                .ConfigureAwait(false);

            if (agent == null || agent.Status != AgentStatus.Enabled)
                throw new InvalidInvitationCodeException(
                    query.AgentInvitationCode,
                    "Can't find the agent info");

            var result = await _playerOperations
                .CreatePlayerAsync(
                    new CreatePlayerData
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
                        Type = query.IsTestPlayer ? PlayerType.TestPlayer : PlayerType.RealPlayer,
                        PlayerStatus = status
                    })
                .ConfigureAwait(false);
            return result;
        }
    }
}
