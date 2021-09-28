using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Services.Operations;
using TGS.Services.Operator.ValidateCode;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Player
{
    internal class CommitSetPlayerPhoneCommandService : ICommandService<CommitSetPlayerPhoneCommand>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IRedisCacheManager _cacheManager;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public CommitSetPlayerPhoneCommandService(
            IValidateCodeOperator validateCodeOperator,
            IPlayerRepository playerRepository,
            IRedisCacheManager cacheManager)
        {
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        }

        public async ValueTask ExecuteAsync(CommitSetPlayerPhoneCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _validateCodeOperator
                .GetValidateCodeDataAsync<PhoneValidateData>(command.ValidateCode, command.SecurityKey)
                .ConfigureAwait(false);

            if (result == null)
                throw new ValidateCodeNotVerifyException();

            var updatedOn = DateTimeOffset.UtcNow;

            await _playerRepository.SetPlayerPhoneAsync(
                command.PlayerCode,
                result.AreaCode,
                result.Phone).ConfigureAwait(false);
            await _cacheManager
                .ClearCacheAsync(RedisCacheType.Player, command.PlayerCode)
                .ConfigureAwait(false);
        }
    }
}