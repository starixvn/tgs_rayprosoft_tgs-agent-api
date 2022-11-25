using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.BrandCertificate.Clients;
using Tgs.PlayerService.Client;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Player;

internal class CreatePlayerQueryService : IGetService<CreatePlayerQuery, string>
{
    private readonly IPlayerServiceClient _playerServiceClient;
    private readonly IAgentRepository _agentRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly IBrandServiceClient _brandServiceClient;

    public CreatePlayerQueryService(
        IPlayerServiceClient playerServiceClient,
        IAgentRepository agentRepository,
        IBrandRepository brandRepository,
        IBrandServiceClient brandServiceClient)
    {
        _playerServiceClient = playerServiceClient ?? throw new ArgumentNullException(nameof(playerServiceClient));
        _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        _brandServiceClient = brandServiceClient ?? throw new ArgumentNullException(nameof(brandServiceClient));
    }

    public async ValueTask<string> GetAsync(
        CreatePlayerQuery query,
        CancellationToken cancellationToken = default)
    {
        var player = await _playerServiceClient
            .GetPlayerInfoByUserNameAsync(query.BrandId, query.Username, cancellationToken)
            .ConfigureAwait(false);

        if (player != null)
            return player.PlayerCode;

        var agent = await _agentRepository
            .GetAgentByIdAsync(query.AgentId);

        var brand = await _brandServiceClient
            .GetBrandInfoAsync(query.BrandId, cancellationToken)
            .ConfigureAwait(false);

        var vipInfo = await _brandServiceClient
            .GetVipLevelInfosAsync(query.BrandId, cancellationToken)
            .FirstOrDefaultAsync(x => x.Level == 1, cancellationToken)
            .ConfigureAwait(false);

        var betLimitId = await _brandRepository
            .GetBrandDefaultBetLimitIdAsync(query.BrandId, cancellationToken)
            .ConfigureAwait(false);

        var playerData = new CreatePlayerData(
            query.BrandId,
            query.AgentId,
            vipInfo!.Id,
            brand.Currency,
            query.Username,
            query.Password,
            betLimitId,
            PlayerType.RealPlayer,
            agent!.Username
        );

        var playerCode = await _playerServiceClient
            .CreatePlayerAsync(playerData, cancellationToken)
            .ConfigureAwait(false);

        await _playerServiceClient
            .SetPlayerStatusAsync(playerCode, PlayerStatus.Enabled, cancellationToken)
            .ConfigureAwait(false);

        return playerCode;
    }
}
