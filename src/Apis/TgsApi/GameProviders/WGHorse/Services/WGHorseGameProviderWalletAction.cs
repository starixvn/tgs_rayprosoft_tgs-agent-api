using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.GameProviderWalletAction;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Services.Caches;
using TGS.GameProviders.Repositories;
using TGS.Services.Operations;
using TGS.Services.WalletService;
using TGS.Shared.Game.ApiClients.WGHorse;
using TGS.Shared.Game.Models.WGHorse.Enums;
using TGS.Shared.Game.Models.WGHorse.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.WGHorse.Services
{
    public class WGHorseGameProviderWalletAction : BaseGameProviderWalletAction
    {
        private readonly IWGHorseApiClient _client;
        private readonly IWGHorseService _service;
        public WGHorseGameProviderWalletAction(
             IPlayerOperations playerOperations,
             IGameProviderOperations gameProviderOperations,
             IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository,
             IWalletRepository walletRepository,
             IWGHorseApiClient client,
             IWGHorseService service) : base(playerOperations, gameProviderOperations, gameProviderFundTransferTransactionRepository, walletRepository)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public override ValueTask<bool> CheckingAmountAsync(CachedPlayer player, decimal amount)
            => ValueTask.FromResult(true);

        public override async ValueTask<(bool Checking, string OrderId)> CheckingOrderIdAsync(CachedPlayer player, string orderId)
        {
            var configuration = await _service.GetConfigurationAsync().ConfigureAwait(false);

            var source = new WGHorseAccessMoneyQueryRequest()
            {
                OrderId = orderId,
            };
            var response = await _client.AccessMoneyQueryAsync(configuration, source).ConfigureAwait(false);
            if (response.ReturnCode.IgnoreCaseNotEquals("1"))
            {
                return (false, null);
            }

            return (true, orderId);
        }

        public override async ValueTask<(bool Success, string OrderId, decimal Balance)> FundInAsync(CachedPlayer player, decimal amount)
        {
            var date = DateTimeOffset.UtcNow.AddHours(+8);
            var configuration = await _service.GetConfigurationAsync().ConfigureAwait(false);
            var orderId = $"{ Consts.GameProvider.WGHorse}{date.ToString("yyyyMMddHHmmss")}{player.PlayerCode}";
            var source = new WGHorseAccessMoneyRequest()
            {
                PlayerCode = player.PlayerCode,
                AmountType = AmountTypes.FundIn,
                OrderId = orderId,
                Amount = amount.ToString()

            };
            var response = await _client.AccessMoneyAsync(configuration, source).ConfigureAwait(false);
            if (response.ReturnCode.ToString().IgnoreCaseNotEquals("1"))
            {
                return (false, orderId, 0);
            }
            var (Success, Balance) = await GetGameProviderBalanceAsync(player).ConfigureAwait(false);

            return (true, orderId, Balance);
        }

        public override async ValueTask<(bool Success, string OrderId, decimal Balance)> FundOutAsync(CachedPlayer player, decimal amount)
        {
            var date = DateTimeOffset.UtcNow.AddHours(+8);
            var configuration = await _service.GetConfigurationAsync().ConfigureAwait(false);
            var orderId = $"{ Consts.GameProvider.WGHorse}{date.ToString("yyyyMMddHHmmss")}{player.PlayerCode}";
            var source = new WGHorseAccessMoneyRequest()
            {
                PlayerCode = player.PlayerCode,
                AmountType = AmountTypes.FundOut,
                OrderId = orderId,
                Amount = amount.ToString()

            };
            var response = await _client.AccessMoneyAsync(configuration, source).ConfigureAwait(false);
            if (response.ReturnCode.ToString().IgnoreCaseNotEquals("1"))
            {
                return (false, orderId, 0);
            }
            var (Success, Balance) = await GetGameProviderBalanceAsync(player).ConfigureAwait(false);

            return (true, orderId, Balance);
        }

        public override async ValueTask<(Guid GameProviderId, string GameProviderCode)> GetGameProviderAsync()
        {
            return (await _service.GetGameProviderIdAsync().ConfigureAwait(false), Consts.GameProvider.WGHorse);
        }

        public override async ValueTask<(bool Success, decimal Balance)> GetGameProviderBalanceAsync(CachedPlayer player)
        {
            var configuration = await _service.GetConfigurationAsync().ConfigureAwait(false);
            var source = new WGHorseGetBalanceRequest()
            {
                PlayerCode = player.PlayerCode
            };
            var response = await _client.GetBalanceAsync(configuration, source).ConfigureAwait(false);

            var gameProviderId = await _service.GetGameProviderIdAsync();
            var currencyCode = await _service.GetGameProviderCurrencyCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode);
            if (response.ReturnCode.ToString().IgnoreCaseEquals("602"))
            {
                var prepareLoginRequest = new WGHorsePrepareLoginRequest()
                {
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    PlayerCode = player.PlayerCode,
                    PlayerNickname = player.PlayerCode,
                    CurrencyCode = currencyCode,
                };
                var prepareLoginResponse = await _client.GetPrepareLoginAsync(configuration, prepareLoginRequest).ConfigureAwait(false);
                if (prepareLoginResponse.ReturnCode.ToString().IgnoreCaseEquals("1"))
                {
                    response = await _client.GetBalanceAsync(configuration, source).ConfigureAwait(false);
                }
            }

            return response.ReturnCode.IgnoreCaseNotEquals("1") ? (true, decimal.Parse(response.Balance)) : ((bool Success, decimal Balance))(false, 0);
        }
    }
}
