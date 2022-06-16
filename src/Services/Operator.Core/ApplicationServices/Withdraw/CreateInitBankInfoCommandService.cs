using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using Tgs.Payment.Clients;
using Tgs.ServiceUtility;
using Tgs.BrandCertificate.Clients;

namespace TGS.Services.Operator.ApplicationServices.Withdraw;

internal class CreateInitBankInfoCommandService : ICommandService<CreateInitBankInfoCommand>
{
    private readonly IBrandServiceClient _brandServiceClient;
    private readonly IRedisCacheManager _cacheManager;
    private readonly IPaymentServiceClient _paymentServiceClient;
    private readonly IPlayerRepository _playerRepository;

    public CreateInitBankInfoCommandService(
        IBrandServiceClient brandServiceClient,
        IPlayerRepository playerRepository,
        IPaymentServiceClient paymentServiceClient,
        IRedisCacheManager cacheManager)
    {
        _brandServiceClient = brandServiceClient ?? throw new ArgumentNullException(nameof(brandServiceClient));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
    }

    public async ValueTask ExecuteAsync(
        CreateInitBankInfoCommand command,
        CancellationToken cancellationToken = default)
    {
        var brand = await _brandServiceClient
            .GetBrandInfoAsync(command.BrandId)
            .ConfigureAwait(false);

        if (brand!.BankInfoRegisterFlowEnabled is false)
            return;

        var playerCode = command.PlayerCode;

        var bankInfo = new BankCreateInfo
        {
            BankType = command.BankType,
            PaymentChannelId = command.PaymentChannelId,
            Detail = command.Detail
        };

        var bankInfoId = await _paymentServiceClient
            .CreatePlayerBankInfoAsync(
                command.BrandId,
                playerCode,
                bankInfo,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await _paymentServiceClient
            .SetDefaultPlayerBankInfoAsync(
                playerCode,
                bankInfoId,
                playerCode,
                cancellationToken)
            .ConfigureAwait(false);

        await _playerRepository
            .UpdatePlayerStatusAsync(playerCode, PlayerStatus.Enabled)
            .ConfigureAwait(false);

        await _playerRepository
            .PlayerRegisterIpAsync(playerCode, command.IpAddress, command.HostName, DateTimeOffset.UtcNow)
            .ConfigureAwait(false);

        await _playerRepository
            .PlayerLoginIpAsync(playerCode, command.IpAddress, command.HostName)
            .ConfigureAwait(false);

        await _cacheManager
            .ClearCacheAsync(RedisCacheType.Player, playerCode)
            .ConfigureAwait(false);
    }
}
