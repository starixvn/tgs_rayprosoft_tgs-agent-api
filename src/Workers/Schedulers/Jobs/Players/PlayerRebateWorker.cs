using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Models.Messages;
using TGS.Workers.Schedulers.Hangfire.Services;

namespace TGS.Workers.Schedulers.Jobs.Players;

[Queue(Consts.Queue.Report)]
public sealed class PlayerRebateWorker : BaseWork<TgsResponse>
{
    private readonly IPlayerRebateWorkerService _worker;

    public PlayerRebateWorker(
        IPlayerRebateWorkerService worker)
    {
        _worker = worker;
    }

    public override Task<TgsResponse> ExecuteAsync(object context) => _worker.ExecutionAsync();
}