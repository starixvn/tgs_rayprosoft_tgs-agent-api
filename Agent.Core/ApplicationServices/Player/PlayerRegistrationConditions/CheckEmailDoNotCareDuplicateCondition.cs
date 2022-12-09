using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TGS.Services.Agent.ApplicationServices.Player.PlayerRegistrationConditions
{
    internal class CheckEmailDoNotCareDuplicateCondition : IRegistrationCondition
    {
        public ValueTask ExecuteAsync(Guid brandId, JsonDocument input,
            CancellationToken cancellationToken) => ValueTask.CompletedTask;
    }
}
