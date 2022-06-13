using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TGS.Services.Operator;

internal class RegisterFlow : IRegisterFlowStep
{
    private readonly Queue<IRegisterFlowStep> _flowSteps = new Queue<IRegisterFlowStep>();

    public string Type => "RegisterFlow";

    public RegisterFlow(IEnumerable<IRegisterFlowStep> steps)
    {
        foreach (var step in steps)
            _flowSteps.Enqueue(step);
    }

    public async ValueTask<bool> ConfirmCompletedAsync(CancellationToken cancellationToken = default)
    {
        if (_flowSteps.Count == 0)
            return true;

        while (_flowSteps.Count > 0)
        {
            var isCompleted = await _flowSteps.Peek().ConfirmCompletedAsync().ConfigureAwait(false);
            if (isCompleted)
                _flowSteps.Dequeue();
            else
                return false;
        }

        return true;
    }

    public async ValueTask<RegisterFlowProcessResult> ProcessInputAsync(JsonDocument input, CancellationToken cancellationToken = default)
    {
        var currentStep = _flowSteps.Peek();

        var result = await currentStep
            .ProcessInputAsync(input)
            .ConfigureAwait(false);

        if (result.IsCompleted)
        {
            _flowSteps.Dequeue();

            var isCompleted = await ConfirmCompletedAsync().ConfigureAwait(false);

            return new RegisterFlowProcessResult(
                isCompleted,
                isCompleted ? null : _flowSteps.Peek().Type,
                result.StepResponse);
        }
        else
            return new RegisterFlowProcessResult(
                false,
                currentStep.Type,
                result.StepResponse);
    }
}
