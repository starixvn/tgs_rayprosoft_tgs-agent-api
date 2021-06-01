using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;
using TGS.Services.Operator.Services.StationLetters;
using TGS.Services.Operator.StationLetters;

namespace TGS.Services.Operator.Services.StationLetters
{
    internal class SetHaveReadCommandService : ICommandService<SetHaveReadCommand>
    {
        private readonly IStationLetterRepository _stationLetterRepository;

        public SetHaveReadCommandService(IStationLetterRepository stationLetterRepository)
        {
            _stationLetterRepository = stationLetterRepository ?? throw new ArgumentNullException(nameof(stationLetterRepository));
        }

        public ValueTask ExecuteAsync(SetHaveReadCommand command, CancellationToken cancellationToken = default)
        {
            return _stationLetterRepository.SetStationLetterHaveReadAsync(command.PlayerCode, command.LetterId);
        }
    }
}
