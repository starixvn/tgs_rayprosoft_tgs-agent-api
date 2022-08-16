using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Tgs.DepositAccumulateRoulette.Clients;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.ApplicationServices.Promotions;

internal class DepositAccumulateRouletteSettingQueryService : IQueryService<DepositAccumulateRouletteSettingQuery, RouletteItem>
{
    private readonly IDepositAccumulateRouletteClient _depositAccumulateRouletteClient;

    public DepositAccumulateRouletteSettingQueryService(
        IDepositAccumulateRouletteClient depositAccumulateRouletteClient)
    {
        _depositAccumulateRouletteClient = depositAccumulateRouletteClient ?? throw new ArgumentNullException(nameof(depositAccumulateRouletteClient));
    }

    public async IAsyncEnumerable<RouletteItem> QueryAsync(
        DepositAccumulateRouletteSettingQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var settings = await _depositAccumulateRouletteClient
            .GetRouletteSettingAsync(query.PromotionId, cancellationToken)
            .ConfigureAwait(false);

        if (settings is null)
            yield break;

        foreach (var setting in settings.Items)
            yield return new RouletteItem(setting.Id, setting.Title);
    }
}
