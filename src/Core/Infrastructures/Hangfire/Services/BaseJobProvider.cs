using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Infrastructures.Hangfire.Services
{
    internal interface IJobProvider
    {
        IEnumerable<JobDescription> GetJobDescriptions();

        bool TryGetJobDescriptionById(Guid id, out JobDescription job);
    }

    public abstract class BaseJobProvider : IJobProvider
    {
        private readonly IReadOnlyDictionary<Guid, JobDescription> _jobs;

        protected BaseJobProvider(params JobDescription[] jobs)
        {
            _jobs = jobs.ToDictionary(j => j.Id);
        }

        IEnumerable<JobDescription> IJobProvider.GetJobDescriptions() => 
            _jobs.Values;

        bool IJobProvider.TryGetJobDescriptionById(Guid id, out JobDescription job) =>
            _jobs.TryGetValue(id, out job);
    }
}
