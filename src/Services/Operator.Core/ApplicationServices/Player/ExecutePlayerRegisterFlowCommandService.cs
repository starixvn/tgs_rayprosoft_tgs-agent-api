using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.ApplicationServices.Player;

internal class ExecutePlayerRegisterFlowCommandService : IGetService<ExecutePlayerRegisterFlowCommand, RegisterFlowProcessResult>
{
    private readonly IBrandRegisterFlowDefinitionManager _brandRegisterFlowDefinitionManager;

    public ExecutePlayerRegisterFlowCommandService(IBrandRegisterFlowDefinitionManager brandRegisterFlowDefinitionManager)
    {
        _brandRegisterFlowDefinitionManager = brandRegisterFlowDefinitionManager ?? throw new ArgumentNullException(nameof(brandRegisterFlowDefinitionManager));
    }

    public async ValueTask<RegisterFlowProcessResult> GetAsync(ExecutePlayerRegisterFlowCommand query, CancellationToken cancellationToken = default)
    {
        var flowDefinition = await _brandRegisterFlowDefinitionManager
            .GetRegisterFlowDefinitionAsync(query.BrandId, cancellationToken)
            .ConfigureAwait(false);

        var flow = flowDefinition.BuildRegisterFlow();

        var isCompleted = await flow
            .ConfirmCompletedAsync(query.LocationInfo.PlayerCode, cancellationToken)
            .ConfigureAwait(false);

        if (isCompleted)
            return new RegisterFlowProcessResult(
                true,
                null,
                null);

        var result = await flow.ProcessInputAsync(
            query.BrandId,
            query.Input,
            query.LocationInfo,
            cancellationToken).ConfigureAwait(false);

        return result;
    }
}
