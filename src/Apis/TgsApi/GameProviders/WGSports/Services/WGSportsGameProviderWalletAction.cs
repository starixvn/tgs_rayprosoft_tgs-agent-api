using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.GameProviderWalletAction;
using TGS.Core.Models;
using TGS.Core.Models.Services.Caches;
using TGS.GameProviders.Repositories;
using TGS.Services.Operations;
using TGS.Services.WalletService;
using TGS.Shared.Game.ApiClients.WGSports;
using TGS.Shared.Game.Models.WGSports.Enums;
using TGS.Shared.Game.Models.WGSports.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.WGSports.Services
{
    public class WGSportsGameProviderWalletAction : BaseGameProviderWalletAction
    {
        private readonly IWGSportsApiClient _client;
        private readonly IWGSportsService _service;
        public WGSportsGameProviderWalletAction(
             IPlayerOperations playerOperations,
             IGameProviderOperations gameProviderOperations,
             IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository,
             IWalletRepository walletRepository,
             IWGSportsApiClient client,
             IWGSportsService service) : base(playerOperations, gameProviderOperations, gameProviderFundTransferTransactionRepository, walletRepository)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public override ValueTask<bool> CheckingAmountAsync(CachedPlayer player, decimal amount)
            => ValueTask.FromResult(true);

        public override async ValueTask<(bool Success, string OrderId, decimal? Balance)> FundInAsync(Guid transactionGameProviderId, CachedPlayer player, decimal amount)
        {
            var date = DateTimeOffset.UtcNow.AddHours(+8);
            var configuration = await _service.GetConfigurationAsync().ConfigureAwait(false);
            var orderId = $"{ Consts.GameProvider.WGSports}{date.ToString("yyyyMMddHHmmss")}{player.PlayerCode}";
            var source = new WGSportsAccessMoneyRequest()
            {
                PlayerCode = player.PlayerCode,
                AmountType = AmountTypes.FundIn,
                OrderId = orderId,
                Amount = amount.ToString()

            };
            var response = await _client.AccessMoneyAsync(configuration, source).ConfigureAwait(false);
            if (response.ReturnCode != 1)
            {
                if (response.ReturnCode != 305)
                {
                    (bool Checking, string OrderId) = await CheckingOrderIdAsync(player, orderId).ConfigureAwait(false);
                    if (!Checking)
                    {
                        return (false, orderId, null);
                    }
                }
                else
                {
                    return (false, orderId, null);
                }
            }
            var (Success, Balance) = await GetGameProviderBalanceAsync(player).ConfigureAwait(false);

            return (true, orderId, Balance);
        }

        public override async ValueTask<(bool Success, string OrderId, decimal? Balance)> FundOutAsync(Guid transactionGameProviderId, CachedPlayer player, decimal amount)
        {
            var date = DateTimeOffset.UtcNow.AddHours(+8);
            var configuration = await _service.GetConfigurationAsync().ConfigureAwait(false);
            var orderId = $"{ Consts.GameProvider.WGSports}{date.ToString("yyyyMMddHHmmss")}{player.PlayerCode}";
            var source = new WGSportsAccessMoneyRequest()
            {
                PlayerCode = player.PlayerCode,
                AmountType = AmountTypes.FundOut,
                OrderId = orderId,
                Amount = amount.ToString()

            };
            var response = await _client.AccessMoneyAsync(configuration, source).ConfigureAwait(false);
            if (response.ReturnCode != 1)
            {
                if (response.ReturnCode != 305)
                {
                    (bool Checking, string OrderId) = await CheckingOrderIdAsync(player, orderId).ConfigureAwait(false);
                    if (!Checking)
                    {
                        return (false, orderId, null);
                    }
                }
                else
                {
                    return (false, orderId, null);
                }
            }
            var (Success, Balance) = await GetGameProviderBalanceAsync(player).ConfigureAwait(false);

            return (true, orderId, Balance);
        }

        public override async ValueTask<(Guid GameProviderId, string GameProviderCode)> GetGameProviderAsync()
        {
            return (await _service.GetGameProviderIdAsync().ConfigureAwait(false), Consts.GameProvider.WGSports);
        }

        public override async ValueTask<(bool Success, decimal Balance)> GetGameProviderBalanceAsync(CachedPlayer player)
        {
            var configuration = await _service.GetConfigurationAsync().ConfigureAwait(false);
            var source = new WGSportsGetBalanceRequest()
            {
                PlayerCode = player.PlayerCode
            };
            var response = await _client.GetBalanceAsync(configuration, source).ConfigureAwait(false);
            var gameProviderId = await _service.GetGameProviderIdAsync();
            var currencyCode = await _service.GetGameProviderCurrencyCodeAsync(gameProviderId, player.DefaultWalletCurrencyCode);
            if (response.ReturnCode == 602)
            {
                var prepareLoginRequest = new WGSportsPrepareLoginRequest()
                {
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    PlayerCode = player.PlayerCode,
                    PlayerNickname = player.PlayerCode,
                    CurrencyCode = currencyCode,
                };
                var prepareLoginResponse = await _client.GetPrepareLoginAsync(configuration, prepareLoginRequest).ConfigureAwait(false);
                if (prepareLoginResponse.ReturnCode == 1)
                {
                    response = await _client.GetBalanceAsync(configuration, source).ConfigureAwait(false);
                }
            }
            return response.ReturnCode == 1 ? (true, decimal.Parse(response.Balance)) : ((bool Success, decimal Balance))(false, 0);
        }

        private async ValueTask<(bool Checking, string OrderId)> CheckingOrderIdAsync(CachedPlayer player, string orderId)
        {
            var configuration = await _service.GetConfigurationAsync().ConfigureAwait(false);

            var source = new WGSportsAccessMoneyQueryRequest()
            {
                OrderId = orderId,
            };
            var response = await _client.AccessMoneyQueryAsync(configuration, source).ConfigureAwait(false);
            if (response.ReturnCode != 1)
            {
                return (false, null);
            }

            return (true, orderId);
        }

    }
}
