using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TGS.Services.Operator;

public interface IRegisterFlowStep
{
    string Type { get; }

    ValueTask<bool> ConfirmCompletedAsync(string? playerCode, CancellationToken cancellationToken = default);

    ValueTask<RegisterFlowProcessResult> ProcessInputAsync(
        Guid brandId,
        JsonDocument input,
        PlayerLocationInfo locationInfo,
        CancellationToken cancellationToken = default);
}
