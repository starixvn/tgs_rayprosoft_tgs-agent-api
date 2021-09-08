using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Commands.PlayerHandlers
{
    public class UpdatePlayerHandler : Handler<UpdatePlayerRequest, MessageResponse>
    {
        private readonly IAgentService _agentService;
        private readonly IGetService<CheckBrandEmailQuery, bool> _getService;
        private readonly IPlayerService _service;

        public UpdatePlayerHandler(
            IPlayerService service,
            IAgentService agentService,
            IHttpContextAccessor accessor,
            IGetService<CheckBrandEmailQuery, bool> getService) :
            base(accessor)
        {
            _service = service;
            _agentService = agentService;
            _getService = getService;
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

            if (request.Type == updatePlayerType[1])
            {
                var isBrandHasSameEmail = await _getService
                    .GetAsync(new CheckBrandEmailQuery(player.BrandId, request.Email))
                    .ConfigureAwait(false);
                if (isBrandHasSameEmail)
                    return CreateInvalidResponse<MessageResponse>(Player.BrandEmailDuplicate);
            }

            await _service.UpdatePlayerAsync(player, request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}
