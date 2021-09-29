using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Services.Operator.ValidateCode;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Player
{
    public class CommitPlayerInitPhoneService : ICommandService<CommitPlayerInitPhoneCommand>
    {
        private readonly IRedisCacheManager _cacheManager;
        private readonly IPlayerRepository _playerRepository;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public CommitPlayerInitPhoneService(
            IValidateCodeOperator validateCodeOperator,
            IPlayerRepository playerRepository,
            IRedisCacheManager cacheManager)
        {
            _validateCodeOperator =
                validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        }

        public async ValueTask ExecuteAsync(CommitPlayerInitPhoneCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _validateCodeOperator
                .GetValidateCodeDataAsync<PhoneValidateData>(command.ValidateCode, command.SecurityKey)
                .ConfigureAwait(false);

            if (result == null)
            {
                throw new ValidateCodeNotVerifyException();
            }

            var playerCode = result.PlayerCode;
            var updatedOn = DateTimeOffset.UtcNow;

            await _playerRepository
                .SetPlayerPhoneAsync(result.PlayerCode, result.AreaCode, result.Phone)
                .ConfigureAwait(false);

            await _playerRepository
                .UpdatePlayerStatusAsync(playerCode, PlayerStatus.Enabled)
                .ConfigureAwait(false);

            await _cacheManager
                .ClearCacheAsync(RedisCacheType.Player, playerCode)
                .ConfigureAwait(false);
        }
    }
}
