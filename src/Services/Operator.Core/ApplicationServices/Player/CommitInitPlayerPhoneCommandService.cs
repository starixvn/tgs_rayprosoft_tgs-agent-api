using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tgs.BrandCertificate.Clients;
using Tgs.PlayerService.Client;
using Tgs.ServiceUtility;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Services.Operator.ValidateCode;

namespace TGS.Services.Operator.ApplicationServices.Player;

public class CommitInitPlayerPhoneCommandService : ICommandService<CommitInitPlayerPhoneCommand>
{
    private readonly IBrandServiceClient _brandServiceClient;
    private readonly IRedisCacheManager _cacheManager;
    private readonly IPlayerServiceClient _playerServiceClient;
    private readonly IValidateCodeOperator _validateCodeOperator;

    public CommitInitPlayerPhoneCommandService(
        IValidateCodeOperator validateCodeOperator,
        IBrandServiceClient brandServiceClient,
        IRedisCacheManager cacheManager,
        IPlayerServiceClient playerServiceClient)
    {
        _validateCodeOperator =
            validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
        _brandServiceClient = brandServiceClient ?? throw new ArgumentNullException(nameof(brandServiceClient));
        _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        _playerServiceClient = playerServiceClient ?? throw new ArgumentNullException(nameof(playerServiceClient));
    }

    public async ValueTask ExecuteAsync(CommitInitPlayerPhoneCommand command,
        CancellationToken cancellationToken = default)
    {
        var brand = await _brandServiceClient
            .GetBrandInfoAsync(command.BrandId)
            .ConfigureAwait(false);

        if (brand!.SmsRegisterFlowEnabled is false)
            return;

        var result = await _validateCodeOperator
            .GetValidateCodeDataAsync<PhoneValidateData>(command.ValidateCode, command.SecurityKey)
            .ConfigureAwait(false);

        if (result == null)
            throw new ValidateCodeNotVerifyException();

        var playerCode = result.PlayerCode;

        var findPlayer = await _playerServiceClient.GetPlayerInfoByPhoneAsync(
            command.BrandId,
            result.AreaCode,
            result.Phone,
            cancellationToken).ConfigureAwait(false);

        var isBrandHasSamePhone = findPlayer is not null && findPlayer.PlayerCode != playerCode;

        if (isBrandHasSamePhone)
            throw new BrandPhoneDuplicateException();

        await _playerServiceClient.SetPlayerPropertiesAsync(
            result.PlayerCode,
            new Dictionary<PlayerPropertyName, string?>
            {
                [PlayerPropertyName.AreaCode] = result.AreaCode,
                [PlayerPropertyName.Phone] = result.Phone,
                [PlayerPropertyName.PhoneAuthentication] = true.ToString()
            }.ToImmutableDictionary(),
            cancellationToken)
            .ConfigureAwait(false);

        if (brand.EmailRegisterFlowEnabled)
            return;

        await _cacheManager
            .ClearCacheAsync(RedisCacheType.Player, playerCode)
            .ConfigureAwait(false);
    }
}
