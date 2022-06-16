using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.BrandCertificate.Clients;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.Operations.Models.Services;

namespace TGS.Services.Operator.ApplicationServices.Player;

internal class CreateAgentInvitationPlayerCommandService : IGetService<CreatePlayerWithAgentInvitationCodeCommand, CreatePlayerInfo>
{
    private readonly IAgentOperations _agentOperations;
    private readonly IBrandOperations _brandOperations;
    private readonly IBrandServiceClient _brandServiceClient;
    private readonly IPasswordHandler _passwordHandler;
    private readonly IPlayerOperations _playerOperations;
    private readonly IPlayerRepository _playerRepository;

    public CreateAgentInvitationPlayerCommandService(
        IPlayerOperations playerOperations,
        IPasswordHandler passwordHandler,
        IBrandOperations brandOperations,
        IAgentOperations agentOperations,
        IPlayerRepository playerRepository,
        IBrandServiceClient brandServiceClient)
    {
        _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
        _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
        _agentOperations = agentOperations ?? throw new ArgumentNullException(nameof(agentOperations));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _brandServiceClient = brandServiceClient ?? throw new ArgumentNullException(nameof(brandServiceClient));
    }

    public async ValueTask<CreatePlayerInfo> GetAsync(
        CreatePlayerWithAgentInvitationCodeCommand command,
        CancellationToken cancellationToken = default)
    {
        var player = await _playerRepository
            .GetPlayerInfoByUserNameAsync(command.BrandId, command.UserName)
            .ConfigureAwait(false);

        if (player != null)
            return new CreatePlayerInfo(player.PlayerCode, false);

        var (salt, password) = _passwordHandler.GenerateCredential(command.Password);

        var brand = await _brandServiceClient
            .GetBrandInfoAsync(command.BrandId)
            .ConfigureAwait(false) ?? throw new BrandNotFoundException();

        var betLimitId = await _brandOperations
            .GetBrandDefaultBetLimitAsync(command.BrandId, command.CurrencyCode)
            .ConfigureAwait(false);

        var playerStatus = brand.SmsRegisterFlowEnabled || brand.EmailRegisterFlowEnabled || brand.BankInfoRegisterFlowEnabled
            ? PlayerStatus.InBlacklist
            : PlayerStatus.Enabled;

        var resultPlayerCode = await CreatePlayerByAgentAsync(
            command,
            salt!,
            password!,
            command.CurrencyCode,
            betLimitId,
            playerStatus).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(resultPlayerCode)
            && !brand.SmsRegisterFlowEnabled
            && !brand.EmailRegisterFlowEnabled
            && !brand.BankInfoRegisterFlowEnabled)
        {
            await _playerRepository
                .PlayerRegisterIpAsync(
                    resultPlayerCode,
                    command.IpAddress,
                    command.HostName,
                    DateTimeOffset.UtcNow)
                .ConfigureAwait(false);

            await _playerRepository
                .PlayerLoginIpAsync(
                    resultPlayerCode,
                    command.IpAddress,
                    command.HostName!)
                .ConfigureAwait(false);
        }

        return new CreatePlayerInfo(resultPlayerCode, true);
    }

    private async Task<string> CreatePlayerByAgentAsync(
        CreatePlayerWithAgentInvitationCodeCommand command,
        byte[] salt,
        byte[] password,
        string currency,
        int betLimitId,
        PlayerStatus status)
    {
        var agent = await _agentOperations
            .GetAgentByInvitationCodeAsync(command.BrandId, command.AgentInvitationCode)
            .ConfigureAwait(false);

        if (agent == null || agent.Status != AgentStatus.Enabled)
            throw new InvalidInvitationCodeException(
                command.AgentInvitationCode,
                "Can't find the agent info");

        var result = await _playerOperations
            .CreatePlayerAsync(
                new CreatePlayerData
                {
                    BrandId = command.BrandId,
                    UserName = command.UserName,
                    Salt = salt,
                    Password = password,
                    CurrencyCode = currency,
                    BetLimitId = betLimitId,
                    AgentId = agent.Id,
                    AgentInvitationCode = command.AgentInvitationCode,
                    Type = command.IsTestPlayer ? PlayerType.TestPlayer : PlayerType.RealPlayer,
                    PlayerStatus = status
                })
            .ConfigureAwait(false);

        return result;
    }
}
