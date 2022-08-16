using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.DepositAccumulateRoulette.Clients;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.ApplicationServices.Promotions;

internal class DepositAccumulateRouletteCommandService : IGetService<DepositAccumulateRouletteCommand, RouletteResultWithCount>
{
    private readonly IDepositAccumulateRouletteClient _depositAccumulateRouletteClient;

    public DepositAccumulateRouletteCommandService(
        IDepositAccumulateRouletteClient depositAccumulateRouletteClient)
    {
        _depositAccumulateRouletteClient = depositAccumulateRouletteClient ?? throw new ArgumentNullException(nameof(depositAccumulateRouletteClient));
    }

    public async ValueTask<RouletteResultWithCount> GetAsync(
        DepositAccumulateRouletteCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _depositAccumulateRouletteClient.RouletteAsync(
            command.PromotionId,
            command.PlayerCode,
            cancellationToken).ConfigureAwait(false);

        return new RouletteResultWithCount(result.Id, result.ItemIndex, result.RemainingCount);
    }
}
