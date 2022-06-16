using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operator;

namespace TGS.Tests.OperatorApi.UnitTests;

public class StubRegisterFlowStep : IRegisterFlowStep
{
    private Func<bool> _confirmCompletedDelegate = () => false;

    private Func<Guid, JsonDocument, PlayerLocationInfo, Task<RegisterFlowProcessResult>> _ProcessInputDelegate =
        (_, _, _) => Task.FromResult(new RegisterFlowProcessResult(false, null, null));

    public string Type { get; set; }

    public ValueTask<bool> ConfirmCompletedAsync(CancellationToken cancellationToken = default)
        => new ValueTask<bool>(_confirmCompletedDelegate());

    public async ValueTask<RegisterFlowProcessResult> ProcessInputAsync(
        Guid brandId,
        JsonDocument input,
        PlayerLocationInfo locationInfo,
        CancellationToken cancellationToken = default)
        => await _ProcessInputDelegate(brandId, input, locationInfo).ConfigureAwait(false);

    internal void SetConfirmCompletedDelegate(Func<bool> confirmCompletedDelegate)
        => _confirmCompletedDelegate = confirmCompletedDelegate;

    internal void SetProcessInputDelegate(Func<Guid, JsonDocument, PlayerLocationInfo, Task<RegisterFlowProcessResult>> processInputDelegate)
        => _ProcessInputDelegate = processInputDelegate;
}
