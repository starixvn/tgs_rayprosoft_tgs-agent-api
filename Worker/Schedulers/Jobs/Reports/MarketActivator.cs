using Hangfire;
using TGS.Core.DataServices.Tgs;

namespace TGS.Worker.Schedulers.Jobs.Reports
{
    public class MarketActivator : JobActivator
    {
        private readonly ITgsRepository _repository;

        public MarketActivator(ITgsRepository repository)
        {
            _repository = repository;
        }
    }
}