using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Game;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.GameHandlers
{
    public class UpdateGameHandler : Handler<UpdateGameRequest, MessageResponse>
    {
        private readonly IGameService _gameService;

        public UpdateGameHandler(IGameService gameService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _gameService = gameService;
        }

        public override async Task<MessageResponse> Handle(UpdateGameRequest request,
            CancellationToken cancellationToken)
        {
            var game = await _gameService.GetGameAsync(x => x.Id == request.Id);

            if (game is null)
            {
                return CreateInvalidResponse<CreateGamesResponse>(GameProvider.GameRequired);
            }

            await _gameService.UpdateGameAsync(game);

            return CreateValidResponse<CreateGamesResponse>();
        }
    }
}