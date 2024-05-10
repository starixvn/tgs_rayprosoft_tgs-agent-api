using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TGS.Services.Agent.ApplicationServices.Player.PlayerRegistrationConditions;

public interface IRegistrationCondition
{
    ValueTask ExecuteAsync(Guid brandId, JsonDocument input, CancellationToken cancellationToken);
}