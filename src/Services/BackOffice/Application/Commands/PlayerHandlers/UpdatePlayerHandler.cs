using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.PlayerHandlers
{
    public class UpdatePlayerHandler : Handler<UpdatePlayerRequest, MessageResponse>
    {
        private readonly IAgentService _agentService;
        private readonly IPlayerService _service;

        public UpdatePlayerHandler(IPlayerService service, IAgentService agentService, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
            _agentService = agentService;
        }

        public override async Task<MessageResponse> Handle(UpdatePlayerRequest request,
            CancellationToken cancellationToken)
        {
            var updatePlayerType = new[] { "info", "contact", "password" };

            if (!updatePlayerType.Contains(request.Type))
            {
                return CreateInvalidResponse<MessageResponse>(Player.UpdateTypeNotFound);
            }

            var player = await _service.GetPlayerAsync(p => p.PlayerCode == request.PlayerCode);

            if (player == null)
            {
                return CreateInvalidResponse<MessageResponse>(Player.NotFound);
            }

            if (!request.AgentUsername.IsNullOrEmpty())
            {
                var agent = await _agentService
                    .GetAgentAsync(p => p.BrandId == player.BrandId
                        && p.Status == AgentStatus.Enabled
                        && p.Username == request.AgentUsername)
                    .ConfigureAwait(false);

                if (agent == null)
                {
                    return CreateInvalidResponse<MessageResponse>(Agent.NotFound);
                }

                player.AgentId = agent.Id;
            }


            await _service.UpdatePlayerAsync(player, request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}