using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Services.Operator.ValidateCode;

namespace TGS.Services.Operator.ApplicationServices.Player;

internal class CommitInitPlayerEmailCommandService : ICommandService<CommitInitPlayerEmailCommand>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IRedisCacheManager _cacheManager;
    private readonly IGetService<CheckBrandEmailQuery, bool> _checkService;
    private readonly IPlayerRepository _playerRepository;
    private readonly IValidateCodeOperator _validateCodeOperator;

    public CommitInitPlayerEmailCommandService(
       IBrandRepository brandRepository,
       IPlayerRepository playerRepository,
       IGetService<CheckBrandEmailQuery, bool> checkService,
       IValidateCodeOperator validateCodeOperator,
       IRedisCacheManager cacheManager)
    {
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _checkService = checkService ?? throw new ArgumentNullException(nameof(checkService));
        _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
        _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
    }

    public async ValueTask ExecuteAsync(
        CommitInitPlayerEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository
            .GetBrandInfoAsync(command.BrandId)
            .ConfigureAwait(false);

        if (brand!.EmailRegisterFlowEnabled is false)
            return;

        var result = await _validateCodeOperator
            .GetValidateCodeDataAsync<EmailValidateData>(
                command.ValidateCode,
                command.SecurityKey)
            .ConfigureAwait(false);

        if (result == null)
            throw new ValidateCodeNotVerifyException();

        var playerCode = result.PlayerCode;

        var checkQuery = new CheckBrandEmailQuery(command.BrandId, result.Email, playerCode);

        var isBrandHasSameEmail = await _checkService
            .GetAsync(checkQuery, cancellationToken)
            .ConfigureAwait(false);

        if (isBrandHasSameEmail)
            throw new BrandEmailDuplicateException();

        await _playerRepository
            .SetPlayerEmailAsync(playerCode, result.Email)
            .ConfigureAwait(false);

        if (brand.BankInfoRegisterFlowEnabled)
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
