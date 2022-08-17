using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.DepositAccumulateRoulette.Clients;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.ApplicationServices.Promotions;

internal class GetDepositAccumulateRouletteSettingQueryService : IGetService<GetDepositAccumulateRouletteSettingQuery, RouletteSetting?>
{
    private readonly IDepositAccumulateRouletteClient _depositAccumulateRouletteClient;

    public GetDepositAccumulateRouletteSettingQueryService(
        IDepositAccumulateRouletteClient depositAccumulateRouletteClient)
    {
        _depositAccumulateRouletteClient = depositAccumulateRouletteClient ?? throw new ArgumentNullException(nameof(depositAccumulateRouletteClient));
    }

    public ValueTask<RouletteSetting?> GetAsync(
        GetDepositAccumulateRouletteSettingQuery query,
        CancellationToken cancellationToken = default)
        => _depositAccumulateRouletteClient
            .GetRouletteSettingAsync(query.PromotionId, cancellationToken);
}
