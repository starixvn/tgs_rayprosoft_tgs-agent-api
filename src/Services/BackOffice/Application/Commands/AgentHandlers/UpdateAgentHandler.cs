using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Agent;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Domain.Enums;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.AgentHandlers
{
    public class UpdateAgentHandler : Handler<UpdateAgentRequest, MessageResponse>
    {
        private readonly IAgentService _service;

        public UpdateAgentHandler(
            IAgentService service,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(
            UpdateAgentRequest request,
            CancellationToken cancellationToken)
        {
            var agent = await _service.GetAgentAsync(q => q.Id == request.Id);

            if (agent == null)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.NotFound);
            }

            if (request.AgentDividendConditions.Length > 1)
            {
                return CreateInvalidResponse<MessageResponse>("Can only have one rank.");
            }

            if (request.ParentUsername.IsNotNullOrEmpty())
            {
                var parentAgent = await _service.GetAgentAsync(x => x.BrandId == request.BrandId &&
                    x.Username == request.ParentUsername);

                if (request.ParentUsername == agent.Username || parentAgent == null)
                {
                    return CreateInvalidResponse<MessageResponse>(Agent.ParentAgentNotExists);
                }

                foreach (var condition in request.AgentDividendConditions.OrderBy(x => x.Rank))
                {
                    var parent = parentAgent.AgentDividendConditions.FirstOrDefault(x => x.Rank == condition.Rank);

                    if (parent is null)
                    {
                        return CreateInvalidResponse<MessageResponse>(string.Format(Agent.ParentRankNotFound,
                            condition.Rank));
                    }

                    if (condition.DividendPercentage > parent.DividendPercentage)
                    {
                        return CreateInvalidResponse<MessageResponse>(string.Format(
                            Agent.DividendPercentageMustLessThan,
                            condition.Rank,
                            parent.DividendPercentage));
                    }
                }
            }

            var childCondition = await _service.GetAgentQueries().Where(x => x.ParentAgentId == agent.Id)
                .SelectMany(x => x.AgentDividendConditions).GroupBy(x => x.Rank,
                    (groupItem, aggregates) => new
                    {
                        Rank = groupItem,
                        MinNetProfit = aggregates.Min(a => a.NetProfit),
                        MinActiveNumber = aggregates.Min(a => a.ActiveNumber),
                        MaxDividendPercentage = aggregates.Max(a => a.DividendPercentage)
                    }).ToArrayAsync(cancellationToken).ConfigureAwait(false);

            if (childCondition.Length > request.AgentDividendConditions.Length)
            {
                return CreateInvalidResponse<MessageResponse>(string.Format(Agent.RankCountInvaild,
                    childCondition.Length));
            }

            foreach (var condition in request.AgentDividendConditions.OrderBy(x => x.Rank))
            {
                var child = childCondition.FirstOrDefault(x => x.Rank == condition.Rank);

                if (child is not null)
                {
                    if (condition.DividendPercentage < child.MaxDividendPercentage)
                    {
                        return CreateInvalidResponse<MessageResponse>(string.Format(
                            Agent.DividendPercentageMustGreaterThan,
                            condition.Rank,
                            child.MaxDividendPercentage));
                    }
                }
            }

            if (request.UpdateType == MemberUpdateType.Contact &&
                request.Email.IsNotNullOrEmpty() &&
                await _service.ExistsAsync(x => x.Id != agent.Id && EF.Functions.ILike(x.Email, $"%{request.Email}%")))
            {
                return CreateInvalidResponse<MessageResponse>(Agent.EmailExisted);
            }

            await _service.UpdateAgentAsync(agent, request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}