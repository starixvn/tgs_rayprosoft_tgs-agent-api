using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.Operations.Queries
{
    public interface IAgentQueries
    {
        Task<Agent> GetAgentAsync(string username);
    }

    public class AgentQueries : IAgentQueries
    {
        private readonly ITgsRepository _repository;

        public AgentQueries(ITgsRepository repository)
        {
            _repository = repository;
        }

        Task<Agent> IAgentQueries.GetAgentAsync(string invitationCode) =>
            _repository.Agents.FirstOrDefaultAsync(x => x.InvitationCode == invitationCode);
    }
}