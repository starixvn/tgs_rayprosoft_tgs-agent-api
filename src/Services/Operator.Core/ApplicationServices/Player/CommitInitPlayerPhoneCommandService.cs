using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Services.Operator.ValidateCode;

namespace TGS.Services.Operator.ApplicationServices.Player;

public class CommitInitPlayerPhoneCommandService : ICommandService<CommitInitPlayerPhoneCommand>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IRedisCacheManager _cacheManager;
    private readonly IGetService<CheckBrandPhoneQuery, bool> _checkService;
    private readonly IPlayerRepository _playerRepository;
    private readonly IValidateCodeOperator _validateCodeOperator;

    public CommitInitPlayerPhoneCommandService(
        IValidateCodeOperator validateCodeOperator,
        IBrandRepository brandRepository,
        IPlayerRepository playerRepository,
        IRedisCacheManager cacheManager,
        IGetService<CheckBrandPhoneQuery, bool> checkService)
    {
        _validateCodeOperator =
            validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        _checkService = checkService ?? throw new ArgumentNullException(nameof(checkService));
    }

    public async ValueTask ExecuteAsync(CommitInitPlayerPhoneCommand command,
        CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository
            .GetBrandInfoAsync(command.BrandId)
            .ConfigureAwait(false);

        if (brand!.SmsRegisterFlowEnabled is false)
            return;

        var result = await _validateCodeOperator
            .GetValidateCodeDataAsync<PhoneValidateData>(command.ValidateCode, command.SecurityKey)
            .ConfigureAwait(false);

        if (result == null)
        {
            throw new ValidateCodeNotVerifyException();
        }

        var playerCode = result.PlayerCode;

        var checkQuery = new CheckBrandPhoneQuery(
            command.BrandId,
            playerCode,
            result.AreaCode,
            result.Phone);

        var isBrandHasSamePhone = await _checkService
            .GetAsync(checkQuery, cancellationToken)
            .ConfigureAwait(false);

        if (isBrandHasSamePhone)
            throw new BrandPhoneDuplicateException();

        await _playerRepository
            .SetPlayerPhoneAsync(result.PlayerCode, result.AreaCode, result.Phone)
            .ConfigureAwait(false);

        if (brand.EmailRegisterFlowEnabled)
            return;

        await _playerRepository
            .UpdatePlayerStatusAsync(playerCode, PlayerStatus.Enabled)
            .ConfigureAwait(false);

        await _playerRepository
            .PlayerRegisterIpAsync(
                result.PlayerCode,
                command.IpAddress,
                command.HostName,
                DateTimeOffset.UtcNow)
            .ConfigureAwait(false);

        await _playerRepository
            .PlayerLoginIpAsync(result.PlayerCode, command.IpAddress, command.HostName)
            .ConfigureAwait(false);

        await _cacheManager
            .ClearCacheAsync(RedisCacheType.Player, playerCode)
            .ConfigureAwait(false);
    }
}
