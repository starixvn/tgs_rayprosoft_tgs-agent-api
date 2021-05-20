using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Promotion.Services
{
    internal class TagPlayerDepositCommandService : ICommandService<TagPlayerDepositCommand>
    {
        private readonly IPlayerDataRepository _playerDataRepository;

        public TagPlayerDepositCommandService(IPlayerDataRepository playerDataRepository)
        {
            _playerDataRepository = playerDataRepository ?? throw new ArgumentNullException(nameof(playerDataRepository));
        }

        public ValueTask ExecuteAsync(TagPlayerDepositCommand command, CancellationToken cancellationToken = default)
            => _playerDataRepository.SetPlayerDepositAsync(command.PlayerCode);
    }
}
