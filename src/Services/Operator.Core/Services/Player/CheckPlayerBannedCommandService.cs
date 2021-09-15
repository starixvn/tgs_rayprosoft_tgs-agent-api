using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operations;
using TGS.ServiceUtility;
using CoreEnums = TGS.Core.Models.Enums;

namespace TGS.Services.Operator.Services.Player
{
    internal class CheckPlayerBannedCommandService : ICommandService<CheckPlayerBannedCommand>
    {
        private readonly IPlayerOperations _playerOperations;

        public CheckPlayerBannedCommandService(IPlayerOperations playerOperations)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        }

        public async ValueTask ExecuteAsync(CheckPlayerBannedCommand command, CancellationToken cancellationToken = default)
        {
            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(command.BrandId, command.UserName);
            var player = await _playerOperations.GetPlayerByCodeAsync(playerCode);

            if (player.Status == CoreEnums.PlayerStatus.InBlacklist)
            {
                throw new UserBannedException();
            }
        }
    }
}