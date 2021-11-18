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
using Tgs.GameProviders.KickPlayerServer.Client;
using Microsoft.Extensions.Logging;

namespace TGS.Services.BackOffice.Application.Commands.PlayerHandlers
{
    public class UpdatePlayerHandler : Handler<UpdatePlayerRequest, MessageResponse>
    {
        private readonly ILogger<UpdatePlayerHandler> _logger;
        private readonly IAgentService _agentService;
        private readonly IPlayerService _service;
        private readonly IKickPlayerServiceClient _kickPlayerServiceClient;

        public UpdatePlayerHandler(
            ILogger<UpdatePlayerHandler> logger,
            IPlayerService service,
            IAgentService agentService,
            IHttpContextAccessor accessor,
            IKickPlayerServiceClient kickPlayerServiceClient) :
            base(accessor)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
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

            var player = await _service.GetPlayerAsync(p => p.PlayerCode == request.PlayerCode).ConfigureAwait(false);

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

            var kickPlayer = false;
            if (request.Type == updatePlayerType[0] &&
                player.Status != request.Status)
            {
                kickPlayer = true;
            }

            await _service.UpdatePlayerAsync(player, request).ConfigureAwait(false);

            if (kickPlayer)
            {
                if (request.Status is PlayerStatus.Disabled or PlayerStatus.InBlacklist)
                {
                    _logger.LogInformation("call KickPlayerAsync");
                    await _kickPlayerServiceClient.KickPlayerAsync(request.PlayerCode).ConfigureAwait(false);
                }
            }

            return CreateValidResponse<MessageResponse>();
        }
    }
}