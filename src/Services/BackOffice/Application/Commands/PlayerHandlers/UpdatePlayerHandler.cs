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
using TGS.GameProviders.KickPlayerServer.Client;

namespace TGS.Services.BackOffice.Application.Commands.PlayerHandlers
{
    public class UpdatePlayerHandler : Handler<UpdatePlayerRequest, MessageResponse>
    {
        private readonly IAgentService _agentService;
        private readonly IPlayerService _service;
        private readonly IKickPlayerServiceClient _kickPlayerServiceClient;

        public UpdatePlayerHandler(
            IPlayerService service,
            IAgentService agentService,
            IHttpContextAccessor accessor,
            IKickPlayerServiceClient kickPlayerServiceClient) :
            base(accessor)
        {
            _service = service;
            _agentService = agentService;
            _kickPlayerServiceClient = kickPlayerServiceClient ?? throw new System.ArgumentNullException(nameof(kickPlayerServiceClient));
        }

        public override async Task<MessageResponse> Handle(
            UpdatePlayerRequest request,
            CancellationToken cancellationToken)
        {
            var updatePlayerType = new[]
            {
                "info",
                "contact",
                "password"
            };

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

            if (request.Email.IsNotNullOrEmpty() && request.Type == updatePlayerType[1] &&
                await _service.ExistsAsync(x
                    => x.BrandId == player.BrandId && x.PlayerCode != player.PlayerCode &&
                    x.Email.ToLower() == request.Email.ToLower()))
            {
                return CreateInvalidResponse<MessageResponse>(Player.BrandEmailDuplicate);
            }

            if (player.Phone.IsNotNullOrEmpty() && request.Phone.IsNullOrEmpty() && request.Type == updatePlayerType[1])
            {
                return CreateInvalidResponse<MessageResponse>(Player.PhoneRequired);
            }

            await _service.UpdatePlayerAsync(player, request);

            if (request.Type == updatePlayerType[0]
                &&
                player.Status != request.Status
                && (request.Status is PlayerStatus.Disabled or PlayerStatus.InBlacklist))
            {
                await _kickPlayerServiceClient.KickPlayerAsync(request.PlayerCode).ConfigureAwait(false);
            }

            return CreateValidResponse<MessageResponse>();
        }
    }
}