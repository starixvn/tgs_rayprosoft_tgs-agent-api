using Prometheus.DotNetRuntime;

namespace TGS.Core.Infrastructures.Prometheus
{
    public static class PrometheusService
    {
        public static void StartCollecting()
        {
            DotNetRuntimeStatsBuilder
               .Customize()
               .WithContentionStats(sampleRate: SampleEvery.FiveEvents)
               .WithJitStats(sampleRate: SampleEvery.TenEvents)
               .WithThreadPoolSchedulingStats(sampleRate: SampleEvery.HundredEvents)
               .WithThreadPoolStats()
               .WithGcStats()
               .WithExceptionStats()
               .StartCollecting();
        }
    }
}