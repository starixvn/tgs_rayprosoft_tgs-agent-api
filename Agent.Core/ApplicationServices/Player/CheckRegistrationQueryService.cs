using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.BrandCertificate.Clients;
using Tgs.PlayerService.Client;
using Tgs.ServiceUtility;
using TGS.Services.Agent.ApplicationServices.Player.PlayerRegistrationConditions;
using TGS.Services.Agent.Exceptions;

namespace TGS.Services.Agent.ApplicationServices.Player;

internal class CheckRegistrationQueryService(
    IBrandServiceClient brandServiceClient,
    IPlayerServiceClient playerServiceClient,
    Func<string, IRegistrationCondition?> conditionFunc)
    : IGetService<CheckRegistrationQuery, bool>
{
    private readonly IBrandServiceClient _brandServiceClient = brandServiceClient ?? throw new ArgumentNullException(nameof(brandServiceClient));
    private readonly IPlayerServiceClient _playerServiceClient = playerServiceClient ?? throw new ArgumentNullException(nameof(playerServiceClient));
    private readonly Func<string, IRegistrationCondition?> _conditionFunc = conditionFunc ?? throw new ArgumentNullException(nameof(conditionFunc));

    public async ValueTask<bool> GetAsync(
        CheckRegistrationQuery query,
        CancellationToken cancellationToken = default)
    {
        var brandFrontendInfo = await _brandServiceClient
            .GetBrandFrontendInfoAsync(query.BrandId, cancellationToken)
            .ConfigureAwait(false);

        var player = await _playerServiceClient
            .GetPlayerInfoByUserNameAsync(query.BrandId, query.Username, cancellationToken)
            .ConfigureAwait(false);

        if (player != null)
            throw new PlayerExistAlreadyException(query.Username);

        if (brandFrontendInfo.PlayerRegisterFlowSettings is null)
        {
            return true;
        }

        foreach (var type in brandFrontendInfo.PlayerRegisterFlowSettings.RootElement.EnumerateArray().Select(element => element.GetProperty("type").GetString()))
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new InvalidOperationException("RegisterFlowStep type is not specified");

            var registerService =  _conditionFunc.Invoke(type);

            if (registerService != null)
                await registerService.ExecuteAsync(query.BrandId, query.Data, cancellationToken);
        }

        return true;
    }
}
