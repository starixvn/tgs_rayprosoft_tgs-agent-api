using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.ServiceUtility;
using CoreModels = TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.BackOffice.Application.Commands.BrandCommunication
{
    internal class UpdateBrandCommunicationCommandService : ICommandService<UpdateBrandCommunicationCommand>
    {
        private readonly ITgsQueries<CoreModels.BrandCommunication> _tgsQueries;
        private readonly ITgsCommands<CoreModels.BrandCommunication> _commands;

        public UpdateBrandCommunicationCommandService(
            ITgsQueries<CoreModels.BrandCommunication> tgsQueries,
            ITgsCommands<CoreModels.BrandCommunication> commands)
        {
            _tgsQueries = tgsQueries ?? throw new ArgumentNullException(nameof(tgsQueries));
            _commands = commands;
        }

        public async ValueTask ExecuteAsync(
            UpdateBrandCommunicationCommand command,
            CancellationToken cancellationToken = default)
        {
            var communication = await _tgsQueries.GetSingleAsync(x => x.Id == command.Id);

            communication.Type = command.CommunicationType;
            communication.Phone = command.Phone;
            communication.AppId = command.AppdId;
            communication.QRCodeUrl = command.QRCodeUrl;

            await _commands.UpdateAsync(communication);
        }
    }
}