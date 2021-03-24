using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Agent;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.AgentHandlers
{
    public class UpdateAgentHandler : Handler<UpdateAgentRequest, MessageResponse>
    {
        private readonly IAgentService _service;

        public UpdateAgentHandler(IAgentService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(UpdateAgentRequest request,
            CancellationToken cancellationToken)
        {
            var agent = await _service.GetAgentAsync(q => q.Id == request.Id);

            if (request.ParentUsername.IsNotNullOrEmpty() &&
                !await _service.ExistsAsync(x => x.Username == request.ParentUsername))
            {
                return CreateInvalidResponse<MessageResponse>(Agent.ParentAgentNotExists);
            }

            await _service.UpdateAgentAsync(agent, request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}