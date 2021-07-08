using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Models.Messages;

namespace TGS.Workers.Schedulers.Jobs
{
    [Queue(Consts.Queue.ArrearsEmail)]
    public sealed class PlatformCreditBalanceWorker : BaseWork<TgsResponse>
    {
        private readonly ITgsRepository _repository;

        public PlatformCreditBalanceWorker(
            ITgsRepository repository)
        {
            _repository = repository;
        }

        public override async Task<TgsResponse> ExecuteAsync(object context)
        {
            var platformCredits = await _repository.PlatformCredits.AsQueryable().ToArrayAsync();
            foreach (var platformCredit in platformCredits)
            {
                platformCredit.Balance = platformCredit.Credit;
            }

            _repository.PlatformCredits.UpdateRange(platformCredits);
            await _repository.SaveChangesAsync();

            return new();
        }
    }
}