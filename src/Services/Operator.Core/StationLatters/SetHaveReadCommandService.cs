using System;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.StationLatters
{
    internal class SetHaveReadCommandService : ICommandService<SetHaveReadCommand>
    {
        private readonly IStationLatterRepository _stationLatterRepository;

        public SetHaveReadCommandService(IStationLatterRepository stationLatterRepository)
        {
            _stationLatterRepository = stationLatterRepository ?? throw new ArgumentNullException(nameof(stationLatterRepository));
        }

        public ValueTask ExecuteAsync(SetHaveReadCommand command)
        {
            return _stationLatterRepository.SetStationLatterHaveReadAsync(command.PlayerCode, command.LatterId);
        }
    }
}
