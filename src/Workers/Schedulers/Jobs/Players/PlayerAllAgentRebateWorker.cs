using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Models.Messages;
using TGS.Workers.Schedulers.Hangfire.Services;

namespace TGS.Workers.Schedulers.Jobs.Players;

[Queue(Consts.Queue.Report)]
public sealed class PlayerAllAgentRebateWorker : BaseWork<TgsResponse>
{
    private readonly IPlayerAllAgentRebateWorkerService _worker;

    public PlayerAllAgentRebateWorker(
        IPlayerAllAgentRebateWorkerService worker)
        => _worker = worker;

    public override Task<TgsResponse> ExecuteAsync(object context) => _worker.ExecutionAsync();
}