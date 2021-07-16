using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.ServiceUtility;
using CoreModels = TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.BackOffice.Application.Commands.BrandCommunication
{
    internal class DeleteBrandCommunicationCommandService : ICommandService<DeleteBrandCommunicationCommand>
    {
        private readonly ITgsQueries<CoreModels.BrandCommunication> _tgsQueries;
        private readonly ITgsCommands<CoreModels.BrandCommunication> _commands;

        public DeleteBrandCommunicationCommandService(
            ITgsQueries<CoreModels.BrandCommunication> tgsQueries,
            ITgsCommands<CoreModels.BrandCommunication> commands)
        {
            _tgsQueries = tgsQueries ?? throw new ArgumentNullException(nameof(tgsQueries));
            _commands = commands;
        }

        public async ValueTask ExecuteAsync(
            DeleteBrandCommunicationCommand command,
            CancellationToken cancellationToken = default)
        {
            var communication = await _tgsQueries.GetSingleAsync(x => x.Id == command.Id);
            await _commands.DeleteAsync(communication);
        }
    }
}