using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.ServiceUtility;
using CoreModels = TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.BackOffice.Application.Commands.BrandLiveChat
{
    internal class DeleteBrandLiveChatCommandService : ICommandService<DeleteBrandLiveChatCommand>
    {
        private readonly ITgsQueries<CoreModels.BrandLiveChat> _tgsQueries;
        private readonly ITgsCommands<CoreModels.BrandLiveChat> _commands;

        public DeleteBrandLiveChatCommandService(
            ITgsQueries<CoreModels.BrandLiveChat> tgsQueries,
            ITgsCommands<CoreModels.BrandLiveChat> commands)
        {
            _tgsQueries = tgsQueries ?? throw new System.ArgumentNullException(nameof(tgsQueries));
            _commands = commands;
        }

        public async ValueTask ExecuteAsync(DeleteBrandLiveChatCommand command, CancellationToken cancellationToken = default)
        {
            var communication = await _tgsQueries.GetSingleAsync(x => x.BrandId == command.BrandId);
            await _commands.DeleteAsync(communication);
        }
    }
}