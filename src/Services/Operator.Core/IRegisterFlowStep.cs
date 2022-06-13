using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TGS.Services.Operator;

public interface IRegisterFlowStep
{
    string Type { get; }

    ValueTask<bool> ConfirmCompletedAsync(CancellationToken cancellationToken = default);

    ValueTask<RegisterFlowProcessResult> ProcessInputAsync(JsonDocument input, CancellationToken cancellationToken = default);
}
