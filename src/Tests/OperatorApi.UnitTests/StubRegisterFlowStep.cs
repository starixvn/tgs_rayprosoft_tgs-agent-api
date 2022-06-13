using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operator;

namespace TGS.Tests.OperatorApi.UnitTests;

public class StubRegisterFlowStep : IRegisterFlowStep
{
    private Func<bool> _confirmCompletedDelegate = () => false;

    private Func<JsonDocument, Task<RegisterFlowProcessResult>> _ProcessInputDelegate =
        _ => Task.FromResult(new RegisterFlowProcessResult(false, null, null));

    public string Type { get; set; }

    public ValueTask<bool> ConfirmCompletedAsync(CancellationToken cancellationToken = default)
        => new ValueTask<bool>(_confirmCompletedDelegate());

    public async ValueTask<RegisterFlowProcessResult> ProcessInputAsync(JsonDocument input, CancellationToken cancellationToken = default)
        => await _ProcessInputDelegate(input);

    internal void SetConfirmCompletedDelegate(Func<bool> confirmCompletedDelegate)
        => _confirmCompletedDelegate = confirmCompletedDelegate;

    internal void SetProcessInputDelegate(Func<JsonDocument, Task<RegisterFlowProcessResult>> processInputDelegate)
        => _ProcessInputDelegate = processInputDelegate;
}
