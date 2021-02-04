using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NATS.Client;
using TGS.Core.Infrastructures.Nats;

namespace TGS.Core.Infrastructures.HealthChecks
{
    public sealed class NatsHealthCheck : IHealthCheck
    {
        private readonly INatsManager _natsManager;

        public NatsHealthCheck(INatsManager natsManager)
        {
            _natsManager = natsManager;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            try
            {
                var connState = _natsManager.GetConnectStatus();
                var healthStatus = connState switch
                {
                    ConnState.CONNECTED     => HealthStatus.Healthy,
                    ConnState.DISCONNECTED  => HealthStatus.Unhealthy,
                    ConnState.CLOSED        => HealthStatus.Unhealthy,
                    ConnState.RECONNECTING  => HealthStatus.Degraded,
                    ConnState.CONNECTING    => HealthStatus.Degraded,
                    ConnState.DRAINING_SUBS => HealthStatus.Healthy,
                    ConnState.DRAINING_PUBS => HealthStatus.Healthy,
                    _                       => HealthStatus.Unhealthy
                };

                var result = new HealthCheckResult(healthStatus, $"Nats connection state is '{connState}'");

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(exception: ex));
            }
        }
    }
}
