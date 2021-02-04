using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs.Reports
{
    [Queue(Consts.Queue.Schedule)]
    public sealed class MarketReportWorker : BaseWork<TgsResponse>
    {
        protected override Task<TgsResponse> ExecuteAsync(object context) => 
            Task.FromResult(new TgsResponse());
    }
}
