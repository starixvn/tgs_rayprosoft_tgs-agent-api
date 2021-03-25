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
    public class CreateAgentHandler : Handler<CreateAgentRequest, MessageResponse>
    {
        private readonly IAgentService _service;

        public CreateAgentHandler(IAgentService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(CreateAgentRequest request,
            CancellationToken cancellationToken)
        {
            if (await _service.ExistsAsync(x => x.Username == request.Username))
            {
                return CreateInvalidResponse<MessageResponse>(Agent.NameExisted);
            }

            if (request.ParentUsername.IsNotNullOrEmpty() &&
                !await _service.ExistsAsync(x => x.Username == request.ParentUsername))
            {
                return CreateInvalidResponse<MessageResponse>(Agent.ParentAgentNotExists);
            }

            await _service.CreateAgentAsync(request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}