using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs.Agents
{
    [Queue(Consts.Queue.Report)]
    public sealed class AgentDividendWorker : BaseWork<TgsResponse>
    {
        private readonly IAgentDividendWorkerService _worker;

        public AgentDividendWorker(
            IAgentDividendWorkerService worker)
        {
            _worker = worker;
        }

        public override Task<TgsResponse> ExecuteAsync(object context) => _worker.ExecutionAsync();
    }
}